﻿using UnityEngine.AI;
using UnityEngine;

partial class Removable
{
    private NavMeshAgent  agent;
    private float         timer;
    private bool          isArrive;
    private Material      mat;

    private void AgentAwake()
    {
        agent = GetComponent<NavMeshAgent>();
        mat = GetComponent<Renderer>().material;
    }

    private void AgentUpdate()
    {
        if (isArrive) Arrive();
        else Move();
    }

    // spaceship에 도착
    private void Arrive()
    {
        mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), new Color(0.6F, 0, 0, 3F), Time.deltaTime * 7f));

        transform.position = Vector3.Lerp(transform.position, Spaceship.instance.pos + (Vector3.up * 5), Time.deltaTime * 2f);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 0.9f);

        if (transform.localScale.magnitude < 0.45f)
        {
            MaterialOffset.disActive = true;
            gameObject.SetActive(false);

            Spaceship.instance.StopEffect();
        }
    }

    // spaceship으로 이동
    private void Move()
    {
        if (needs <= works)
        {
            agent.enabled = true;
            if (timer < 3f) timer += Time.deltaTime;

            else agent.SetDestination(Spaceship.instance.pos);
        }
        else
        {
            agent.enabled = false;
            timer = 0;
        }

        if (Vector3.Distance(Spaceship.instance.pos, transform.position) < 1.0f)
        {
            agent.enabled = false;
            isArrive = true;
            while (factory.childCount > 0)
                factory.GetChild(0).GetComponent<Pikmin>().Init();

            Spaceship.instance.PlayEffect();
        }
    }
}