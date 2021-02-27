using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject prefabs;

    public Transform end;
    // Start is called before the first frame update
    void Start()
    {
        //5개
        //vector.up * 10f 3번째
        //vector.up * 5f 1번째
        //vector.up * 0f 0번째
        //vector.up * -5; 2번째
        //vector.up * -10; 4번째// -vetor.up = vector.down

        //<-  10
        //<-  5
        //<-  0
        //<- -5
        //<- -10

        float[] value = { 0, 5f, -5f, 10f, -10f };

        int num = 3;

        Vector3 angle = end.position - transform.position;
        Quaternion quater = Quaternion.LookRotation(angle);

        //transform.po

        for (int i = 0; i < num; i++)
        {
            var obj = Instantiate(prefabs, transform.position, quater);
            obj.transform.Rotate(Vector3.up * value[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(Vector3.up * 5f);
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            transform.Rotate(Vector3.down * 5f);
        }
    }
}
