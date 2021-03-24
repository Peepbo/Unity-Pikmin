using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    void Update() => transform.Rotate(Vector3.forward * 0.75f);
}
