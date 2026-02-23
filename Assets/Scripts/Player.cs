using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject tile;
   
    [SerializeField]
    private float moveSpeed;

    private bool leftWallCollision = false;
    private bool rightWallCollision = false;
    
    public Action<Player> TriggerNewTile;

    public void InitializePlayer() =>
        transform.position = new Vector3(0, 0, 0); 

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TileTrigger"))
        {
            TriggerNewTile?.Invoke(this);
            //Instantiate(tile, new Vector3(0, -3, 226), Quaternion.identity);
        }

        if (other.gameObject.CompareTag("LeftWall"))
        {
            leftWallCollision = true;
        }

        if (other.gameObject.CompareTag("RightWall"))
        {
            rightWallCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("LeftWall"))
        {
            leftWallCollision = false;
        }

        if (other.gameObject.CompareTag("RightWall"))
        {
            rightWallCollision = false;
        }
    }



    public void MovePlayer()
    {
        var moveX = Input.GetAxisRaw("Horizontal");
        if (leftWallCollision && moveX < 0)
        {
            moveX = 0;    
        }

        if (rightWallCollision && moveX > 0)
        {
           moveX = 0; 
        }

        var movement = new Vector3(moveX, 0f, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}
