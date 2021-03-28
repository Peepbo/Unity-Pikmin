using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using System;

public enum PikminState { STAY, FOLLOW, ATTACK, FLY }

public class Pikmin : MonoBehaviour, ICollider
{
    public GameObject leefParticle0, leefParticle1, bottomPoint;

    public PikminState state;
    private Vector3 flyTarget;
    private Transform followTarget;
    private NavMeshAgent agent;
    private Rigidbody rigid;
    private Removable removable;
    private CapsuleCollider col;

    private Animator anim;

    private Action stayAct, followAct, flyAct, attackAct;
    private bool isDelivery, isJump;

    //
    public test testScript;
    public Enemy enemyScript;

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
            if (!CheckGround()) return;

            Stay();
            anim.SetInteger("animation", 1);

            if (!followTarget)
            {
                leefParticle0.SetActive(true);
                leefParticle1.SetActive(true);
            }
        };
        followAct = () =>
        {
            if (!CheckGround()) return;

            Move();
            anim.SetInteger("animation", 2);

            leefParticle0.SetActive(false);
            leefParticle1.SetActive(false);
        };
        flyAct = () =>
        {
            if (!CheckGround()) return;

            Fly();
        };
        attackAct = () =>
        {
            anim.SetInteger("animation", 3);

            Attack();
        };
    }

    private bool CheckGround()
    {
        Debug.DrawRay(bottomPoint.transform.position, Vector3.down * 0.1f);

        RaycastHit _hit;
        if (Physics.Raycast(bottomPoint.transform.position, Vector3.down, out _hit, 0.1f))
        {
            if (_hit.transform.CompareTag("Floor"))
            {
                rigid.useGravity = false;
                rigid.isKinematic = true;
                agent.enabled = true;
                return true;
            }
        }

        anim.SetInteger("animation", 1);
        rigid.useGravity = true;
        rigid.isKinematic = false;
        agent.enabled = false;
        return false;
    }

    private void Stay()
    {
        col.enabled = true;
        agent.enabled = false;

        if (followTarget != null)
        {
            if (!isDelivery && Vector3.Distance(transform.position, followTarget.position) > 2.0f)
            {
                state = PikminState.FOLLOW;
            }
        }
    }

    private void Move()
    {
        col.enabled = false;
        agent.enabled = true;
        agent.SetDestination(followTarget.position);

        if (!isDelivery)
        {
            agent.speed = 3.5f;
            if (Vector3.Distance(transform.position, followTarget.position) < 2.0f)
            {
                state = PikminState.STAY;
            }
        }
        else agent.speed = transform.parent.parent.GetComponent<NavMeshAgent>().speed;
    }

    private void Fly()
    {
        agent.enabled = true;
        rigid.isKinematic = true;

        if (removable != null)
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

    private void Attack()
    {
        if (testScript == null)
        {
            if (CheckGround()) state = PikminState.STAY;
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
        testScript = null;
        enemyScript = null;
        transform.rotation = Quaternion.identity;

        if (transform.parent != null)
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

    public void FlyPikmin(Vector3 startPos, Vector3 endPos, Removable removableScript)
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

        while (t < duration)
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
        set { followTarget = value; }
    }

    public void PushedOut(Vector3 direction) => transform.Translate(direction * Time.deltaTime);
}