using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{
    public CapsuleCollider MainCollider;
    public GameObject Rig;
    public Animator Animator;

    void Start()
    {
        Rig = this.gameObject;
        Animator = GetComponent<Animator>();
        GetRagdollBits();
        RagdollModeOff();
    }

    void Update()
    {
        GetRagdollBits();


        if (Input.GetKeyDown(KeyCode.X))
        {
            RagdollModeOn();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            RagdollModeOff();
        }
    }

    public Collider[] ragDollColliders;
    public Rigidbody[] limbsRigibodies;
    private void GetRagdollBits()
    {
        ragDollColliders = Rig.GetComponentsInChildren<Collider>();
        limbsRigibodies = Rig.GetComponentsInChildren<Rigidbody>();
    }

    private void RagdollModeOn()
    {
        GetComponentInParent<AgentMovement>().movementOff = true;

        Animator.enabled = false;
        MainCollider.enabled = false;

        foreach (Collider col in ragDollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in limbsRigibodies)
        {
            rigid.isKinematic = false;
        }

        GetComponentInParent<Rigidbody>().isKinematic = true;
    }
    private void RagdollModeOff()
    {
        GetComponentInParent<AgentMovement>().movementOff = false;

        Animator.enabled = true;

        foreach (Collider col in ragDollColliders)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rigid in limbsRigibodies)
        {
            rigid.isKinematic = true;
        }

        MainCollider.enabled = true;
        GetComponentInParent<Rigidbody>().isKinematic = false;
    }
}
