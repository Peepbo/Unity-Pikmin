using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    //public
    public GameObject cursor3D;
    
    public GameObject cylinder;
    public GameObject upParticle;
    public GameObject dwParticle;

    private Vector3 cylinderEndScale = new Vector3(7f, 2f, 7f);
    private Vector3 particleEndScale = new Vector3(1.43f, 1.43f, 1.43f);

    public LineRenderer lineVisual; 

    //private
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
        //circleUp.transform.position = circleDown.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        cursor3D.transform.position = GetHit;

        if (Input.GetMouseButton(0))
            CircleActive();
        else
            CircleDeActive();

        lineVisual.SetPosition(0, PlayerController.UserTransform.position);
        lineVisual.SetPosition(1, GetHit);

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

            int layerMask = 1 << LayerMask.NameToLayer("ground") | 1 << LayerMask.NameToLayer("object");

            if (Physics.Raycast(ray, out hit, 100f, layerMask)) 
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

    #region Circle
    void CircleActive()
    {
        cylinder.transform.position = GetHit;

        if (cylinder.transform.localScale.y > 1.99f) return;

        if(!cylinder.activeSelf)
        {
            cylinder.SetActive(true);
            upParticle.SetActive(true);
            dwParticle.SetActive(true);
        }

        cylinder.transform.localScale = Vector3.Lerp(cylinder.transform.localScale, cylinderEndScale, Time.deltaTime * 5f);
        upParticle.transform.localScale = Vector3.Lerp(upParticle.transform.localScale, particleEndScale, Time.deltaTime * 5f);
        dwParticle.transform.localScale = Vector3.Lerp(upParticle.transform.localScale, particleEndScale, Time.deltaTime * 5f);
    }

    void CircleDeActive()
    {
        if (!cylinder.activeSelf) return;

        cylinder.transform.localScale = Vector3.Lerp(cylinder.transform.localScale, Vector3.zero, Time.deltaTime * 8f);
        upParticle.transform.localScale = Vector3.Lerp(upParticle.transform.localScale, Vector3.zero, Time.deltaTime * 8f);
        dwParticle.transform.localScale = Vector3.Lerp(upParticle.transform.localScale, Vector3.zero, Time.deltaTime * 8f);

        if (cylinder.transform.localScale.y < 0.1f)
        {
            cylinder.SetActive(false);
            upParticle.SetActive(false);
            dwParticle.SetActive(false);
        }
    }
    #endregion
}