using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using System;

public enum PikminState { STAY, FOLLOW, ATTACK, FLY, CALL }

public class Pikmin : MonoBehaviour
{
    public GameObject leefParticle0, leefParticle1, bottomPoint;

    public PikminState state;
    private Vector3 flyTarget;
    private Transform followTarget;
    private NavMeshAgent agent;
    private Rigidbody rigid;
    public Removable removable;
    private CapsuleCollider col;

    private Animator anim;

    private Action stayAct, followAct, flyAct, attackAct;
    public bool isDelivery;

    public Enemy enemyScript;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        state = PikminState.STAY;

        var charAnim = transform.GetChild(0).GetComponent<AnimSetting>();

        charAnim.AddAct("Attack", () => { if (enemyScript != null) enemyScript.GetDamaged(1); });
        charAnim.AddAct("Follow", () => state = PikminState.FOLLOW);
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

    public bool CheckGround()
    {
        if (transform.parent != null) return true;

        Debug.DrawRay(bottomPoint.transform.position, Vector3.down * 1f, Color.red);

        RaycastHit _hit;
        if (Physics.Raycast(bottomPoint.transform.position, Vector3.down, out _hit, 1f))
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
        if(transform.parent == null)
        {
            col.enabled = true;
            if (followTarget != null)
            {
                if (!isDelivery)
                {
                    if (Vector3.Distance(transform.position, followTarget.position) > 2.5f)
                    {
                        state = PikminState.FOLLOW;
                    }
                }
            }
        }

        else if(removable != null)
        {
            transform.rotation = Quaternion.identity;
            if (removable.GetComponent<NavMeshAgent>().enabled)
            {
                state = PikminState.FOLLOW;
            }
        }
    }

    private void Move()
    {
        if (followTarget == null)
        {
            state = PikminState.STAY;
            return;
        }

        col.enabled = false;

        if (agent.enabled) agent.SetDestination(followTarget.position);

        if(removable || enemyScript)
        {
            if (Vector3.Distance(transform.position, followTarget.position) >= 0.25f)
            {
                agent.enabled = true;
            }

            else
            {
                agent.enabled = false;
                transform.position = followTarget.transform.position;

                if(removable)
                {
                    transform.LookAt(Spaceship.instance.endPos.position);

                    if (!removable.GetComponent<NavMeshAgent>().enabled)
                    {
                        state = PikminState.STAY;
                    }
                }

                else if(enemyScript)
                {
                    state = PikminState.ATTACK;
                }
            }
        }

        else
        {
            agent.enabled = true;
            agent.speed = 3.5f;

            Collider[] cols = Physics.OverlapSphere(transform.position, 0.3f);

            foreach (Collider _collider in cols)
            {
                if (_collider.CompareTag("Pikmin"))
                {
                    var script = _collider.GetComponent<Pikmin>();
                    if (script.state == PikminState.STAY)
                    {
                        state = PikminState.STAY;
                        break;
                    }
                }
            }

            if (Vector3.Distance(transform.position, followTarget.position) < 2.0f)
            {
                state = PikminState.STAY;
            }
        }
    }

    private void Fly()
    {
        col.enabled = true;
        agent.enabled = true;
        rigid.isKinematic = true;

        if (removable != null)
        {
            WorkPikmin();
        }

        else
        {
            followTarget = null;
            state = PikminState.STAY;
        }
    }

    private void Attack()
    {
        if (enemyScript == null)
        {
            if (CheckGround()) state = PikminState.STAY;
        }
    }

    public void WorkPikmin()
    {
        if(removable)
        {
            isDelivery = true;

            removable.Arrangement(transform);
            agent.stoppingDistance = 0.2f;
            state = PikminState.FOLLOW;
        }

        else if(enemyScript)
        {
            enemyScript.Arrangement(transform);
            agent.stoppingDistance = 0.2f;
            state = PikminState.FOLLOW;
        }
    }

    private void FixedUpdate() => Animation(); 

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
            case PikminState.CALL:
                anim.SetInteger("animation", 6);
                break;
        }
    }

    public void Init()
    {
        if (CheckGround()) state = PikminState.CALL;
        else state = PikminState.STAY;

        enemyScript = null;
       
        //transform.rotation = Quaternion.identity;
        agent.stoppingDistance = 2f;

        if (transform.parent != null)
        {
            if(isDelivery)
            {
                isDelivery = false;
                removable.FinishWork();
                removable = null;
            }

            transform.parent = null;
        }
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

        Vector3 _parabola = Utils.CalculateVelocity(endPos, startPos, 1.5f);

        rigid.isKinematic = false;
        rigid.useGravity = true;
        rigid.velocity = _parabola;

        transform.parent = null;
        followTarget = null;

        state = PikminState.FLY;
        flyTarget = endPos;

        agent.enabled = false;
        col.enabled = true;

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

    public void AttackPikmin(Enemy enemy)
    {
        StopAllCoroutines();
        PikminTarget = null;
        transform.parent = enemy.factory;
        state = PikminState.ATTACK;
        enemyScript = enemy;
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;
        transform.LookAt(enemy.transform);
    }

    public Transform PikminTarget
    {
        get { return followTarget; }
        set { followTarget = value; }
    }
}