using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardNormal : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            transform.forward = hit.normal;
        }
    }
}
