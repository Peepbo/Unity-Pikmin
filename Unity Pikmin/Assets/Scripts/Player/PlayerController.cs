﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public  PlayerState       state;
    public  int               myPikminCount;
    public  GameObject        myHand;

    public  int               allNums;
    public  int               orderNums;

    private Animator          anim;
    private Vector3           throwPos;
    private Vector3           direction;

    private List<Pikmin>      pikmins = new List<Pikmin>();
    private CharacterController controller;

    public  ParticleSystem    leftDustParticle;
    public  ParticleSystem    rightDustParticle;

    private void Awake()
    {
        instance = this;
        anim = transform.GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        state = PlayerState.Idle;
        FootPos = transform.GetChild(2).transform;
        UserTransform = transform;

        var charAnim = transform.GetChild(0).GetComponent<AnimSetting>();

        charAnim.AddAct("Idle", () => state = PlayerState.Idle);
        charAnim.AddAct("Throw", ThrowPik);
        charAnim.AddAct("Catch", CatchPik);
        charAnim.AddAct("ThrowCheck", CheckThrow);
        charAnim.AddAct("EndThrow", () =>
        {
            anim.SetBool("RightClick", false);
            state = PlayerState.Idle;
        });
        charAnim.AddAct("LeftDust", () => leftDustParticle.Play());
        charAnim.AddAct("RightDust", () => rightDustParticle.Play());
    }

    private void Start()
    {
        var obj = GameObject.FindGameObjectsWithTag("Pikmin");

        foreach(var ob in obj)
        {
            AddPikmin(ob);
        }
    }

    public void AddPikmin(GameObject obj)
    {
        pikmins.Add(obj.GetComponent<Pikmin>());
        allNums++;
    }

    private void Update() => Animation();

    private void Animation()
    {
        switch (state)
        {
            case PlayerState.Idle:

                Move();
                CatchPikmin();
                LeftButton();
                RightButton();
                AutoThrow();
                Order();
                ChangeOrderNums();
                anim.SetFloat("MoveSpeed", 0);
                break;
            case PlayerState.Walk:

                Move();
                CatchPikmin();
                LeftButton();
                RightButton();
                Order();
                ChangeOrderNums();
                anim.SetFloat("MoveSpeed", 1);
                break;
            case PlayerState.ThrowAction:

                anim.SetBool("RightClick", true);
                anim.SetFloat("MoveSpeed", 0);
                break;
        }
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0,
                Input.GetAxisRaw("Vertical"));

        if(!Collision()) controller.Move(move * Time.deltaTime * 5f);

        if (!Utils.AlmostSame(move.magnitude, 0f))
        {
            state = PlayerState.Walk;
            direction = new Vector3(h, 0, v);
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else state = PlayerState.Idle;

        controller.Move(Vector3.down * 9.8f * Time.deltaTime);
    }

    private void ChangeOrderNums()
    {
        switch (MouseController.instance.GetWheel())
        {
            case MouseWheel.UP:
                if (orderNums + 1<= myPikminCount) orderNums++;
                break;
            case MouseWheel.DOWN:
                if (orderNums - 1 >= 0) orderNums--;
                break;
        }
    }

    private void LeftButton()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 _point = MouseController.instance.GetHit;
            float _radius = MouseController.instance.whistle.GetRadius;

            if (_point != Vector3.zero)
            {
                foreach (Pikmin pik in pikmins)
                {
                    Vector3 temp = pik.transform.position;
                    temp.y = 0;

                    if (Vector3.Distance(temp, _point) < _radius)
                    {
                        if (pik.state < (PikminState)3 && pik.PikminTarget != transform)
                        {
                            pik.Init();
                            pik.PikminTarget = transform;
                            myPikminCount++;
                        }
                    }
                }
            }
        }
    }

    private void RightButton()
    {
        if (state == PlayerState.Walk) return;

        if(Input.GetMouseButtonDown(1))
        {
            if (myHand.transform.childCount > 0)
            {
                Vector3 mouseHit = MouseController.instance.GetHit;
                mouseHit.y = transform.position.y;

                transform.LookAt(mouseHit);

                throwPos = MouseController.instance.GetHit;

                state = PlayerState.ThrowAction;
            }
        }
    }

    private void CatchPikmin()
    {
        if (Input.GetKeyDown(KeyCode.Space) && myPikminCount > 0 && myHand.transform.childCount == 0)
        {
            CatchPik();
        }
    }

    private void ThrowPik()
    {
        if (myHand.transform.childCount == 0) return;

        Pikmin pik = myHand.GetComponentInChildren<Pikmin>();

        //throwPow
        Collider[] _cols = Physics.OverlapSphere(throwPos, 2f);

        Removable _removable = null;
        foreach (Collider col in _cols)
        {
            if (col.CompareTag("Object"))
            {
                var _type = col.GetComponent<IObject>().objectType;

                if (_type == ObjectType.MOVEABLE_OBJ)
                {
                    _removable = col.GetComponent<Removable>();
                    throwPos = _removable.ThrowPos();
                    break;
                }
            }
        }

        pik.FlyPikmin(myHand.transform.position, throwPos, _removable);
    }

    private void AutoThrow()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if (orderNums <= 0) return;

            Vector3 mouseHit = MouseController.instance.GetHit;
            mouseHit.y = transform.position.y;
            transform.LookAt(mouseHit);

            throwPos = MouseController.instance.GetHit;
            anim.SetTrigger("FastThrow");
        }
    }

    private void CatchPik()
    {
        Pikmin choose = null;
        float dis = 0.0f;

        foreach (Pikmin pik in pikmins)
        {
            if (pik.state != PikminState.STAY) continue;
            if (pik.transform.parent != null) continue;
            if (pik.PikminTarget == null) continue;

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

        if (choose == null) return;
        choose.PickMe(myHand.transform);

        if (orderNums == myPikminCount) orderNums--;

        myPikminCount--;

    }

    private void CheckThrow()
    {
        if (orderNums == 0) return;
        if (state == PlayerState.Walk) return;

        anim.SetTrigger("FastThrow");
    }

    private bool Collision()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position + Vector3.down * 1f + direction, 0.3f);

        if (cols.Length == 0) return true;

        foreach (var col in cols)
        {
            if (col.CompareTag("Object")) return true;
        }

        return false;
    }

    private void Order()
    {
        if(Input.GetMouseButtonDown(2))
        {
            Transform _db = MouseController.instance.GetObjectHit();
            if (_db != null)
            {
                bool isOut = false;
                while(orderNums > 0 && !isOut)
                {
                    IObject _obj = _db.GetComponent<IObject>();

                    Pikmin choose = null;
                    float dis = 0.0f, cmp;

                    switch (_obj.objectType)
                    {
                        case ObjectType.MONSTER_OBJ:
                            EnemyManager enem = _db.GetComponent<EnemyManager>();

                            foreach (Pikmin pik in pikmins)
                            {
                                if (pik.state != PikminState.STAY) continue;
                                if (pik.transform.parent != null) continue;

                                cmp = (transform.position - pik.transform.position).magnitude;

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

                            if (choose == null) return;

                            orderNums--;
                            myPikminCount--;

                            enem.Expansion();
                            choose.enemy = enem;
                            choose.WorkPikmin();
                            break;
                        case ObjectType.MOVEABLE_OBJ:

                            Removable _removable = _db.GetComponent<Removable>();

                            foreach (Pikmin pik in pikmins)
                            {
                                if (pik.state != PikminState.STAY) continue;
                                if (pik.transform.parent != null) continue;

                                cmp = (transform.position - pik.transform.position).magnitude;

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

                            if (choose == null) return;

                            orderNums--;
                            myPikminCount--;

                            _removable.Expansion();
                            choose.removable = _removable;
                            choose.WorkPikmin();
                            break;
                        default:
                            isOut = true;
                            break;
                    }
                }
            }
        }
    }

    public Transform FootPos { get; private set; }
    public Transform UserTransform { get; private set; }
}