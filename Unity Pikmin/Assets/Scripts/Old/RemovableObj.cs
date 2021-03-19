using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

public class RemovableObj : MonoBehaviour, ICollider
{
    private bool isArrive;

    public GameObject colPrefab;

    [Header("SETTINGS")]
    public Transform target;
    public float radius;
    public float radius2;
    public float downValue;
    public int needNum;
    public int speed;

    [Header("INFO")]
    public int inNum;
    public int arriveNum;

    [Header("GIZMO")]
    public Color gizmoColor;
    public float gizmoSize;

    private NavMeshAgent agent;
    private TextSetting textSetting;
    private Stack<Old_Pikmin> pikminStack = new Stack<Old_Pikmin>();
    private Material mat;
    private Color emissionColor = new Color(0.6F, 0, 0, 3F);
    private Spaceship spaceship;

    bool stop = false;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.enabled = false;
        textSetting = transform.GetChild(0).GetComponent<TextSetting>();
        spaceship = target.GetComponent<Spaceship>();

        mat.EnableKeyword("_EMISSION");
    }

    private void Start()
    {
        //GameObject[] colObj = new GameObject[needNum];
        //
        //float angle;
        //float PI2 = Mathf.PI * 2;
        //
        //for (int i = 0; i < needNum; i++)
        //{
        //    colObj[i] = Instantiate(colPrefab, transform);
        //    colObj[i].transform.position += Vector3.down * downValue;
        //
        //    angle = i * PI2 / needNum;
        //    colObj[i].transform.position += new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        //}

        textSetting.ChangeText(needNum.ToString());
    }

    private void Update()
    {
        Arrive();

        Move();
    }

    private void Arrive()
    {
        if (isArrive)
        {
            mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), emissionColor, Time.deltaTime * 7f));

            transform.position = Vector3.Lerp(transform.position, Spaceship.instance.pos + (Vector3.up * 5), Time.deltaTime * 2f);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 0.9F);

            if (transform.localScale.magnitude < 0.45f)
            {
                MaterialOffset.disActive = true;
                gameObject.SetActive(false);

                spaceship.StopEffect();
            }
            return;
        }
    }

    private void Move()
    {
        if(arriveNum == needNum)
        {
            //COL
            Collider[] cols = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider col in cols)
            {
                if (col.CompareTag("Player"))
                {
                    agent.enabled = false;
                    return;
                }
            }
        }


        if (arriveNum == needNum)
        {
            agent.enabled = true;

            Vector3 des = Spaceship.instance.pos;
            des.y = transform.position.y;
            agent.SetDestination(des);
            if (Vector3.Distance(transform.position, des) < 0.1f)
            {
                StopCarrying();
                agent.enabled = false;
                isArrive = true;
                transform.GetChild(0).gameObject.SetActive(false);

                spaceship.PlayEffect();
            }
        }
        else agent.enabled = false;
    }

    public Vector3 GetArrivePoint()
    {
        //float angle;
        //float PI2 = Mathf.PI * 2;

        //return new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

        Debug.Log(inNum);
        if (isFull()) return Vector3.zero;
        return transform.GetChild(2 + inNum).position;

        /*
         *         //GameObject[] colObj = new GameObject[needNum];
        //
        //float angle;
        //float PI2 = Mathf.PI * 2;
        //
        //for (int i = 0; i < needNum; i++)
        //{
        //    colObj[i] = Instantiate(colPrefab, transform);
        //    colObj[i].transform.position += Vector3.down * downValue;
        //
        //    angle = i * PI2 / needNum;
        //    colObj[i].transform.position += new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        //}
         */
    }

    public Vector3 GetArrivePoint(int num)
    {
        Debug.Log(inNum);
        if (isFull()) return Vector3.zero;
        Debug.Log(transform.GetChild(2 + num).name);
        Debug.Log(2 + num);
        return transform.GetChild(2 + num).position;
    }

    public bool isFull() { return inNum == needNum ? true : false; }

    public Transform Positioning(Old_Pikmin pikmin)
    {
        inNum++;
        textSetting.ChangeText(needNum.ToString() + "\n─\n" + inNum.ToString());
        pikminStack.Push(pikmin);
        return transform.GetChild(inNum + 1);
    }

    public void StopCarrying() //물체에 좌클릭이 충돌했을 때 실행
    {
        arriveNum = 0;
        while (inNum > 0)
        {
            inNum--;
            Old_Pikmin pk = pikminStack.Pop();
            pk.Init();
        }

        textSetting.ChangeText(needNum.ToString() + "\n─\n" + inNum.ToString());
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position+Vector3.down * downValue, Vector3.down, radius);

        Gizmos.color = gizmoColor;

        float PI2 = Mathf.PI * 2f;

        float angle;
        for(int i = 0; i < needNum; i++)
        {
            angle = i * PI2 / needNum;
            Gizmos.DrawWireSphere(transform.position + (Vector3.down * downValue) + new Vector3(Mathf.Cos(angle) * radius,
    0, Mathf.Sin(angle) * radius), gizmoSize);
        }
    }

    public void PushedOut(Vector3 direction)
    {
        //throw new System.NotImplementedException();
    }
}