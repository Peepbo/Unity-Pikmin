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

    public void Damaged(int value)
    {
        hp -= value;
    }

    //IInteractionObject .. interface
    abstract public void Arrangement();

    abstract public void Expansion();

    abstract public void FinishWork();

    abstract public void Reduction();

    //IObject .. interface
    abstract public float infoSize { get; set; }

    abstract public ObjectType objectType { get; set; }

    //Gizmo
    abstract public void OnDrawGizmos();
}