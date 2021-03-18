using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleScript : MonoBehaviour
{
    public Vector3 speed;
    public Vector3 endSize;
    public float rSpeed = 4f;
    
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, endSize, Time.deltaTime * rSpeed);
    }
}
