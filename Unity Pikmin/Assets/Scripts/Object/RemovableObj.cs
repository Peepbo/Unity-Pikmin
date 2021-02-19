using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

public class RemovableObj : MonoBehaviour
{
    bool isArrive;

    public GameObject colPrefab;

    [Header("SETTINGS")]
    public float radius;
    public float radius2;
    public int needNum;

    [Header("INFO")]
    public int inNum;
    public int arriveNum;

    [Header("GIZMO")]
    public Color gizmoColor;
    public float gizmoSize;

    private NavMeshAgent agent;

    private TextMesh textMesh;

    Stack<Pikmin> pikminStack = new Stack<Pikmin>();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        textMesh = transform.GetComponentInChildren<TextMesh>();
        textMesh.text = needNum.ToString();
    }

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
        if (isArrive) return;

        if (arriveNum == needNum)
        {
            agent.enabled = true;

            Vector3 des = Spaceship.pos;
            des.y = transform.position.y;
            agent.SetDestination(des);
            if (Vector3.Distance(transform.position, des) < 0.1f)
            {
                StopCarrying();
                agent.enabled = false;
                isArrive = true;
            }
        }
        else agent.enabled = false;
    }

    public bool isFull() { return inNum == needNum ? true : false; }

    public Transform Positioning(Pikmin pikmin)
    {
        inNum++;
        textMesh.text = needNum.ToString() + "\n─\n" + inNum.ToString();
        pikminStack.Push(pikmin);
        return transform.GetChild(inNum);
    }

    public void StopCarrying() //물체에 좌클릭이 충돌했을 때 실행
    {
        arriveNum = 0;
        while (inNum > 0)
        {
            inNum--;
            textMesh.text = needNum.ToString() + "─" + inNum.ToString();
            Pikmin pk = pikminStack.Pop();
            pk.transform.parent = null;
            pk.objScript = null;
            pk.isDelivery = false;
        }
        textMesh.text = needNum.ToString();
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
