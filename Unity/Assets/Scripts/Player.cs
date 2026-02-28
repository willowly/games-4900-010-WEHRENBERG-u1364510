using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum State {
        Neutral,
        Grabbing,
    }
    public State state;
    [Header("Movement")]
    public float speed;
    public float acceleration;
    public float airControl;
    public float airDrag;
    public float gravity;
    public float jumpHeight;
    public float jumpFloatForce;
    public float coyoteTime;
    public float lookSpeed;
    public float landShakeThreshold;
    public float landShakeFactor;
    public float landShakeMaximum;
    public float landImpactThreshold;
    public GameObject landImpact;

    

    [Header("References")]
    public Transform head;
    public CharacterController characterController;
    public PlayerInputMap map;
    
    
    private Vector3 velocity;
    private Vector2 lookRotation;
    private float jumpInput;
    private bool jumpInputHeld;
    private bool wasGrounded;
    private float wasGroundedTimer;


    
    private float timeInState;
    private float grabInput;
    private bool grabHeld;
    
    //private float jumpFloating;
    
    private void Awake() {
        map = new PlayerInputMap();
        map.Enable();

        map.Player.Jump.started += JumpStart;
        map.Player.Jump.canceled += JumpEnd;
    }

    void FixedUpdate()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        velocity += Vector3.down * gravity * Time.fixedDeltaTime;

        timeInState += Time.deltaTime;
        
        switch(state) {
            case State.Neutral:
                StateNeutral();
                break;
        }

        jumpInput -= Time.fixedDeltaTime;
        grabInput -= Time.fixedDeltaTime;

        wasGrounded = characterController.isGrounded;


    }

    void SetState(State newState) {
        state = newState;
        timeInState = 0;
    }

    void StateNeutral() {

        // DO movement
        var move = map.Player.Move.ReadValue<Vector2>().normalized;
        if(characterController.isGrounded) {
            // Ground movement
            velocity.x = SmoothAxis(velocity.x,move.x);
            velocity.z = SmoothAxis(velocity.z,move.y);
            characterController.Move(transform.rotation * velocity * Time.deltaTime); // uses local space (to allow turning)
            wasGroundedTimer = 0;
        } else {
            // Air movement
            DoAirMovement(move);
            wasGroundedTimer += Time.fixedDeltaTime;
        }


        // transform velocity between local and world space
        if(wasGrounded && !characterController.isGrounded) {
            velocity = transform.rotation * velocity;
        }
        if(!wasGrounded && characterController.isGrounded) {
            velocity = Quaternion.Inverse(transform.rotation) * velocity;
        }

        

        

        if(wasGroundedTimer <= coyoteTime) CheckJumpInput();
        if(velocity.y > 0 && jumpInputHeld) {
            velocity.y += jumpFloatForce * Time.fixedDeltaTime;
        }
    }

   


    float SmoothAxis(float velocity,float move) {
        if(move != 0 && Mathf.Sign(velocity) != Mathf.Sign(move)) velocity = 0; //change directions instantly
        return Mathf.MoveTowards(velocity,move * speed,speed * acceleration * Time.fixedDeltaTime);
    }

    float SmoothAxisAir(float velocity,float move) {
        var acc = airControl;
        if(move == 0) {
            acc = airDrag;
        } else {
            if(Mathf.Sign(velocity) == Mathf.Sign(move) && Mathf.Abs(velocity) > Mathf.Abs(move * speed)) { //dont limit high speed
                return velocity;
            }
        }
        return Mathf.MoveTowards(velocity,move * speed,speed * acc * Time.fixedDeltaTime);
    }

    void DoAirMovement(Vector2 move) {
        move = Quaternion.Euler(0,0,-transform.eulerAngles.y) * move;
        velocity.x = SmoothAxisAir(velocity.x,move.x);
        velocity.z = SmoothAxisAir(velocity.z,move.y);
        characterController.Move(velocity * Time.deltaTime); //uses world space 
    }

    void Update()
    {
        var lookVector = map.Player.Look.ReadValue<Vector2>() * lookSpeed;
        lookRotation += lookVector;
        lookRotation.y = Mathf.Clamp(lookRotation.y,-89,89);
        transform.localEulerAngles = Vector3.up * lookRotation.x;
        head.localEulerAngles = Vector3.left * lookRotation.y;
    }

    IEnumerator HitStop(float time) {
        float timeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = timeScale;
    }



    bool CheckJumpInput() {
        if(jumpInput > 0) {
            jumpInput = 0;
            velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
            return true;
        }
        return false;
    }

    void JumpStart(InputAction.CallbackContext e) {
        jumpInput = 0.3f;
        jumpInputHeld = true;
    }

    void JumpEnd(InputAction.CallbackContext e) {
        jumpInputHeld = false;
    }

    void GrabStart(InputAction.CallbackContext e) {
        grabInput = 0.3f;
        grabHeld = true;
    }

    void GrabEnd(InputAction.CallbackContext e) {
        grabInput = 0.3f;
        grabHeld = false;
    }
}
