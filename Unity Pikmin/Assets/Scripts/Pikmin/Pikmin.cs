using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using System;

public enum PikminState {STAY, FOLLOW, ATTACK, FLY}

public class Pikmin : MonoBehaviour, ICollider
{
    public  GameObject       leefParticle0, leefParticle1;

    public  PikminState      state;
    private Vector3          flyTarget;
    private Transform        followTarget;
    private NavMeshAgent     agent;
    private Rigidbody        rigid;
    private Removable        removable;
    private CapsuleCollider  col;

    private Animator         anim;

    private Action           stayAct, followAct, flyAct, attackAct;
    private bool             isDelivery, isJump;

    //
    public test testScript;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        state = PikminState.STAY;

        var charAnim = transform.GetChild(0).GetComponent<AnimSetting>();

        charAnim.AddAct("Attack", () => { if (testScript != null) testScript.hp--; });
    }

    private void Start() => SetAction();

    private void SetAction()
    {
        stayAct = () =>
        {
            Stay();
            anim.SetInteger("animation", 1);

            if(!followTarget)
            {
                leefParticle0.SetActive(true);
                leefParticle1.SetActive(true);
            }
        };
        followAct = () =>
        {
            Move();
            anim.SetInteger("animation", 2);

            leefParticle0.SetActive(false);
            leefParticle1.SetActive(false);
        };
        flyAct    = () => Fly();
        attackAct = () => 
        {
            Attack();
            anim.SetInteger("animation", 3);
        };
    }

    private void JumpCheck()
    { 

    }

    private void Stay()
    {
        agent.enabled = false;

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
        //RaycastHit rayHit;

        //if(!Physics.Raycast(transform.position,Vector3.down, out rayHit, 0.5f))
        //{
        //    rigid.useGravity = true;
        //    rigid.isKinematic = false;
        //}
        //else if(rayHit.transform.CompareTag("Floor"))
        //{
        //    rigid.useGravity = false;
        //    rigid.isKinematic = true;
        //}

        agent.enabled = true;
        agent.SetDestination(followTarget.position);

        if (Vector3.Distance(transform.position, followTarget.position) < 2.0f)
        {
            state = PikminState.STAY;
        }
    }

    private void Fly()
    {
        col.enabled = true;
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, 0.5f))
        {
            if (rayHit.transform.CompareTag("Floor"))
            {
                col.enabled = true;
                rigid.velocity = Vector3.zero;
                rigid.useGravity = false;
                rigid.isKinematic = true;
                state = PikminState.STAY;
            }
        }

        if (Vector3.Distance(transform.position, flyTarget) < 0.5f)
        {
            agent.enabled = true;
            rigid.isKinematic = true;

            if(removable != null)
            {
                isDelivery = true;

                removable.Arrangement(transform);
                agent.stoppingDistance = 0.2f;
                state = PikminState.FOLLOW;
            }

            else
            {
                followTarget = null;
                state = PikminState.STAY;
            }
        }
    }

    private void Attack()
    {
        if (testScript == null)
        {
            rigid.useGravity = true;

            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, Vector3.down, out rayHit, 0.5f))
            {
                if (rayHit.transform.CompareTag("Floor"))
                {
                    rigid.velocity = Vector3.zero;
                    rigid.useGravity = false;
                    rigid.isKinematic = true;
                    state = PikminState.STAY;
                }
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
            if (transform.parent.parent.CompareTag("Enemy"))
            {
                transform.parent = null;
            }
            else
            {
                Removable removable = transform.parent.parent.GetComponent<Removable>();

                isDelivery = false;
                transform.parent = null;
                removable.FinishWork();
            }
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

    public void FlyPikmin (Vector3 startPos, Vector3 endPos, Removable removableScript)
    {
        removable = removableScript;

        Vector3 _parabola = Parabola.CalculateVelocity(endPos, startPos, 1.5f);
        //transform.rotation = Quaternion.identity;
        rigid.isKinematic = false;
        rigid.velocity = _parabola;

        transform.parent = null;
        followTarget = null;

        state = PikminState.FLY;
        flyTarget = endPos;

        rigid.useGravity = true;
        agent.enabled = false;

        Vector3 temp0 = flyTarget;
        Vector3 temp1 = transform.position;
        temp0.y = temp1.y = 0;

        transform.LookAt(flyTarget);
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
        rigid.velocity = Vector3.zero;
    }

    public void AttackPikmin(test enemy)
    {
        StopAllCoroutines();
        PikminTarget = null;
        transform.parent = enemy.transform;
        state = PikminState.ATTACK;
        testScript = enemy;
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;
        transform.LookAt(transform);
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