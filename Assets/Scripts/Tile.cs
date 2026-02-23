using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Action<Tile> DestroyTile;

    public void MoveTile(int moveSpeed) =>
        transform.position += new Vector3(0, 0, moveSpeed) * Time.deltaTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
            DestroyTile?.Invoke(this);
        }
    }
}
