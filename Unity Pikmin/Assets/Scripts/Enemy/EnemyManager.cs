using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class EnemyManager : MonoBehaviour, IInteractionObject
{
    [Header("Enemy Common Info")]
    public int hp;
    public int works;
    public bool isDie;
    public Transform factory, location;

    [Header("Gizmo Settings")]
    public Color gColor;
    public float gSize;
    public float gYpos;
    protected float gAngle;

    //Enemy .. abstract
    abstract public void Damaged(int value);

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