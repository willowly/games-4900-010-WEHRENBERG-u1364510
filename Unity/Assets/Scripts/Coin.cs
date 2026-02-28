using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Coin Collected!");
        Destroy(gameObject);
    }
}
