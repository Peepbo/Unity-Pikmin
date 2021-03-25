using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    //void Update() => Debug.LogError("delete plz");
    public Vector3 cubeSize;
    public Vector3 center;

    private void Update()
    {
        Collider[] cols = Physics.OverlapBox(transform.position + center, cubeSize);

        foreach (Collider col in cols)
        {
            if (col.CompareTag("Pikmin"))
            {
                var script = col.GetComponent<Pikmin>();

                script.state = PikminState.ATTACK;
                script.StopAllCoroutines();

                var rigid = col.GetComponent<Rigidbody>();
                rigid.useGravity = false;
                rigid.velocity = Vector3.zero;
                script.transform.LookAt(transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawCube(transform.position, Vector3.one * 10f);

        Gizmos.DrawWireCube(transform.position + center, cubeSize);
    }
}
