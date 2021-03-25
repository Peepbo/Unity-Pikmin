using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookCamera : MonoBehaviour
{
    private Transform cam;
    private void Awake() => cam = Camera.main.transform;
    void Update() => transform.LookAt(cam);
}
