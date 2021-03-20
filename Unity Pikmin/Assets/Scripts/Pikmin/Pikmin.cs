using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using System;

public enum PikminState {STAY, FOLLOW, ATTACK, FLY}

public class Pikmin : MonoBehaviour, ICollider
{
    public  PikminState      state;
    private Vector3          flyTarget;
    private Transform        followTarget;
    private NavMeshAgent     agent;
    private Rigidbody        rigid;

    private Animator         anim;

    private Action           stayAct, followAct, flyAct, attackAct;
    private bool             isDelivery;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        anim  = transform.GetChild(0).GetComponent<Animator>();
        state = PikminState.STAY;
    }

    private void Start() => SetAction();

    private void SetAction()
    {
        stayAct = () =>
        {
            Stay();
            anim.SetInteger("animation", 1);
        };
        followAct = () =>
        {
            Move();
            anim.SetInteger("animation", 2);
        };
        flyAct    = () => Fly();
        attackAct = () => { };
    }

    private void Stay()
    {
        //agent.enabled = false;

        if (followTarget != null)
        {
            if(!isDelivery && Vector3.Distance(transform.position,followTarget.position) > 2.0f)
            {
                state = PikminState.FOLLOW;
            }
        }
    }

    private void Move()
    {
        agent.enabled = true;
        agent.SetDestination(followTarget.position);

        if (Vector3.Distance(transform.position, followTarget.position) < 2.0f)
        {
            state = PikminState.STAY;
        }
    }

    private void Fly()
    {
        if (Vector3.Distance(transform.position, flyTarget) < 0.5f)
        {
            agent.enabled = true;
            rigid.isKinematic = true;

            Collider[] cols = Physics.OverlapSphere(transform.position, 2);

            bool flag = true;

            foreach (var col in cols)
            {
                if (col.CompareTag("Object"))
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
                followTarget = null;
                state = PikminState.STAY;
            }

            transform.rotation = Quaternion.identity;
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

    public void FlyPikmin (Vector3 startPos, Vector3 endPos)
    {
        Vector3 _parabola = Parabola.CalculateVelocity(endPos, startPos, 1.5f);
        transform.rotation = Quaternion.identity;
        rigid.isKinematic = false;
        rigid.velocity = _parabola;

        transform.parent = null;
        state = PikminState.FLY;
        flyTarget = endPos;

        rigid.useGravity = true;
        agent.enabled = false;
        StartCoroutine(RotateMe(1.5f));
    }

    private IEnumerator RotateMe(float duration)
    {
        Quaternion startRot = transform.rotation;
        float t = 0.0f;

        while(t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = startRot * Quaternion.AngleAxis(t / duration * 720f, Vector3.right);

            yield return null;
        }

        transform.rotation = startRot;
    }

    public Transform PikminTarget
    {
        get { return followTarget; }
        set
        {
            followTarget = value;
            agent.enabled = true;
        }
    }

    public void PushedOut(Vector3 direction) => transform.Translate(direction * Time.deltaTime);
}