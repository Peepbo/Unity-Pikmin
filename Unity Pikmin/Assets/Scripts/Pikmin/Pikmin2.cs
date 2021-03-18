using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using System;

public class Pikmin2 : MonoBehaviour, ICollider
{
    public PikminState state;
    public Vector3 flyTarget;

    public Transform target;
    private NavMeshAgent agent;
    private Rigidbody rigid;

    private Action stayAct, followAct, flyAct, attackAct;
    private bool isDelivery;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        state = PikminState.STAY;
    }

    private void Start()
    {
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
            Fly();
            //FlyCheck();
        };

        attackAct = () => { };
    }

    private void Move()
    {
        if (target != null)
        {
            if(isDelivery && Vector3.Distance(transform.position,target.position) < 0.3f)
            {
                agent.enabled = false;
                transform.position = target.position;
            }

            else
            {
                agent.enabled = true;
                agent.SetDestination(target.position);
            }
        }
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

    public void Init()
    {
        if(transform.parent != null)
        {
            Removable removable = transform.parent.parent.GetComponent<Removable>();

            isDelivery = false;
            transform.parent = null;
            removable.FinishWork();
        }

        agent.stoppingDistance = 2f;
    }

    public void PickMe(Transform parent)
    {
        transform.parent = parent;
        transform.position = parent.position;
        transform.rotation = Quaternion.identity;
        state = PikminState.STAY;
        rigid.useGravity = false;
        agent.enabled = false;
    }

    public void FlyPikmin(Vector3 pos)
    {
        GetComponent<CapsuleCollider>().enabled = false;
        transform.parent = null;
        state = PikminState.FLY;
        flyTarget = pos;
        rigid.useGravity = true;
        agent.enabled = false;
        StartCoroutine(RotateMe(1.4f));
    }

    private IEnumerator RotateMe(float inTime)
    {
        transform.LookAt(flyTarget);
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

    public void Fly()
    {
        if (Vector3.Distance(transform.position, flyTarget) < 0.5f)
        {
            agent.enabled = true;
            GetComponent<CapsuleCollider>().enabled = true;
            rigid.isKinematic = true;
            
            Collider[] cols = Physics.OverlapSphere(transform.position, 2);

            bool flag = true;

            foreach(var col in cols)
            {
                if(col.CompareTag("Object"))
                {
                    isDelivery = true;

                    col.GetComponent<Removable>().Arrangement(transform);
                    agent.stoppingDistance = 0.2f;
                    state = PikminState.FOLLOW;
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                target = null;
                state = PikminState.STAY;
            }

            transform.rotation = Quaternion.identity;
        }
    }

    public Transform ChangeTarget
    {
        get { return target; }
        set
        {
            target = value;
            agent.enabled = true;
        }
    }

    public void PushedOut(Vector3 direction) => transform.Translate(direction * Time.deltaTime);
}