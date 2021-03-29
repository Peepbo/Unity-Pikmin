using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isDie;
    public int hp;
    public Vector3 cubeSize;
    public Vector3 center;
    public Transform factory;

    // Update is called once per frame
    void Update()
    {
        if(!isDie) Stick();
    }

    public void Stick()
    {
        Collider[] cols = Physics.OverlapBox(transform.position + center, cubeSize / 2);

        foreach (Collider col in cols)
        {
            if (col.transform.parent == null && col.CompareTag("Pikmin"))
            {
                if (col.GetComponent<Pikmin>().PikminTarget != null) continue;

                col.transform.position += (transform.position - col.transform.position).normalized * 0.25f;
                col.GetComponent<Pikmin>().AttackPikmin(this);
            }
        }
    }

    public void GetDamaged(int value)
    {
        if (isDie) return;

        hp -= value;
        if (hp <= 0)
        {
            isDie = true;

            while (factory.childCount > 0)
            {
                factory.GetChild(0).GetComponent<Pikmin>().Init();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + center, cubeSize);
    }
}
