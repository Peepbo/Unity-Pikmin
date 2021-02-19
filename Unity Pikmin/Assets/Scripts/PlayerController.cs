using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        ThrowReady,
        ThrowAction
    }

    public PlayerState state;
    public List<Pikmin> pikmins = new List<Pikmin>();
    public List<RemovableObj> objects = new List<RemovableObj>();

    public int myPikminCount = 0;
    public GameObject myHand;

    private Animator anim;
    private Vector3 throwPos;

    Action idleAct, walkAct, throw0Act, throw1Act;
     
    private void Awake()
    {
        GetPos = transform;
        anim = transform.GetComponentInChildren<Animator>();
        state = PlayerState.Idle;
    }

    private void Start()
    {
        var obj = GameObject.FindGameObjectsWithTag("Pikmin");

        foreach(var ob in obj)
        {
            pikmins.Add(ob.GetComponent<Pikmin>());
        }

        obj = GameObject.FindGameObjectsWithTag("Object");
        foreach (var ob in obj)
        {
            objects.Add(ob.GetComponent<RemovableObj>());
        }

        SetAction();
    }

    private void SetAction()
    {
        idleAct = () =>
        {
            Move();
            CatchPikmin();
            LeftButton();
            RightButton();
            anim.SetFloat("MoveSpeed", 0);
        };

        walkAct = () =>
        {
            Move();
            CatchPikmin();
            LeftButton();
            RightButton();
            anim.SetFloat("MoveSpeed", 1);
        };

        throw0Act = () =>
        {
            anim.SetBool("RightClick", true);
            RightButton();
        };

        throw1Act = () =>
        {
            anim.SetBool("RightClick", false);
        };
    }

    // Update is called once per frame
    private void Update() => Animation();

    private void Animation()
    {
        switch (state)
        {
            case PlayerState.Idle:
                idleAct();
                break;
            case PlayerState.Walk:
                walkAct();
                break;
            case PlayerState.ThrowReady:
                throw0Act();
                break;
            case PlayerState.ThrowAction:
                throw1Act();
                break;
        }
    }

    #region Action
    private void Move()
    {
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
                        if (pik.state < (PikminState)3)
                        {
                            pik.ChangeTarget = transform;
                            pik.state = PikminState.FOLLOW;

                            myPikminCount++;
                        }
                        //else if(pik.state == )
                    }
                }

                foreach(RemovableObj obj in objects)
                {
                    if (Vector3.Distance(obj.transform.position, _point) < _radius)
                    {
                        if(obj.inNum > 0)
                        {
                            obj.StopCarrying();
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
                mouseHit.y = transform.position.y;

                transform.LookAt(mouseHit);

                state = PlayerState.ThrowReady;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (myHand.transform.childCount > 0)
            {
                throwPos = MouseController.GetHit;

                state = PlayerState.ThrowAction;
                myPikminCount--;
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
                choose.GetComponent<CapsuleCollider>().enabled = false;
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
    #endregion

    #region Animator
    public void ThrowPik()
    {
        Pikmin pik = myHand.GetComponentInChildren<Pikmin>();

        throwPos.y = 0;
        pik.FlyPikmin(throwPos);

        Vector3 Vo = Parabola.CalculateVelocity(throwPos, myHand.transform.position, 1.5f);
        pik.transform.rotation = Quaternion.identity;

        Rigidbody rigid = pik.GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.velocity = Vo;
    }

    public void ChangeState(PlayerState _state) { state = _state; }
    #endregion

    #region Static
    public static Transform GetPos { get; private set; }
    #endregion
}