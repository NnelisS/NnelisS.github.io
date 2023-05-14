using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public enum AgentState
{
    idle = 0,
    roam,
    find,
    attack
}

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovement : MonoBehaviour
{
    private float idleTime = 2;

    [Header("AI State")]
    public AgentState state;

    [Space(20)]
    [Header("Agent Info")]
    [SerializeField] private NavMeshAgent agent;

    [Space(20)]
    [Header("AI Settings")]
    public AISO AIStats;

    [HideInInspector]
    public Vector3 GoToPos;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        switch (state)
        {
            case AgentState.idle:
                Idle();
                break;
            case AgentState.roam:
                Roam();
                break;
            case AgentState.find:
                LastPos();
                break;
            case AgentState.attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        transform.Rotate(new Vector3(0, 20, 0), 100 * Time.deltaTime);

        idleTime -= Time.deltaTime;
        if (idleTime <= 0)
        {

            int minplus = Random.Range(0, 2);
            print(minplus);

            if (minplus == 0)
                agent.SetDestination(new Vector3(transform.position.x + Random.Range(AIStats.MinRoamRange, AIStats.MaxRoamRange), transform.position.y, transform.position.z + Random.Range(AIStats.MinRoamRange, AIStats.MaxRoamRange)));
            else if (minplus == 1)
                agent.SetDestination(new Vector3(transform.position.x - Random.Range(AIStats.MinRoamRange, AIStats.MaxRoamRange), transform.position.y, transform.position.z - Random.Range(AIStats.MinRoamRange, AIStats.MaxRoamRange)));

            state = AgentState.roam;

            idleTime = Random.Range(1, 2.5f);
        }
    }

    private void Roam()
    {
        if (agent.pathPending || agent.remainingDistance > 0.1f)
            return;

        state = AgentState.idle;
    }

    private void LastPos()
    {
        if (agent.pathPending || agent.remainingDistance < 0.1f)
            state = AgentState.roam;

        agent.SetDestination(GoToPos);
    }

    private void Attack()
    {
        if (agent.pathPending || agent.remainingDistance < 0.1f)
            state = AgentState.idle;

        agent.SetDestination(GoToPos);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, agent.destination);
    }
}
