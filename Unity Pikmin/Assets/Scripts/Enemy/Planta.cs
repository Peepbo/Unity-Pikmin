using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class Planta : EnemyManager
{
    [Header("Bee Settings")]
    public bool isSleep;
    public float findRadius;

    public float restTime;

    public Transform target;
    public ParticleSystem buble;

    protected override void Start()
    {
        base.Start();
        anim.SetInteger("animation", 0);
        restTime = 1f;
        isSleep = true;

        var _animSettngs = anim.GetComponent<AnimSetting>();
        _animSettngs.AddAct("Attack", Attack);
        _animSettngs.AddAct("EndAttack", () => state = EnemyState.STAY);
    }

    public override void Damaged(int value)
    {
        base.Damaged(value);

        if (isDie) return;

        if (isSleep)
        {
            isSleep = false; 

            var em = buble.emission;
            em.enabled = false;

            anim.SetInteger("animation", 1);
        }

        if (hp <= 0)
        {
            anim.SetTrigger("Die");
            isDie = true;
            base.CheckDie();
        }
    }

    private void Attack()
    {
        Pikmin _pik = target.GetComponent<Pikmin>();
        //_pik.Init();

        if(_pik.PikminTarget != transform)
        {
            if (PlayerController.instance.myPikminCount > 0)
                PlayerController.instance.myPikminCount--;
            if (PlayerController.instance.orderNums > 0)
                PlayerController.instance.orderNums--;
        }
        //_pik.PikminTarget = null;
        _pik.state = PikminState.HIT;
        _pik.Damaged(1);
        _pik.GetComponent<NavMeshAgent>().velocity = Vector3.zero;

        target = null;
        restTime = 0.75f;
    }

    protected override void Stay()
    {
        if(restTime > 0f) restTime -= Time.deltaTime;

        else
        {
            if (factory.childCount > 0)
            {
                target = factory.GetChild(0);
                state = EnemyState.MOVE;
            }

            else
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, findRadius);

                foreach (Collider col in cols)
                {
                    if (col.CompareTag("Pikmin"))
                    {
                        target = col.transform;

                        state = EnemyState.MOVE;
                        break;
                    }
                }
            }
        }
    }

    protected override void Move()
    {
        Debug.DrawRay(anim.transform.position, anim.transform.forward * 2.5f, Color.red);

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f;
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        anim.transform.rotation = Quaternion.RotateTowards(anim.transform.rotation,
            lookRotation, 50f * Time.deltaTime);

        if (Quaternion.Angle(lookRotation, anim.transform.rotation) < 1f)
        {
            float distance = (target.position - transform.position).magnitude;

            if (distance > 2.5f)
            {
                agent.SetDestination(target.position);
            }

            else
            {
                agent.velocity = Vector3.zero;
                state = EnemyState.ATTACK;
            }
        }
    }

    protected override void Animation()
    {
        switch (state)
        {
            case EnemyState.STAY:

                if (isSleep) return;
                anim.SetInteger("animation", 1);
                Stay();

                break;
            case EnemyState.MOVE:
                anim.SetInteger("animation", 2);
                Move();

                break;
            case EnemyState.ATTACK:
                anim.SetInteger("animation", 3);

                break;
        }
    }

    //Gizmo
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, findRadius);
    }
}