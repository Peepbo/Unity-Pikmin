using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour, IObject
{
    public bool isActive = false;
    public float force;
    Rigidbody rigid;
    public float iSize;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        infoSize = iSize;
        objetType = ObjectType.TOUCH_OBJ;
    }
    void Update()
    {
        if (isActive) return;

        Collider[] cols = Physics.OverlapSphere(transform.position + transform.up * 0.1f, .75f);

        foreach(Collider col in cols)
        {
            if(col.CompareTag("Pikmin"))
            {
                isActive = true;
                rigid.AddForce(transform.up * force, ForceMode.Impulse);
                rigid.useGravity = true;

                var _pikminRigid = col.GetComponent<Rigidbody>();

                break;
            }
        }
    }

    public float infoSize { get; set; }
    public ObjectType objetType { get; set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up * 0.1f, .75f);
    }
}
