using UnityEngine.AI;
using UnityEngine;

partial class Removable
{
    private NavMeshAgent agent;
    private float timer = 0;
   
    private void AgentAwake() => agent = GetComponent<NavMeshAgent>();

    private void Update()
    {
        if (needs <= works)
        {
            agent.enabled = true;
            if (timer < 3f) timer += Time.deltaTime;

            else agent.SetDestination(Spaceship.pos);
        }
        else
        {
            agent.enabled = false;
            timer = 0;
        }
    }
}