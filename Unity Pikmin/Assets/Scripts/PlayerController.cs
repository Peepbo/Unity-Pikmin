using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum PlayerState
    {
        Idle,
        Walk,
        ThrowReady,
        ThrowAction
    }

    PlayerState state;

    public List<Pikmin> pikmins = new List<Pikmin>();
    public int myPikminCount = 0;
    public GameObject myHand;

    private Animator anim;
    private bool isWalk;
     
    private void Awake()
    {
        GetPos = transform;
        anim = transform.GetComponentInChildren<Animator>();
        state = PlayerState.Idle;
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
        Animation();

        Move();

        //mouse left btn
        LeftButton();

        //mouse right btn
        RightButton();

        //keyboard space btn
        CatchPikmin();
    }

    private void Animation()
    {
        switch (state)
        {
            case PlayerState.Idle:
                anim.SetFloat("MoveSpeed", 0);
                break;
            case PlayerState.Walk:
                anim.SetFloat("MoveSpeed", 1);
                break;
            case PlayerState.ThrowReady:
                anim.SetBool("RightClick", true);
                break;
            case PlayerState.ThrowAction:
                anim.SetBool("RightClick", false);
                state = PlayerState.Idle;
                break;
        }
    }

    private void Move()
    {
        if (state > (PlayerState)1) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 pos = transform.position;

        if (Mathf.Abs(h) + (Mathf.Abs(v)) > 0f)
        {
            state = PlayerState.Walk;

            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(new Vector3( h, 0, v)), Time.deltaTime*5f);
        }
        else state = PlayerState.Idle;

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
        if (Input.GetMouseButton(1))
        {
            if(myHand.transform.childCount > 0)
            {
                Vector3 mouseHit = MouseController.GetHit;
                mouseHit.y = 0;

                transform.rotation = Quaternion.Lerp(transform.rotation,
        Quaternion.LookRotation(mouseHit), Time.deltaTime * 5f);

                state = PlayerState.ThrowReady;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (myHand.transform.childCount > 0)
            {
                Pikmin pik = myHand.GetComponentInChildren<Pikmin>();

                Vector3 mouseHit = MouseController.GetHit;

                pik.FlyPikmin(mouseHit);

                Vector3 Vo = Parabola.CalculateVelocity(mouseHit, myHand.transform.position, 1.5f);
                pik.transform.rotation = Quaternion.identity;

                Rigidbody rigid = pik.GetComponent<Rigidbody>();
                rigid.isKinematic = false;
                rigid.velocity = Vo;

                state = PlayerState.ThrowAction;
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
                choose.transform.rotation = myHand.transform.rotation;

                choose.state = PikminState.ATTACK;
                choose.PickMe();
                //choose.
                myPikminCount--;
            }
        }
    }

    public static Transform GetPos { get; private set; }
}