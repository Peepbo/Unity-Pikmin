using UnityEngine;
using UnityEngine.AI;

public class test : Interaction
{
    public enum EnemyState { STAY,MOVE,ATTACK }

    private NavMeshAgent agent;
    private Vector3 goal;
    public float time = 0;
    public EnemyState state;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = EnemyState.STAY;
    }
    
    public override void Arrangement(Transform trans)
    {
        Debug.Log("Arrangement");
    }

    public override void Expansion()
    {
        Debug.Log("Expansion");
    }

    public override void FinishWork()
    {
        Debug.Log("FinishWork");
    }

    public override void Reduction()
    {
        Debug.Log("Reduction");
    }

    private Vector3 NewPath(bool isCol)
    {
        if(!isCol) return transform.position + -transform.forward * Random.Range(3, 6);

        Vector3 dir = Utils.RandomVector(-Vector3.one, Vector3.one).normalized * Random.Range(5, 8);
        dir.y = 0;

        return transform.position + dir;
    }

    private void Move()
    {
        Debug.DrawRay(transform.position + transform.forward, Vector3.down * 5f);
        if (!Physics.Raycast(transform.position + transform.forward, Vector3.down, 5f))
        {
            goal = NewPath(true);
            agent.SetDestination(goal);
        }

        //almost same
        else if (Vector3.Distance(transform.position, goal) < 0.25f)
        {
            state = EnemyState.STAY;
            agent.velocity = Vector3.zero;
        }
    }

    private void Stay()
    {
        time += Time.deltaTime;

        if (time > 2.5f)
        {
            time = 0;
            state = EnemyState.MOVE;

            //Debug.DrawRay(transform.position + transform.forward, Vector3.down * 5f);

            bool _isCol = Physics.Raycast(transform.position + transform.forward, Vector3.down, 5f);
            
            goal = NewPath(_isCol);
            agent.SetDestination(goal);
        }
    }

    private void Animation()
    {
        switch (state)
        {
            case EnemyState.STAY:
                Stay();
                break;
            case EnemyState.MOVE:
                Move();
                break;
            case EnemyState.ATTACK:
                break;
        }
    }

    private void Update() => Animation();
}