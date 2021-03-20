using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    //public
    public GameObject cursor3D;
    public GameObject circle;

    public LayerMask layer;
    public LineRenderer lineVisual; 

    //private
    private int lineSegment = 10;
    public Transform handPos;

    //static
    private static Camera cam;
    private static RaycastHit hit;
    private static Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        cursor3D.SetActive(true);
        cam = Camera.main;
        GetRadius = 0;

        lineVisual.positionCount = lineSegment;
    }

    // Update is called once per frame
    void Update()
    {
        cursor3D.transform.position = GetHit;

        if (Input.GetMouseButton(0))
            CircleActive();
        else
            CircleDeActive();

        if (Input.GetMouseButton(1))
        {
            lineVisual.enabled = true;
            Vector3 Vo = Parabola.CalculateVelocity(GetHit, handPos.position, 1.5f);
            Visualize(handPos.position, Vo);
        }
        else lineVisual.enabled = false;

        if(GetRemovableHit != null)
        {
            if(Input.mouseScrollDelta.y > 0)
                GetRemovableHit.TextNum++;

            if (Input.mouseScrollDelta.y < 0)
                GetRemovableHit.TextNum--;
        }
    }

    public static Vector3 GetHit
    {
        get
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
                return hit.point;

            return Vector3.zero;
        }
    }

    public static Removable GetRemovableHit
    {
        get
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.CompareTag("Object")) 
                    return hit.transform.GetComponent<Removable>();
            }
            return null;
        }
    }

    public static float GetRadius { get; private set; }

    void Visualize(Vector3 origin ,Vector3 vo)
    {
        for(int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = Parabola.CalculatePosInTime(origin, vo, i / (float)lineSegment);
            lineVisual.SetPosition(i, pos);
        }
    }

    #region Circle
    void CircleActive()
    {
        circle.transform.position = GetHit;

        if (GetRadius > 5.4f) return;

        circle.SetActive(true);
        GetRadius = Mathf.Lerp(GetRadius, 5.5f, Time.deltaTime * 3f);
        float diameter = GetRadius * 2;
        circle.transform.localScale = Vector3.one * diameter;
    }

    void CircleDeActive()
    {
        if (!circle.activeSelf) return;

        circle.transform.localScale = Vector3.zero;
        GetRadius = 0;
        circle.SetActive(false);
    }
    #endregion
}