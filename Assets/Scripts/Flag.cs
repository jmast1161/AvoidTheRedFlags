using System;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public Action<Flag> DestroyFlag;
    public Action<Flag> GameOver;
    
    public void MoveFlag(int moveSpeed)
    {   
        transform.position += new Vector3(0, 0, moveSpeed) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
            DestroyFlag?.Invoke(this);
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            GameOver?.Invoke(this);
        }
    }
}
