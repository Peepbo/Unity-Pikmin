//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestObj : MonoBehaviour, ICollider
//{
//    [Header("Setting")]
//    public float radius;
//    public Vector3 pos;
//    public int order;

//    public Vector3 direction;
//    public bool stop;

//    // Update is called once per frame
//    void Update()
//    {
//        PushedOut(direction);
//    }

//    public int GetOrder { get { return order; } }

//    public bool Collision()
//    {
//        Collider[] cols = Physics.OverlapSphere(transform.position + pos, radius);

//        foreach(Collider col in cols)
//        {
//            if (col.CompareTag("Object") && !col.name.Equals(this.name))
//            {
//                ICollider colObj = col.GetComponent<ICollider>();

//                if(order < colObj.GetOrder)
//                {
//                    return true;
//                }
//                else
//                {
//                    Vector3 a = transform.position;
//                    a.y = 0;
//                    Vector3 b = col.transform.position;
//                    b.y = 0;
//                    Vector3 v = a - b;
//                    colObj.PushedOut(-(v.normalized) * 5 / v.magnitude);
//                }
//            }
//        }

//        return false;
//    }

//    public void PushedOut(Vector3 direction)
//    {
//        if (direction == Vector3.zero) return;
//        if (Collision()) return;

//        if(!stop) transform.Translate(direction * Time.deltaTime);
//    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position + pos, radius);
//    }
//}
