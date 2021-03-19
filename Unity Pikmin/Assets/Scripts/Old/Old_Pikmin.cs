using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using System;

public class Old_Pikmin : MonoBehaviour, ICollider
{
    public RemovableObj objScript;

    public PikminState state;
    public Vector3 flyTarget;
    public CapsuleCollider col;

    public bool isDelivery;
    private bool isArrive;

    private Transform target;
    private NavMeshAgent agent;
    private Rigidbody rigid;
    private float checkTime;

    private Action stayAct, followAct, flyAct, attackAct;

    private void Awake()
    {
        col = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        state = PikminState.STAY;
    }

    private void Start()
    {
        target = PlayerController.FootPos;

        SetAction();
    }

    public void Init()
    {
        transform.parent = null;
        objScript = null;
        isDelivery = false;
    }

    private void SetAction()
    {
        stayAct = () =>
        {
        };

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
                if (agent.enabled == false) return;

                if (!isDelivery)
                {
                    followAct();
                }
                else
                {
                    CheckMove();
                }
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
        Vector3 angle = (target.position - transform.position).normalized;
        agent.stoppingDistance = 0.75f;
        agent.SetDestination(transform.position + angle);
    }

    private void CheckMove()
    {
        float distance = Vector3.Magnitude(target.position - transform.position);

        if (distance < 0.3f)
        {
            if (objScript != null)
            {
                if (!isArrive) StartCoroutine(ArriveTime());
            }
        }

        if (objScript != null)
        {
            agent.stoppingDistance = 0.2f;
            agent.SetDestination(target.position);
        }
    }

    IEnumerator ArriveTime()
    {
        isArrive = true;
        yield return new WaitForSeconds(1.5f);
        objScript.arriveNum++;
        Vector3 goal = target.position;
        goal.y = transform.position.y;
        transform.position = goal;
        agent.enabled = false;
        isArrive = false;
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

            //ObjectCheck();
            FindObject(objScript);
        }

        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
            {
                checkTime += Time.deltaTime;
            }

            if (checkTime > 1.0f)
            {
                checkTime = 0;
                state = PikminState.STAY;
                Rigidbody rigid = GetComponent<Rigidbody>();
                rigid.isKinematic = true;
                rigid.Sleep();
                agent.enabled = true;

                //ObjectCheck();
                FindObject(objScript);
            }
        }
    }

    public void FindObject(RemovableObj script)
    {
        if (script == null) return;
        objScript = script;
        if (objScript.isFull()) return;

        state = PikminState.FOLLOW;

        ChangeTarget = objScript.Positioning(this);
        transform.parent = ChangeTarget;
        isDelivery = true;
        col.enabled = false;
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

    public void PickMe(Transform parent)
    {
        GetComponent<CapsuleCollider>().enabled = false;
        transform.parent = parent;
        transform.position = parent.position;
        transform.rotation = Quaternion.identity;
        state = PikminState.ATTACK;
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
        set
        {
            target = value;
            agent.enabled = true;
            isDelivery = false;
            col.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, 2f);
    }

    public void PushedOut(Vector3 direction)
    {
        transform.Translate(direction * Time.deltaTime);
    }
}