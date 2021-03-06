﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [Header("SeedSettings")]
    public  RotateObject    rObj;
    public  Animator        anim;
    private Rigidbody       rigid;
    private RaycastHit      hit;
    private SeedState       state;
    private float           force, speed;

    [Header("Particles")]
    public GameObject       particle1;
    public GameObject       particle2;

    private void Awake() => rigid = GetComponent<Rigidbody>();

    private void OnEnable()
    {
        force = 12f;
        speed = 3f;

        Vector3 randomForce;
        randomForce.x = Random.Range(0.05f, 0.125f) * Utils.RandomSign;
        randomForce.y = 1;
        randomForce.z = Random.Range(0.05f, 0.125f) * Utils.RandomSign;
        randomForce *= force;

        rigid.useGravity = true;
        rigid.isKinematic = false;
        rigid.AddForce(randomForce, ForceMode.Impulse);

        state = SeedState.UP;
    }

    public void Update()
    {
        switch (state)
        {
            case SeedState.UP:

                if (rigid.velocity.y > -1) return;

                rigid.useGravity = false;
                rigid.isKinematic = true;
                rigid.velocity = Vector3.zero;

                state = SeedState.DOWN;
                break;
            case SeedState.DOWN:

                rigid.transform.Translate(Vector3.down * speed * Time.deltaTime);

                //Debug.DrawRay(transform.position, Vector3.down * 0.5f);
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
                {
                    if (hit.transform.CompareTag("Floor"))
                    {
                        rObj.transform.rotation = Quaternion.identity;
                        rObj.enabled = false;
                        anim.enabled = true;

                        particle1.SetActive(true);
                        particle2.SetActive(true);

                        state = SeedState.END;
                    }
                }
                break;
            case SeedState.END:
                Collider[] cols = Physics.OverlapSphere(transform.position, 1f);

                foreach(Collider col in cols)
                {
                    if(col.CompareTag("Player"))
                    {
                        var obj = ObjectPool.instance.BorrowObject(ObjectPoolType.PIKMIN, 1);
                        obj.transform.parent = null;
                        obj.transform.position = transform.position; 
                        PlayerController.instance.AddPikmin(obj);
                        ObjectPool.instance.ReturnObject(gameObject);
                    }
                }
                break;
        }
    }
}