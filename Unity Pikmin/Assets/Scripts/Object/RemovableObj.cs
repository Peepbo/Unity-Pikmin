﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

public class RemovableObj : MonoBehaviour
{
    private bool isArrive;

    public GameObject colPrefab;

    [Header("SETTINGS")]
    public Transform target;
    public float radius;
    public float radius2;
    public float downValue;
    public int needNum;

    [Header("INFO")]
    public int inNum;
    public int arriveNum;

    [Header("GIZMO")]
    public Color gizmoColor;
    public float gizmoSize;

    private NavMeshAgent agent;
    private TextSetting textSetting;
    private Stack<Pikmin> pikminStack = new Stack<Pikmin>();
    private Material mat;
    private Color emissionColor = new Color(0.6F, 0, 0, 3F);
    private Spaceship spaceship;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        textSetting = transform.GetChild(0).GetComponent<TextSetting>();
        spaceship = target.GetComponent<Spaceship>();

        mat.EnableKeyword("_EMISSION");
    }

    private void Start()
    {
        GameObject[] colObj = new GameObject[needNum];

        float angle;
        float PI2 = Mathf.PI * 2;

        for (int i = 0; i < needNum; i++)
        {
            colObj[i] = Instantiate(colPrefab, transform);
            colObj[i].transform.position += Vector3.down * downValue;

            angle = i * PI2 / needNum;
            colObj[i].transform.position += new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    private void Update()
    {
        Arrive();

        Move();
    }

    public void Arrive()
    {
        if (isArrive)
        {
            mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), emissionColor, Time.deltaTime * 7f));

            transform.position = Vector3.Lerp(transform.position, Spaceship.pos + (Vector3.up * 5), Time.deltaTime * 2f);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 0.9F);

            if (transform.localScale.magnitude < 0.45f)
            {
                MaterialOffset.disActive = true;
                spaceship.turnOff();
                gameObject.SetActive(false);
            }
            return;
        }
    }

    public void Move()
    {
        if (isFull())
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
                transform.GetChild(0).gameObject.SetActive(false);

                spaceship.turnOn();
                spaceship.Smoke();
            }
        }
        else agent.enabled = false;
    }

    public bool isFull() { return inNum == needNum ? true : false; }

    public Transform Positioning(Pikmin pikmin)
    {
        inNum++;
        textSetting.ChangeText(needNum.ToString() + "\n─\n" + inNum.ToString());
        pikminStack.Push(pikmin);
        return transform.GetChild(inNum);
    }

    public void StopCarrying() //물체에 좌클릭이 충돌했을 때 실행
    {
        arriveNum = 0;
        while (inNum > 0)
        {
            inNum--;
            Pikmin pk = pikminStack.Pop();
            pk.Init();
        }

        textSetting.ChangeText(needNum.ToString() + "─" + inNum.ToString());
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

        Gizmos.DrawSphere(transform.position, radius2);
    }
}
