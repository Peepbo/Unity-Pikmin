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
                factory.GetChild(0).GetComponent<Pikmin>().Init();
            }

            return;
        }

        Collider[] cols = Physics.OverlapBox(transform.position + center, cubeSize / 2);

        foreach (Collider col in cols)
        {
            if (col.transform.parent == null && col.CompareTag("Pikmin"))
            {
                if (col.GetComponent<Pikmin>().PikminTarget != null) continue;

                col.transform.position += (transform.position - col.transform.position).normalized * 0.25f;

                var script = col.GetComponent<Pikmin>();
                script.transform.parent = factory;
                script.StopAllCoroutines();
                script.testScript = this;

                var rigid = col.GetComponent<Rigidbody>();
                rigid.isKinematic = true;
                rigid.velocity = Vector3.zero;
                script.transform.LookAt(transform);
                script.state = PikminState.ATTACK;
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
