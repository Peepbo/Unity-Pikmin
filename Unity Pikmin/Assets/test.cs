using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        objectType = ObjectType.TOUCH_OBJ;
    }
    void FixedUpdate()
    {
        if (isActive)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 1f))
            {
                if(_hit.transform.CompareTag("Floor"))
                {
                    rigid.isKinematic = true;
                    objectType = ObjectType.MOVEABLE_OBJ;

                    var removable = gameObject.AddComponent<Removable>();
                    var navMesh = gameObject.AddComponent<NavMeshAgent>();

                    removable.Init(0.63f, 1.33f, 1);
                    //다른 방식으로 해결합시다.. (using prefabs..)
                }
            }

            return;
        }

        Collider[] cols = Physics.OverlapSphere(transform.position + transform.up * 0.1f, .75f);

        foreach(Collider col in cols)
        {
            if(col.CompareTag("Pikmin"))
            {
                isActive = true;
                rigid.AddForce(transform.up * force, ForceMode.Impulse);
                rigid.useGravity = true;

                var _pikmin = col.GetComponent<Pikmin>();
                _pikmin.StopAllCoroutines();
                var _pikminRigid = col.GetComponent<Rigidbody>();

                _pikminRigid.velocity = Vector3.zero;
                _pikminRigid.AddForce(transform.up * force * 1.5f, ForceMode.Impulse);
                break;
            }
        }
    }

    public float infoSize { get; set; }
    public ObjectType objectType { get; set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up * 0.1f, .75f);
    }
}
