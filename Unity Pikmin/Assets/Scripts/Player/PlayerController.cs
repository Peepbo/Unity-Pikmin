using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, ICollider
{
    public enum PlayerState   {Idle,Walk,ThrowReady,ThrowAction}

    public PlayerState        state;
    public int                myPikminCount;
    public GameObject         myHand;

    private Animator          anim;
    private Vector3           throwPos;
    public Removable          removable;
    private Vector3           direction;

    private Action            idleAct, walkAct, throw0Act, throw1Act;
    public  List<Pikmin>      pikmins = new List<Pikmin>();
     
    private void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
        state = PlayerState.Idle;
        FootPos = transform.GetChild(2).transform;
        UserTransform = transform;
    }

    private void Start()
    {
        var obj = GameObject.FindGameObjectsWithTag("Pikmin");

        foreach(var ob in obj)
        {
            pikmins.Add(ob.GetComponent<Pikmin>());
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
            state = PlayerState.ThrowReady;
            RightButton();
            anim.SetBool("RightClick", true);
            anim.SetFloat("MoveSpeed", 0);
        };

        throw1Act = () =>
        {
            anim.SetBool("RightClick", false);
        };
    }

    // Update is called once per frame
    private void Update()
    {
        Animation();

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if (removable == null) return;
        }
    }

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

        direction = new Vector3(h, 0, v);
        if (Mathf.Abs(h) + Mathf.Abs(v) > 0f)
        {
            state = PlayerState.Walk;

            transform.rotation = Quaternion.LookRotation(direction);
        }
        else state = PlayerState.Idle;

        if (Collision()) return;
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
                        if(pik.state < (PikminState)2 && pik.PikminTarget != transform)
                        {
                            pik.Init();
                            pik.PikminTarget = transform;
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
        if (state == PlayerState.Walk) return;

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
                removable = MouseController.GetRemovableHit;

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
                if (pik.state == PikminState.FOLLOW || pik.state == PikminState.STAY)
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
                choose.PickMe(myHand.transform);
                myPikminCount--;
            }
        }
    }
    #endregion

    #region Animator
    public void ThrowPik()
    {
        if (myHand.transform.childCount == 0) return;

        Pikmin pik = myHand.GetComponentInChildren<Pikmin>();

        if(removable == null)
            pik.FlyPikmin(myHand.transform.position, throwPos);
        else
            pik.FlyPikmin(myHand.transform.position, removable.ThrowPos());
    }

    public void ChangeState(PlayerState _state) { state = _state; }
    #endregion

    //collider
    private bool Collision()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position + Vector3.down * 0.82f + direction * 0.3f, 0.3f);
        foreach(var col in cols)
        {
            if (col.CompareTag("Object")) return true;

            if(col.CompareTag("Pikmin"))
            {
                ICollider colObj = col.GetComponent<ICollider>();

                Vector3 a = transform.position;
                a.y = 0;
                Vector3 b = col.transform.position;
                b.y = 0;
                Vector3 v = a - b;
                colObj.PushedOut(-(v.normalized) * 5/v.magnitude);
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.82f + direction * 0.3f, 0.3f);
    }

    public void PushedOut(Vector3 direction)
    {
        transform.Translate(direction * Time.deltaTime);
    }

    //public static Transform GetPos { get; private set; }
    public static Transform FootPos { get; private set; }
    public static Transform UserTransform { get; private set; }
}