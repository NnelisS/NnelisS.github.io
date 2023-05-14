using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SoundBarrier : MonoBehaviour
{
    private AgentMovement agent;

    public List<Transform> EnemyHeard = new List<Transform>();

    [Header("Noise Info")]
    [Range(0, 20)]
    [SerializeField] private float currentNoise;

    [Space(20)]
    [Header("Noise Settings")]
    [SerializeField] private float MaxWalkNoise = 10;
    [SerializeField] private float MaxRunNoise = 20;

    [SerializeField] private LayerMask enemyLayer;

    void Start()
    {
        agent = GetComponentInParent<AgentMovement>();
        StartCoroutine("FindTargetsWithDelay", 0.2f);

    }

    void Update()
    {
        if (agent.currentVelocity > 0 && agent.currentVelocity <= 2.5f)
        {
            if (currentNoise >= MaxWalkNoise)
                currentNoise = MaxWalkNoise;

            if (currentNoise <= MaxWalkNoise)
                currentNoise += 0.2f * agent.currentVelocity * 4 * Time.deltaTime;
        }
        else if (agent.currentVelocity > 2.5f && agent.currentVelocity <= 8)
        {
            if (currentNoise <= MaxRunNoise)
                currentNoise += 0.2f * agent.currentVelocity * 4 * Time.deltaTime;
        }
        else if (agent.currentVelocity <= 0)
            if (currentNoise >= 0)
                currentNoise -= 1 * Time.maximumDeltaTime;
    }

    private void NoiseHit()
    {
        EnemyHeard.Clear();
        Collider[] enemy = Physics.OverlapSphere(transform.position, currentNoise, enemyLayer);

        for (int i = 0; i < enemy.Length; i++)
        {
            Transform target = enemy[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < 360 / 2)
            {
                Debug.Log(target.name);

                EnemyHeard.Add(target);
                for (int a = 0; a < EnemyHeard.Count; a++)
                {
                    EnemyHeard[a].GetComponent<AIMovement>().GoToPos = transform.position;
                    EnemyHeard[a].GetComponent<AIMovement>().state = AgentState.find;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawSolidArc(transform.position, Vector3.down, Vector3.right, 360, currentNoise);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            NoiseHit();
        }
    }
}
