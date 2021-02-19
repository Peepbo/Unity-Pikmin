using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RemovableObj : MonoBehaviour
{
    public GameObject colPrefab;

    [Header("SETTINGS")]
    public float radius;
    public float radius2;
    public int needNum;

    [Header("INFO")]
    public int inNum;

    [Header("GIZMO")]
    public Color gizmoColor;
    public float gizmoSize;

    Stack<Pikmin> pikminStack = new Stack<Pikmin>();

    private void Start()
    {
        GameObject[] colObj = new GameObject[needNum];

        float angle;
        float PI2 = Mathf.PI * 2;

        for (int i = 0; i < needNum; i++)
        {
            colObj[i] = Instantiate(colPrefab, transform);
            colObj[i].transform.position += Vector3.down;

            angle = i * PI2 / needNum;
            colObj[i].transform.position += new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    private void Update()
    {
        if(inNum > 0)
        {

        }
    }

    public bool isFull() { return inNum == needNum ? true : false; }

    public Transform Positioning(Pikmin pikmin)
    {
        inNum++;
        pikminStack.Push(pikmin);
        return transform.GetChild(inNum - 1);
    }

    public void StopCarrying() //물체에 좌클릭이 충돌했을 때 실행
    {
        while (inNum > 0)
        {
            inNum--;
            pikminStack.Pop();
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position+Vector3.down, Vector3.down, radius);

        Gizmos.color = gizmoColor;

        float PI2 = Mathf.PI * 2f;

        float angle;
        for(int i = 0; i < needNum; i++)
        {
            angle = i * PI2 / needNum;
            Gizmos.DrawWireSphere(transform.position + Vector3.down + new Vector3(Mathf.Cos(angle) * radius,
    0, Mathf.Sin(angle) * radius), gizmoSize);
        }

        Gizmos.DrawSphere(transform.position, radius2);
    }
}
