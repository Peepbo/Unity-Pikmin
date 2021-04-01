using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 dir;
    public float force;
    void Update() => transform.Rotate(dir * force);
}
