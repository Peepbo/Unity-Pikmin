using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    public Vector3 cubeSize;
    public Vector3 center;
    public Transform factory;

    //03/29 create custom overlap collider
    //List<GameObject> colliders;
    //colliderType box? sphere? 
    //size, position

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
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
                script.enemyScript = this;

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
        Gizmos.DrawWireCube(transform.position + center, cubeSize);
    }
}
