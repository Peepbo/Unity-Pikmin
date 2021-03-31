using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private Rigidbody rigid;
    public float force;
    public bool isDown;
    public bool isEnd;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    public void Update()
    {
        if (isEnd) return;

        if(rigid.useGravity)
        {
            if (rigid.velocity.y < 0)
            {
                isDown = true;
                rigid.useGravity = false;
                rigid.velocity = Vector3.zero;
            }
        }

        if(isDown)
        {
            rigid.transform.Translate(Vector3.down * 1.15f * Time.deltaTime);

            Debug.DrawRay(transform.position, Vector3.down * 0.5f);

            RaycastHit _hit;
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 0.5f))
            {
                if (_hit.transform.CompareTag("Floor")) isEnd = true;
            }
        }
    }
}
