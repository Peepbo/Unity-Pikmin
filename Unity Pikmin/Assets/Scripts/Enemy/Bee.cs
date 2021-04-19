using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

class Bee : EnemyManager, IFoat
{
    [Header("Bee Settings")]
    public int prefabIndex;
    public float force;
    public bool isActive;

    protected override void Start()
    {
        base.Start();

        base.objectType = ObjectType.TOUCH_OBJ;
    }

    #region IFloat .. interface
    public void Fall()
    {
        if (isActive)
        {
            RaycastHit _hit;

            int layerMask = 1 << LayerMask.NameToLayer("ground");
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, .1f, layerMask))
            {
                Debug.LogError(_hit.transform.name);
                if (_hit.transform.CompareTag("Floor"))
                {
                    state = EnemyState.FALLDOWN;
                    rigid.isKinematic = true;
                }
            }

            return;
        }

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
                
                objectType = ObjectType.MONSTER_OBJ;
                GetComponent<SphereCollider>().radius *= 1.25f;
                agent.enabled = false;
                break;
            }
        }
    }
    #endregion

    private void CheckDie()
    {
        if (!isDie) return;

        var _obj = ObjectPool.instance.BorrowObject("Object", 2);
        _obj.transform.position = transform.position;

        var _model = _obj.transform.GetChild(0);
        _model.rotation = transform.rotation;
        _model.GetComponent<Animator>().Play("Idle_Die");

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

    protected override void Animation()
    {
        switch (base.state)
        {
            case EnemyState.STAY:
                anim.SetInteger("animation", 1);
                base.Stay();

                Fall();
                break;
            case EnemyState.MOVE:
                anim.SetInteger("animation", 2);
                if(!isActive) base.Move();

                Fall();
                break;
            case EnemyState.FALLDOWN:
                anim.SetInteger("animation", 5);

                CheckDie();
                break;
        }
    }
}