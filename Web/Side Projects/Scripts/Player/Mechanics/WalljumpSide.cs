using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalljumpSide : MonoBehaviour
{
    [SerializeField] private bool front = false; 
    [SerializeField] private bool back = false; 

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (front == true)
                GetComponentInParent<WallJump>().Front = true;

            if (back == true)
                GetComponentInParent<WallJump>().Back = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (front == true)
            GetComponentInParent<WallJump>().Front = false;

        if (back == true)
            GetComponentInParent<WallJump>().Back = false;
    }
}
