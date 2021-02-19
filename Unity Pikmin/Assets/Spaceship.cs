using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public static Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }
}
