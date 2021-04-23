using UnityEngine.AI;
using UnityEngine;

partial class Removable
{
    private NavMeshAgent  agent;
    private float         timer;
    private bool          isArrive;

    private void AgentAwake() => agent = GetComponent<NavMeshAgent>();

    private void AgentUpdate()
    {
        if (isArrive) Arrive();
        else Move();
    }

    // spaceship에 도착
    private void Arrive()
    {
        transform.position = Vector3.Lerp(transform.position, Spaceship.instance.endPos.position + (Vector3.up * 5), Time.deltaTime * 2f);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 0.9f);

        if (Vector3.Distance(transform.position, Spaceship.instance.endPos.position) > 5.0f)
        {
            MaterialOffset.disActive = true;

            Spaceship.instance.StopEffect();

            if (useObjectpool) 
                ObjectPool.instance.ReturnObject(gameObject);
            else 
                gameObject.SetActive(false);
        }
    }

    // spaceship으로 이동
    private void Move()
    {
        agent.speed = Mathf.Clamp(1.0f + (works - needs / needs), 1.0f, 1.5f);

        if (needs <= works)
        {
            if (timer < 3f) timer += Time.deltaTime;

            else
            {
                agent.enabled = true;
                agent.SetDestination(Spaceship.instance.endPos.position);
            }
        }
        else
        {
            agent.enabled = false;
            timer = 0;
        }

        if (Vector3.Distance(Spaceship.instance.endPos.position, transform.position) < 1.0f)
        {
            agent.enabled = false;
            isArrive = true;

            Pikmin _pik = null;
            while (factory.childCount > 0)
            {
                _pik = factory.GetChild(0).GetComponent<Pikmin>();
                _pik.Init();
                _pik.PikminTarget = null;
            }

            Spaceship.instance.PlayEffect();
            Spaceship.instance.ActiveDissemination(seedNum);
        }
    }
}