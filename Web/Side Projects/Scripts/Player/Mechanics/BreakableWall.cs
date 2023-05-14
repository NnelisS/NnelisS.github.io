using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public List<Rigidbody> WallParts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerFist"))
        {
            for (int i = 0; i < WallParts.Count; i++)
            {
                WallParts[i].isKinematic = false;
                WallParts[i].AddExplosionForce(Random.Range(50, 250), Vector3.right, 50);
            }
        }
    }
}
