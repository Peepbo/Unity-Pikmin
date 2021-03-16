using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    private NavMeshAgent nav;
    public Transform target;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        target = null;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if(target != null)
        {
            nav.SetDestination(target.position);
        }
    }
}
