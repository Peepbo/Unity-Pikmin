using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject circle;
    float circleRadius = 0;

    public List<Pikmin> pikmins = new List<Pikmin>();
    public int myPikminCount = 0;

    private void Start()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Pikmin");
        int len = obj.Length;
        for(int i = 0; i < len; i++)
        {
            pikmins.Add(obj[i].GetComponent<Pikmin>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        //
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 _point = hit.point;

                CircleActive(_point);

                foreach (Pikmin pik in pikmins)
                {
                    if(Vector3.Distance(pik.transform.position, _point) < circleRadius)
                    {
                        if (pik.state != PikminState.FOLLOW)
                        {
                            pik.state = PikminState.FOLLOW;
                            myPikminCount++;
                        }
                    }
                }
            }
        }
        else
        {
            CircleDeActive();
        }
    }

    void CircleActive(Vector3 pos)
    {
        circle.transform.position = pos;

        if (circleRadius > 5.4f) return;
        circle.SetActive(true);
        circleRadius = Mathf.Lerp(circleRadius, 5.5f, Time.deltaTime * 3f);
        float diameter = circleRadius * 2;
        circle.transform.localScale = Vector3.one * diameter;
    }

    void CircleDeActive()
    {
        if(!circle.activeSelf)return;

        circle.transform.localScale = Vector3.zero;
        circleRadius = 0;
        circle.SetActive(false);
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;

        pos.x += h * Time.deltaTime * 5f;
        pos.z += v * Time.deltaTime * 5f;

        transform.position = pos;
    }
}
