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

    public void test()
    { 
        come--;
        CheckNum2();
    }

    public void Arrangement(Transform trans)
    {
        trans.parent = workers;

        Relocation();
    }

    public Vector3 ThrowPos()
    {
        if (capacity == come) CheckNumber();

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

    public void CheckNum2()
    {
        float PI2 = Mathf.PI * 2;
        for(int i = 0; i < capacity; i++)
        {
            float _angle = i * PI2 / come;

            if (i < come)
            {
                colObjs.GetChild(i).position = transform.position + Vector3.down * gYpos;
                colObjs.GetChild(i).position += new Vector3(Mathf.Cos(_angle) * gSize, 0, Mathf.Sin(_angle) * gSize);
                workers.GetChild(i).GetComponent<Pikmin2>().ChangeTarget = colObjs.GetChild(i);
            }
            else
            {
                ObjectPool.instance.ReturnObject(colObjs.GetChild(i).gameObject);
                break;
            }
        }

        capacity--;
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
                GameObject colObj = ObjectPool.instance.GetObjectFromPooler("Collider");
                colObj.SetActive(true);
                colObj.transform.parent = colObjs;
                //GameObject colObj = Instantiate(colPrefab, colObjs);
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
