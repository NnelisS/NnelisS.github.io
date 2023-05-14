using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private Transform player;
    private Vector3 groundPos;

    private void Start()
    {
        player = FindObjectOfType<AgentMovement>().transform;
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.transform.localPosition, Vector3.down, out hit))
        {
            groundPos = new Vector3(player.position.x, hit.transform.position.y + 0.0001f, player.position.z);
            Debug.DrawLine(hit.transform.localPosition, player.transform.localPosition, Color.blue);
        }

        Debug.DrawRay(player.transform.position, Vector3.down, Color.green, 0.1f);

        transform.position = new Vector3(player.transform.position.x, groundPos.y, player.transform.position.z);
    }
}
