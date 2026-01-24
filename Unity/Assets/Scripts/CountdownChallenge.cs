using UnityEngine;

public class CountdownChallenge : MonoBehaviour
{
    public int countdown;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(countdown <= 0) return;

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = 1;
            countdown--;
            Debug.Log("Time Left: " + countdown);

            transform.position += Vector3.up;
            if(countdown == 0)
            {
                Debug.Log("TIMES UP");
            }
        }
    }
}
