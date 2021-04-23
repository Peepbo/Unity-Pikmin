using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Vector3 dir;
    private void Update()
    {
        transform.Rotate(dir * Time.deltaTime);
    }
}