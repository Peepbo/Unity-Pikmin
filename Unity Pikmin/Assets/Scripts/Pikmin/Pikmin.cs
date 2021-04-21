using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using System;

public class Pikmin : MonoBehaviour
{
    public GameObject leefParticle0, leefParticle1, bottomPoint;

    public float hp;

    public PikminState state;
    private Vector3 flyTarget;
    private Transform followTarget;
    private NavMeshAgent agent;
    private Rigidbody rigid;
    public Removable removable;
    private CapsuleCollider col;

    private Animator anim;
    public bool isDelivery;

    public EnemyManager enemy;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        state = PikminState.STAY;

        var charAnim = transform.GetChild(0).GetComponent<AnimSetting>();

        charAnim.AddAct("Attack", () => { if (enemy != null) enemy.Damaged(1); });
        charAnim.AddAct("Follow", () => state = PikminState.FOLLOW);
        charAnim.AddAct("Hit", () => state = PikminState.ATTACK);
        charAnim.AddAct("Die", () => ObjectPool.instance.ReturnObject(gameObject));
    }

    public void Damaged(float value)
    {
        hp -= value;

        if(hp <= 0)
        {
            PlayerController.instance.allNums--;
            
            if(PlayerController.instance.myPikminCount > 0)
                PlayerController.instance.myPikminCount--;
            if(PlayerController.instance.orderNums > 0) 
                PlayerController.instance.orderNums--;

            anim.SetTrigger("Die");
        }
    }

    private bool CheckGround()
    {
        if (transform.parent != null) return true;

        Debug.DrawRay(transform.position + Vector3.up * 0.25f, Vector3.down * 1f, Color.red);

        RaycastHit _hit;
        int layerMask = 1 << LayerMask.NameToLayer("ground");
        if (Physics.Raycast(transform.position + Vector3.up * 0.25f, Vector3.down, out _hit, 1f, layerMask))
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

        if(removable || enemy)
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

                else if(enemy)
                {
                    Vector3 _tempPos = enemy.transform.position;
                    _tempPos.y = transform.position.y;

                    transform.LookAt(_tempPos);
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
        if (Vector3.Distance(transform.position, followTarget.position) >= 0.25f)
        {
            agent.enabled = true;
            state = PikminState.FOLLOW;
        }

        else
        {
            if (enemy == null)
            {
                if (CheckGround()) state = PikminState.STAY;
            }
        }
    }

    public void WorkPikmin()
    {
        if(removable)
        {
            isDelivery = true;

            transform.parent = removable.factory;
            removable.Arrangement();
            agent.stoppingDistance = 0.2f;
            state = PikminState.FOLLOW;
        }

        else if(enemy)
        {
            transform.parent = enemy.factory;
            enemy.Arrangement();
            agent.stoppingDistance = 0.1f;
            state = PikminState.FOLLOW;
        }
    }

    private void FixedUpdate() => Animation(); 

    private void Animation()
    {
        switch (state)
        {
            case PikminState.STAY:

                if (!CheckGround()) return;

                Stay();
                anim.SetInteger("animation", 1);

                if (!followTarget)
                {
                    leefParticle0.SetActive(true);
                    leefParticle1.SetActive(true);
                }
                break;
            case PikminState.FOLLOW:

                if (!CheckGround()) return;

                Move();
                anim.SetInteger("animation", 2);

                leefParticle0.SetActive(false);
                leefParticle1.SetActive(false);
                break;
            case PikminState.FLY:

                if (!CheckGround()) return;

                Fly();
                break;
            case PikminState.ATTACK:

                anim.SetInteger("animation", 3);

                Attack();
                break;
            case PikminState.CALL:

                anim.SetInteger("animation", 6);
                break;
            case PikminState.HIT:

                anim.SetInteger("animation", 5);
                break;
        }
    }

    public void Init()
    {
        if (CheckGround()) state = PikminState.CALL;
        else state = PikminState.STAY;

        agent.stoppingDistance = 2f;

        if (transform.parent != null)
        {
            transform.parent = null;

            if(isDelivery)
            {
                isDelivery = false;
                removable.FinishWork();
                removable = null;
            }
            else if(enemy)
            {
                enemy.FinishWork();
                enemy = null;
            }
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

    public void AttackPikmin(EnemyManager enemy)
    {
        StopAllCoroutines();
        PikminTarget = null;
        transform.parent = enemy.factory;
        state = PikminState.ATTACK;
        this.enemy = enemy;

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