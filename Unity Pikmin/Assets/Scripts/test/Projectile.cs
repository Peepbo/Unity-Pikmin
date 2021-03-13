using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject ball;

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
                GameObject obj = Instantiate(ball, shootPoint.position, Quaternion.identity);

                Rigidbody rigid = obj.GetComponent<Rigidbody>();
                rigid.velocity = Vo;
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
        //크기와 방향을 구한다

        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;
        //크기, 방향에서 y가 0인 벡터 (높이에 관여 안하겠다는 뜻)

        //create a float the represent our distance
        float Sy = distance.y;
        //Sy는 크기, 방향 벡터에서 높이이다.
        float Sxz = distanceXZ.magnitude;
        //Sxz는 두 타겟의 순수 거리이다(y가 0일 때)

        float Vxz = Sxz / time;
        //Vxz는 거리 / 시간 으로 속력을 구한다.

        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;
        //y / t + 1/2 * g * time

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
