using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

public class Removable : MonoBehaviour
{
    [Header("Gizmo")]
    public Color gColor;
    public float gSize, gYpos, gAngle;

    [Header("Settings")]
    private Transform factory, location;

    public GameObject colPrefab;
    public int needs, works;

    private float PI2, angle;

    private void Awake()
    {
        factory = transform.GetChild(0);
        location = transform.GetChild(1);

        PI2 = Mathf.PI * 2;
    }

    public void FinishWork()
    {
        //마지막 위치를 반환
        Reduction();
    }

    public void Arrangement(Transform trans)
    {
        trans.parent = factory;

        Relocation();
    }

    public Vector3 ThrowPos()
    {
        //새로운 위치를 할당
        Expansion();

        return location.GetChild(location.childCount - 1).position;
    }

    public void Relocation()
    {
        Pikmin2 _child = null;

        for (int i = 0; i < factory.childCount; i++)
        {
            _child = factory.GetChild(i).GetComponent<Pikmin2>();
            _child.ChangeTarget = location.GetChild(i);
        }
    }

    public void Reduction()
    {
        works--;

        FixLocation();
        Relocation();

        ObjectPool.instance.ReturnObject(location.GetChild(location.childCount - 1).gameObject);
    }

    private void Expansion()
    {
        works++;

        GameObject _colObj = ObjectPool.instance.BorrowObject("Collider");
        _colObj.transform.parent = location;

        FixLocation();
    }

    private void FixLocation()
    {
        for (int i = 0; i < works; i++)
        {
            angle = i * PI2 / works;

            location.GetChild(i).position = transform.position + Vector3.down * gYpos;
            location.GetChild(i).position += new Vector3(Mathf.Cos(angle) * gSize, 0, Mathf.Sin(angle) * gSize);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = gColor;
        Handles.DrawWireDisc(transform.position + Vector3.down * gYpos, Vector3.down, gSize);

        for (int i = 0; i < works; i++)
        {
            gAngle = i * Mathf.PI * 2 / works;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * gYpos + new Vector3(Mathf.Cos(gAngle) * gSize,
                0, Mathf.Sin(gAngle) * gSize), 0.2f);
        }
    }
}