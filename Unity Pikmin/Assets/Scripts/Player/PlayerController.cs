using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, ICollider
{
    public enum PlayerState   {Idle,Walk,ThrowReady,ThrowAction}

    public  PlayerState       state;
    public  int               myPikminCount;
    public  GameObject        myHand;

    private Animator          anim;
    private Vector3           throwPos;
    private Vector3           direction;

    private Action            idleAct, walkAct, throw0Act, throw1Act;
    public  List<Pikmin>      pikmins = new List<Pikmin>();
     
    void OnTriggerEnter()
    {

    }
    private void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
        state = PlayerState.Idle;
        FootPos = transform.GetChild(2).transform;
        UserTransform = transform;

        var charAnim = transform.GetChild(0).GetComponent<AnimSetting>();

        charAnim.AddAct("Idle", () => state = PlayerState.Idle);
        charAnim.AddAct("Throw", ThrowPik);
        charAnim.AddAct("Catch", CatchPik);
        charAnim.AddAct("ThrowCheck", CheckThrow);
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
            AutoThrow();
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

            if (Collision()) return;
            pos.x += h * Time.deltaTime * 5f;
            pos.z += v * Time.deltaTime * 5f;

            transform.position = pos;
        }
        else state = PlayerState.Idle;
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

        if (Input.GetMouseButton(1))
        {
            if(myHand.transform.childCount > 0)
            {
                Vector3 mouseHit = MouseController.instance.GetHit;
                mouseHit.y = transform.position.y;

                transform.LookAt(mouseHit);

                state = PlayerState.ThrowReady;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (myHand.transform.childCount > 0)
            {
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

    public void ThrowPik()
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

    public void ChangeState(PlayerState _state) { state = _state; }

    public void AutoThrow()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
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

        if (choose == null) return;
        choose.PickMe(myHand.transform);
        myPikminCount--;
    }

    private void CheckThrow()
    {
        if (myPikminCount == 0) return;
        if (state == PlayerState.Walk) return;

        anim.SetTrigger("FastThrow");
    }

    private bool Collision()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position + Vector3.down * 1f + direction, 0.3f);

        Debug.Log(cols.Length);

        if (cols.Length == 0) return true;

        foreach (var col in cols)
        {
            if (col.CompareTag("Object")) return true;

            if (col.CompareTag("Pikmin"))
            {
                ICollider colObj = col.GetComponent<ICollider>();

                Vector3 a = transform.position;
                a.y = 0;
                Vector3 b = col.transform.position;
                b.y = 0;
                Vector3 v = a - b;
                colObj.PushedOut(-(v.normalized) * 5 / v.magnitude);
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

    public static Transform FootPos { get; private set; }
    public static Transform UserTransform { get; private set; }
}