using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetDoors : MonoBehaviour
{
    [SerializeField] private Material normal;
    [SerializeField] private Material selected;

    [SerializeField] private Transform parent;
    private MeshRenderer mesh;

    public bool OnObject = false;

    [SerializeField] private bool left = false;
    [SerializeField] private bool right = false;
    [SerializeField] bool opened = false;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (OnObject == false && mesh.material != normal)
            Normal();

        if (OnObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
                opened = !opened;
        }

        OpenDoor();
    }

    private void OpenDoor()
    {
        if (opened)
        {
            if (left)
                parent.transform.rotation = Quaternion.Lerp(parent.rotation, Quaternion.Euler(parent.rotation.x, 125, parent.rotation.z), 2 * Time.deltaTime);
            else if (right)
                parent.transform.rotation = Quaternion.Lerp(parent.rotation, Quaternion.Euler(parent.rotation.x, -125, parent.rotation.z), 2 * Time.deltaTime);
        }
        if (opened == false)
        {
            if (left)
                parent.transform.rotation = Quaternion.Lerp(parent.rotation, Quaternion.Euler(parent.rotation.x, 0, parent.rotation.z), 2 * Time.deltaTime);
            else if (right)
                parent.transform.rotation = Quaternion.Lerp(parent.rotation, Quaternion.Euler(parent.rotation.x, 0, parent.rotation.z), 2 * Time.deltaTime);
        }

    }

    public void OnTheObject()
    {
        OnObject = true;
        Selected();
    }

    private void Selected()
    {
        mesh.material = selected;
    }

    private void Normal()
    {
        mesh.material = normal;
    }
}
