using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public abstract class EnemyManager : MonoBehaviour, IInteractionObject
{
    [Header("Enemy Common Info")]
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

    [Header("Gizmo Settings")]
    public Color gColor;
    public float gSize;
    public float gYpos;
    protected float gAngle;


    virtual protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    virtual protected void Start()
    {
        infoSize = 1f;
        objectType = ObjectType.MONSTER_OBJ;
        state = EnemyState.STAY;
    }

    private void Update() => Animation();



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
    public void Damaged(int value)
    {
        hp -= value;
    }

    private Vector3 NewPath(bool isCol)
    {
        if (!isCol) return transform.position + -transform.forward * Random.Range(3, 6);

        Vector3 dir = Utils.RandomVector(-Vector3.one, Vector3.one).normalized * Random.Range(5, 8);
        dir.y = 0;

        return transform.position + dir;
    }

    protected void Move()
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

    protected void Stay()
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

    abstract protected void Animation();



    //Gizmo
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, gSize);

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