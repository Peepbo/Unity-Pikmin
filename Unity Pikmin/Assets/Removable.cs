using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

public class Removable : MonoBehaviour
{
    [Header("Gizmo")]
    public Color gColor;
    public float gSize;
    public float gYpos;

    [Header("Settings")]
    public GameObject colPrefab;

    private Transform workers;
    private Transform colObjs;

    public int needs;
    public int capacity;

    public int come;

    private void Awake()
    {
        workers = transform.GetChild(0);
        colObjs = transform.GetChild(1);
    }

    private void Start()
    {
        capacity = needs;

        GameObject[] colObj = new GameObject[needs];

        float angle;
        float PI2 = Mathf.PI * 2;

        for (int i = 0; i < needs; i++)
        {
            colObj[i] = Instantiate(colPrefab, colObjs);
            colObj[i].transform.position = transform.position + Vector3.down * gYpos;

            angle = i * PI2 / needs;
            colObj[i].transform.position += new Vector3(Mathf.Cos(angle) * gSize, 0, Mathf.Sin(angle) * gSize);
        }

        for(int i = 0; i < workers.childCount; i++)
        {
            Arrangement(workers.GetChild(i));
        }
    }

    public void Arrangement(Transform trans)
    {
        trans.parent = workers;

        //CheckNumber();
        Relocation();
    }

    public Vector3 ThrowPos()
    {
        if (capacity == come)
        {
            CheckNumber();
        }
        return colObjs.GetChild(come++).position;
    }

    public void Relocation()
    {
        Pikmin2 _child = null;
        
        for (int i = 0; i < workers.childCount; i++)
        {
            _child = workers.GetChild(i).GetComponent<Pikmin2>();
            _child.ChangeTarget = colObjs.GetChild(i);
        }
    }

    private void CheckNumber()
    {
        float PI2 = Mathf.PI * 2;

        for (int i = 0; i < come + 1; i++)
        {
            float _angle = i * PI2 / (come + 1);
            if (i < come)
            {
                colObjs.GetChild(i).transform.position = transform.position + Vector3.down * gYpos;
                colObjs.GetChild(i).transform.position += new Vector3(Mathf.Cos(_angle) * gSize, 0, Mathf.Sin(_angle) * gSize);
            }

            else
            {
                GameObject colObj = Instantiate(colPrefab, colObjs);
                colObj.transform.position = transform.position + Vector3.down * gYpos;
                colObj.transform.position += new Vector3(Mathf.Cos(_angle) * gSize, 0, Mathf.Sin(_angle) * gSize);
            }
        }
        capacity = come + 1;
    }

    private void OnDrawGizmos()
    {
        Handles.color = gColor;
        Handles.DrawWireDisc(transform.position + Vector3.down * gYpos, Vector3.down, gSize);

        float _angle;
        for(int i = 0; i < come; i++)
        {
            _angle = i * Mathf.PI * 2 / come;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * gYpos + new Vector3(Mathf.Cos(_angle) * gSize,
                0, Mathf.Sin(_angle) * gSize), 0.2f);
        }
    }
}
