using UnityEngine;

public class Caller : MonoBehaviour
{
    public Receiver receiver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        receiver.Hello();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
