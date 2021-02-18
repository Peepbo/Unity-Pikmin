using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RemovableObj : MonoBehaviour
{
    [Header("SETTINGS")]
    public float radius;
    public int needNum;

    [Header("INFO")]
    public int inNum;

    [Header("GIZMO")]
    public Color gizmoColor;
    public float gizmoSize;

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position+Vector3.down, Vector3.down, radius);

        Gizmos.color = gizmoColor;

        float PI2 = Mathf.PI * 2f;

        float angle;
        for(int i = 0; i < needNum; i++)
        {
            angle = i * PI2 / needNum;
            Gizmos.DrawWireSphere(transform.position + Vector3.down + new Vector3(Mathf.Cos(angle) * radius,
    0, Mathf.Sin(angle) * radius), gizmoSize);
        }
    }
}
