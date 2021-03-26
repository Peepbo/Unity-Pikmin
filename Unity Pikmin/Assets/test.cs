using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    //void Update() => Debug.LogError("delete plz");
    public int     hp;
    public Vector3 cubeSize;
    public Vector3 center;
    public Transform factory;
    private void Update()
    {
        if(hp <= 0)
        {
            while (factory.childCount > 0)
            {
                var pikmin = factory.GetChild(0).GetComponent<Pikmin>();
                pikmin.state = PikminState.STAY;
                pikmin.testScript = null;
                pikmin.transform.rotation = Quaternion.identity;
                pikmin.Init();
            }

            return;
        }

        Collider[] cols = Physics.OverlapBox(transform.position + center, cubeSize);

        foreach (Collider col in cols)
        {
            if (col.CompareTag("Pikmin"))
            {
                var script = col.GetComponent<Pikmin>();
                script.PikminTarget = null;
                script.transform.parent = factory;
                script.state = PikminState.ATTACK;
                script.StopAllCoroutines();
                script.testScript = this;

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
