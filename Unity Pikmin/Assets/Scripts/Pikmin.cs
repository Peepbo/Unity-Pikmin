using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public enum PikminState
{
    STAY,
    FOLLOW,
    ATTACK
}

public class Pikmin : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;
    public bool isAvoid = false;
    public PikminState state;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = PikminState.STAY;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PikminState.STAY:
                break;
            case PikminState.FOLLOW:
                {
                    float distance = Vector3.Magnitude(target.position - transform.position);

                    if (distance < 1.5f)
                        isAvoid = true;
                    else
                        isAvoid = false;

                    if (!isAvoid)
                    {
                        agent.stoppingDistance = 2f;
                        agent.isStopped = false;
                        agent.SetDestination(target.position);
                    }
                    else
                    {
                        Vector3 angle = (transform.position - target.position).normalized;
                        agent.stoppingDistance = 0.75f;
                        agent.SetDestination(transform.position + angle);
                    }
                }
                break;
            case PikminState.ATTACK:
                break;
        }
    }
}