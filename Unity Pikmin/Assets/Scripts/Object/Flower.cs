using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour, IObject, IFoat
{
    public bool isActive = false;
    public int prefabIndex;
    public float force;
    public float iSize;
    private Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        infoSize = iSize;
        objectType = ObjectType.TOUCH_OBJ;
    }

    void FixedUpdate() => Fall();

    #region IFoatingObject
    public void Fall()
    {
        if (isActive)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 1f))
            {
                if (_hit.transform.CompareTag("Floor"))
                {
                    rigid.isKinematic = true;
                    var obj = ObjectPool.instance.BorrowObject("Object", prefabIndex);
                    obj.transform.position = transform.position;
                    obj.transform.parent = null;

                    obj.GetComponent<EnsnarePikmin>().Ensnare();

                    gameObject.SetActive(false);
                }
            }

            return;
        }

        Collider[] cols = Physics.OverlapSphere(transform.position + transform.up * 0.1f, 1f);

        foreach (Collider col in cols)
        {
            if (col.CompareTag("Pikmin"))
            {
                rigid.isKinematic = false;
                isActive = true;
                rigid.AddForce(transform.up * force, ForceMode.Impulse);
                rigid.useGravity = true;

                var _pikmin = col.GetComponent<Pikmin>();
                _pikmin.StopAllCoroutines();
                var _pikminRigid = col.GetComponent<Rigidbody>();

                _pikmin.transform.rotation = Quaternion.identity;
                _pikminRigid.velocity = Vector3.zero;
                _pikminRigid.AddForce(transform.up * force * 1.5f, ForceMode.Impulse);
                break;
            }
        }
    }
    #endregion

    #region IObject
    public float infoSize { get; set; }
    public ObjectType objectType { get; set; }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up * 0.1f, 1f);
    }
}
