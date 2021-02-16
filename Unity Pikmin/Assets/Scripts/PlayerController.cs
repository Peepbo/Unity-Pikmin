using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public List<Pikmin> pikmins = new List<Pikmin>();
    public int myPikminCount = 0;
    public GameObject myHand;

    private void Awake()
    {
        GetPos = transform;
    }
    private void Start()
    {
        var obj = GameObject.FindGameObjectsWithTag("Pikmin");
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

        //mouse left btn
        LeftButton();

        //mouse right btn
        RightButton();

        //keyboard space btn
        CatchPikmin();
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

    private void LeftButton()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 _point = MouseController.GetHit;
            float _radius = MouseController.GetRadius;

            if (_point != Vector3.zero)
            {
                foreach (Pikmin pik in pikmins)
                {
                    if (Vector3.Distance(pik.transform.position, _point) < _radius)
                    {
                        if (pik.state == PikminState.STAY)
                        {
                            pik.state = PikminState.FOLLOW;
                            myPikminCount++;
                        }
                    }
                }
            }
        }
    }

    private void RightButton()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (myHand.transform.childCount > 0)
            {
                Pikmin pik = myHand.GetComponentInChildren<Pikmin>();

                Vector3 mouseHit = MouseController.GetHit;

                pik.FlyPikmin(mouseHit);

                Vector3 Vo = Parabola.CalculateVelocity(mouseHit, myHand.transform.position, 1.5f);

                Rigidbody rigid = pik.GetComponent<Rigidbody>();
                rigid.isKinematic = false;
                rigid.velocity = Vo;
            }
        }
    }

    private void CatchPikmin()
    {
        if (Input.GetKeyDown(KeyCode.Space) && myPikminCount > 0 && myHand.transform.childCount == 0)
        {
            Pikmin choose = null;
            float dis = 0.0f;

            foreach (Pikmin pik in pikmins)
            {
                if (pik.state == PikminState.FOLLOW)
                {
                    float cmp = (transform.position - pik.transform.position).magnitude;

                    if (choose == null)
                    {
                        choose = pik;
                        dis = cmp;
                    }

                    else if (dis > cmp)
                    {
                        choose = pik;
                        dis = cmp;
                    }

                }
            }

            if (choose != null)
            {
                choose.transform.position = myHand.transform.position;
                choose.transform.parent = myHand.transform;

                choose.state = PikminState.ATTACK;
                myPikminCount--;
            }
        }
    }

    public static Transform GetPos { get; private set; }
}