using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public enum PikminState
{
    STAY,
    FOLLOW,
    FLY,
    ATTACK
}

public class Pikmin : MonoBehaviour
{
    public bool isAvoid = false;
    public PikminState state;

    public Vector3 flyTarget;

    public Transform target;
    private NavMeshAgent agent;
    private Rigidbody rigid;

    float checkTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        state = PikminState.STAY;
    }

    private void Start()
    {
        target = PlayerController.GetPos;
    }

    public void FlyPikmin(Vector3 pos)
    {
        agent.enabled = false;
        state = PikminState.FLY;
        flyTarget = pos;
        rigid.useGravity = true;
        transform.parent = null;
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
            case PikminState.FLY:
                {
                    if(Vector3.Magnitude(flyTarget - transform.position) < 1f)
                    {
                        state = PikminState.STAY;
                        Rigidbody rigid = GetComponent<Rigidbody>();
                        rigid.isKinematic = true;
                        rigid.Sleep();
                        agent.enabled = true;
                    }

                    else 
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
                        {
                            checkTime += Time.deltaTime;
                        }

                        if(checkTime > 1.3f)
                        {
                            checkTime = 0;
                            state = PikminState.STAY;
                            Rigidbody rigid = GetComponent<Rigidbody>();
                            rigid.isKinematic = true;
                            rigid.Sleep();
                            agent.enabled = true;
                        }
                    }
                }
                break;
            case PikminState.ATTACK:
                break;
        }
    }

    public void PickMe() 
    { 
        rigid.useGravity = false;
        agent.enabled = false;
    }
}