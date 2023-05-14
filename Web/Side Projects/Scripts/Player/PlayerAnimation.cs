using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator PlayerAnimator;
    public AgentMovement PlayerMovement;
    [SerializeField] private AgentMovement agent;

    void Start()
    {
        PlayerMovement = GetComponentInParent<AgentMovement>();
        PlayerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PlayerMovement.inSprint == true)
        {
            PlayerAnimator.SetBool("Running", true);
            PlayerAnimator.SetInteger("IdleWalkRun", 2);
        }
        else if (PlayerMovement.inSprint == false)
            PlayerAnimator.SetBool("Running", false);

        if (PlayerMovement.currentVelocity >= 0.5f && PlayerMovement.currentVelocity <= 3)
            PlayerAnimator.SetInteger("IdleWalkRun", 1);
        else if (PlayerMovement.currentVelocity < 0.5f)
            PlayerAnimator.SetInteger("IdleWalkRun", 0);

        if (PlayerMovement.inCrouch == true)
            PlayerAnimator.SetBool("Crouch", true);
        else if (PlayerMovement.inCrouch == false)
            PlayerAnimator.SetBool("Crouch", false);
    }

    public void OutPunch()
    {
        agent.OutPunch();
    }

    public void ColliderOn()
    {
        agent.PunchCol();
    }
}
