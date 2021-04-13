using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public abstract class EnemyManager : MonoBehaviour, IInteractionObject
{
    [Header("Enemy Common Info")]
    public int maxHp;
    public int hp;
    public int works;
    public bool isDie;
    public Transform factory, location;

    protected NavMeshAgent agent;
    protected Rigidbody rigid;
    protected Animator anim;
    protected EnemyState state;

    private float time;
    private Vector3 goal;

    [Header("Health Sprite")]
    public Transform HpBar;
    public Transform Mask;
    public Transform hpStartPoint;
    public Transform hpEndPoint;

    private float saveDistanceHpBar;

    [Header("Gizmo Settings")]
    public Color gColor;
    public float gSize;
    public float gYpos;
    protected float gAngle;


    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();

        //hpStartPoint = HpBar.GetChild(0);
        //hpEndPoint = HpBar.GetChild(1);
    }

    protected virtual void Start()
    {
        infoSize = 1f;
        objectType = ObjectType.MONSTER_OBJ;
        state = EnemyState.STAY;

        saveDistanceHpBar = hpStartPoint.localPosition.y;

        hp = maxHp;
        HpBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        Animation();

        if (hp != maxHp)
        {
            HpBar.gameObject.SetActive(true);
            HpBar.LookAt(Camera.main.transform);
        }
    }



    //IInteractionObject .. interface
    public void Arrangement()
    {
        Relocation();
    }

    public void Expansion()
    {
        works++;

        GameObject _colObj = ObjectPool.instance.BorrowObject("Collider");
        _colObj.transform.parent = location;

        FixLocation();
        Relocation();
    }

    public void FinishWork()
    {
        Reduction();
    }

    public void Reduction()
    {
        works--;

        FixLocation();
        Relocation();

        ObjectPool.instance.ReturnObject(location.GetChild(location.childCount - 1).gameObject);
    }

    private void Relocation()
    {
        Pikmin _child = null;

        for (int i = 0; i < factory.childCount; i++)
        {
            _child = factory.GetChild(i).GetComponent<Pikmin>();
            _child.PikminTarget = location.GetChild(i);
        }
    }

    private void FixLocation()
    {
        for (int i = 0; i < works; i++)
        {
            float _angle = i * Utils.PI2 / works;

            location.GetChild(i).position = transform.position + Vector3.down * gYpos;
            location.GetChild(i).position += new Vector3(Mathf.Cos(_angle) * gSize, 0, Mathf.Sin(_angle) * gSize);
        }
    }



    //IObject .. interface
    public float infoSize { get; set; }

    public ObjectType objectType { get; set; }



    //Enemy functions
    public virtual void Damaged(int value)
    {
        if (isDie) return;

        hp -= value;

        Mask.localPosition = hpStartPoint.localPosition + Vector3.down * saveDistanceHpBar * ((float)(maxHp - hp) / maxHp);

        if (hp <= 0) isDie = true;
    }

    private Vector3 NewPath(bool isCol)
    {
        if (!isCol) return transform.position + -transform.forward * Random.Range(3, 6);

        Vector3 dir = Utils.RandomVector(-Vector3.one, Vector3.one).normalized * Random.Range(5, 8);
        dir.y = 0;

        return transform.position + dir;
    }

    protected virtual void Move()
    {
        Debug.DrawRay(transform.position + transform.forward, Vector3.down * 5f);
        if (!Physics.Raycast(transform.position + transform.forward, Vector3.down, 5f))
        {
            goal = NewPath(true);
            agent.SetDestination(goal);
        }

        //almost same
        else if (Vector3.Distance(transform.position, goal) < 0.25f)
        {
            state = EnemyState.STAY;
            agent.velocity = Vector3.zero;
        }
    }

    protected virtual void Stay()
    {
        time += Time.deltaTime;

        if (time > 2.5f)
        {
            time = 0;
            state = EnemyState.MOVE;

            bool _isCol = Physics.Raycast(transform.position + transform.forward, Vector3.down, 5f);

            goal = NewPath(_isCol);
            agent.SetDestination(goal);
        }
    }

    protected abstract void Animation();



    //Gizmo
    protected virtual void OnDrawGizmos()
    {
        Handles.color = gColor;
        Handles.DrawWireDisc(transform.position + Vector3.down * gYpos, Vector3.down, gSize);

        for (int i = 0; i < works; i++)
        {
            gAngle = i * Utils.PI2 / works;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * gYpos + new Vector3(Mathf.Cos(gAngle) * gSize,
                0, Mathf.Sin(gAngle) * gSize), 0.2f);
        }
    }
}