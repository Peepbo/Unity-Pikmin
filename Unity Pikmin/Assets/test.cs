using UnityEngine;

public class test : MonoBehaviour
{
    [Range(0, 100)]
    public float range;
    public Transform start, end;

    private Vector3 startPos;
    private float distance;

    private void Start()
    {
        range = 100;

        startPos = start.position;
        distance = (start.position - end.position).magnitude;
    }

    private void Update()
    {
        start.position = startPos + Vector3.down * (distance * ((100 - range) / 100));
    }
}