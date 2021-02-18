using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using System;

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

    private Transform target;
    private NavMeshAgent agent;
    private Rigidbody rigid;
    private float checkTime;

    private Action stayAct, followAct, flyAct, attackAct;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        state = PikminState.STAY;
    }

    private void Start()
    {
        target = PlayerController.GetPos;

        SetAction();
    }

    private void SetAction()
    {
        stayAct = () => { };

        followAct = () =>
        {
            Move();
        };

        flyAct = () =>
        {
            FlyCheck();
        };

        attackAct = () => { };
    }

    // Update is called once per frame
    private void Update() => Animation();

    private void Animation()
    {
        switch (state)
        {
            case PikminState.STAY:
                stayAct();
                break;
            case PikminState.FOLLOW:
                followAct();
                break;
            case PikminState.FLY:
                flyAct();
                break;
            case PikminState.ATTACK:
                attackAct();
                break;
        }
    }

    #region Action
    private void Move()
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

    private void FlyCheck()
    {
        if (Vector3.Magnitude(flyTarget - transform.position) < 1f)
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

            if (checkTime > 1.3f)
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
    #endregion

    private IEnumerator RotateMe(float inTime)
    {
        float culTime = inTime / 2;

        Vector3 byAngles = new Vector3(-360, 0, 0);

        Quaternion fromAngle = transform.rotation;
        Quaternion toAngle = Quaternion.Euler(transform.eulerAngles + byAngles / 2);

        for (float t = 0f; t < 1f; t += Time.deltaTime / culTime)
        {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }

        toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);

        for (float t = 0f; t < 1f; t += Time.deltaTime / culTime)
        {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
    }

    public void PickMe() 
    { 
        rigid.useGravity = false;
        agent.enabled = false;
    }

    public void FlyPikmin(Vector3 pos)
    {
        agent.enabled = false;
        state = PikminState.FLY;
        flyTarget = pos;
        rigid.useGravity = true;
        transform.parent = null;
        transform.LookAt(flyTarget);

        StartCoroutine(RotateMe(1.5f));
    }

    public Transform ChangeTarget
    {
        get { return target; }
        set { target = value; }
    }
}