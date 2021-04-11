using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

class Bee : EnemyManager, IFoat
{
    // Common Info

    // (int)        Hp, Works
    // (bool)       bIsDie
    // (Transform)  factory, location

    // Gizmo Settings

    // (Color)      gColor
    // (float)      gSize, gYpos, gAngle

    // bee Valiable
    [Header("Bee Settings")]
    public EnemyState state;
    public int prefabIndex;
    public float force;
    public bool isActive;

    private Vector3 goal;
    private float time;

    private void Start()
    {
        //var animSettings = anim.GetComponent<AnimSetting>();


        infoSize = 1f;
        objectType = ObjectType.TOUCH_OBJ;
        state = EnemyState.STAY;
    }

    private void Update() => Animation();

    #region IInteractionObject .. interface
    public override void Arrangement()
    {
        Relocation();
    }

    public override void Expansion()
    {
        works++;

        GameObject _colObj = ObjectPool.instance.BorrowObject("Collider");
        _colObj.transform.parent = location;

        FixLocation();
        Relocation();
    }

    public override void FinishWork()
    {
        Reduction();
    }

    public override void Reduction()
    {
        works--;

        FixLocation();
        Relocation();

        ObjectPool.instance.ReturnObject(location.GetChild(location.childCount - 1).gameObject);
    }
    #endregion

    #region IObject .. interface
    public override float infoSize { get; set; }
    public override ObjectType objectType { get; set; }
    #endregion

    #region Gizmo
    public override void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, gSize);

        Handles.color = gColor;
        Handles.DrawWireDisc(transform.position + Vector3.down * gYpos, Vector3.down, gSize);

        for (int i = 0; i < works; i++)
        {
            gAngle = i * Utils.PI2 / works;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * gYpos + new Vector3(Mathf.Cos(gAngle) * gSize,
                0, Mathf.Sin(gAngle) * gSize), 0.2f);
        }
    }
    #endregion

    #region IFloat .. interface
    public void Fall()
    {
        if (isActive)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 1f))
            {
                if (_hit.transform.CompareTag("Floor"))
                {
                    rigid.isKinematic = true;
                    var obj = ObjectPool.instance.BorrowObject("Object", prefabIndex);
                    obj.transform.position = transform.position;
                    obj.transform.parent = null;

                    obj.GetComponent<EnsnarePikmin>().Ensnare();

                    gameObject.SetActive(false);
                }
            }

            return;
        }

        //Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, gSize);
        Collider[] cols = Physics.OverlapSphere(transform.position + Vector3.up * 0.5f, gSize);

        foreach (Collider col in cols)
        {
            if (col.CompareTag("Pikmin"))
            {
                isActive = true;
                rigid.AddForce(transform.up * force, ForceMode.Impulse);
                rigid.useGravity = true;

                var _pikmin = col.GetComponent<Pikmin>();
                _pikmin.StopAllCoroutines();
                var _pikminRigid = col.GetComponent<Rigidbody>();

                _pikmin.transform.rotation = Quaternion.identity;
                _pikminRigid.velocity = Vector3.zero;
                _pikminRigid.AddForce(transform.up * force * 1.5f, ForceMode.Impulse);

                state = EnemyState.FALLDOWN;
                objectType = ObjectType.MONSTER_OBJ;
                GetComponent<SphereCollider>().radius *= 1.25f;
                agent.enabled = false;
                break;
            }
        }
    }
    #endregion

    //Bee Function
    private void Relocation()
    {
        Pikmin _child = null;

        for(int i = 0; i < factory.childCount; i++)
        {
            _child = factory.GetChild(i).GetComponent<Pikmin>();
            _child.PikminTarget = location.GetChild(i);
        }
    }

    private void FixLocation()
    {
        for (int i = 0; i < works; i++)
        {
            float _angle = i * Utils.PI2 / works;

            location.GetChild(i).position = transform.position + Vector3.down * gYpos;
            location.GetChild(i).position += new Vector3(Mathf.Cos(_angle) * gSize, 0, Mathf.Sin(_angle) * gSize);
        }
    }

    private Vector3 NewPath(bool isCol)
    {
        if (!isCol) return transform.position + -transform.forward * Random.Range(3, 6);

        Vector3 dir = Utils.RandomVector(-Vector3.one, Vector3.one).normalized * Random.Range(5, 8);
        dir.y = 0;

        return transform.position + dir;
    }

    private void Stay()
    {
        time += Time.deltaTime;

        if (time > 2.5f)
        {
            time = 0;
            state = EnemyState.MOVE;

            bool _isCol = Physics.Raycast(transform.position + transform.forward, Vector3.down, 5f);

            goal = NewPath(_isCol);
            agent.SetDestination(goal);
        }
    }

    private void Move()
    {
        Debug.DrawRay(transform.position + transform.forward, Vector3.down * 5f);
        if (!Physics.Raycast(transform.position + transform.forward, Vector3.down, 5f))
        {
            goal = NewPath(true);
            agent.SetDestination(goal);
        }

        //almost same
        else if (Vector3.Distance(transform.position, goal) < 0.25f)
        {
            state = EnemyState.STAY;
            agent.velocity = Vector3.zero;
        }
    }

    private void FallDown()
    {
        if (isDie) return;

        if (hp <= 0)
        {
            isDie = true;
            var _obj = ObjectPool.instance.BorrowObject("Object", 1);
            _obj.transform.position = transform.position;
            _obj.transform.parent = null;

            Pikmin _pik = null;
            while (factory.childCount > 0)
            {
                _pik = factory.GetChild(0).GetComponent<Pikmin>();
                _pik.Init();
                _pik.PikminTarget = null;
            }

            gameObject.SetActive(false);
        }
    }

    private void Animation()
    {
        switch (state)
        {
            case EnemyState.STAY:
                anim.SetInteger("animation", 1);
                Stay();
                Fall();
                break;
            case EnemyState.MOVE:
                anim.SetInteger("animation", 2);
                Move();
                break;
            case EnemyState.FALLDOWN:
                anim.SetInteger("animation", 5);
                FallDown();
                break;
        }
    }
}