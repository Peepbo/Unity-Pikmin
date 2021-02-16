using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject pikmin;

    public Rigidbody bulletRigid;
    public GameObject cursor;
    public Transform shootPoint;
    public LayerMask layer;
    public LineRenderer lineVisual;
    public int lineSegment = 10;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        lineVisual.positionCount = lineSegment;
    }

    // Update is called once per frame
    void Update()
    {
        LaunchProjectile();
    }

    void LaunchProjectile()
    {
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, 100f, layer))
        {
            cursor.SetActive(true);
            cursor.transform.position = hit.point + Vector3.up * 0.1f;

            Vector3 Vo = CalculateVelocity(hit.point, shootPoint.position, 1f);

            Visualize(Vo);

            transform.rotation = Quaternion.LookRotation(Vo);

            if (Input.GetMouseButtonDown(0))
            {
                GameObject obj = Instantiate(pikmin, shootPoint.position, Quaternion.identity);
                Pikmin pik = obj.GetComponent<Pikmin>();
                pik.FlyPikmin(hit.point);

                Rigidbody rigid;
                if (obj.TryGetComponent(out rigid))
                {
                    rigid.constraints = RigidbodyConstraints.FreezeAll;
                    rigid.velocity = Vo;
                }
                else
                {
                    rigid = obj.AddComponent<Rigidbody>();
                    rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    rigid.velocity = Vo;
                }
            }
        }
        else cursor.SetActive(false);
    }

    void Visualize(Vector3 vo)
    {
        for (int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = CalculatePosInTime(vo, i / (float)lineSegment);
            lineVisual.SetPosition(i, pos);
        }
    }
         
    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        //create a float the represent our distance
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        Vector3 Vxz = vo;
        Vxz.y = 0f;

        Vector3 result = shootPoint.position + vo * time;
        float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + shootPoint.position.y;

        result.y = sY;

        return result;
    }
}
