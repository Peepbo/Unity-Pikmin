using UnityEngine;

public class Enemy : Interaction, IObject
{
    public int works;

    private bool isDie;
    public int hp;
    public Vector3 cubeSize;
    public Vector3 center;
    public Transform factory;
    public Transform location;

    public Transform centerPt;
    public Transform A, B;

    [Header("Object Settings")]
    public float objSize;

    void Start()
    {
        infoSize = objSize;
        objectType = ObjectType.MONSTER_OBJ;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDie) Stick();
    }

    #region  Interaction
    // Pikmin을 factory에 넣음
    public override void Arrangement(Transform trans)
    {
        trans.parent = factory;

        Pikmin _child = factory.GetChild(factory.childCount - 1).GetComponent<Pikmin>();
        _child.PikminTarget = location.GetChild(factory.childCount - 1);
    }

    public override void Expansion()
    {
        works++;

        GameObject _colObj = ObjectPool.instance.BorrowObject("Collider");
        _colObj.transform.parent = location;

        _colObj.transform.position = Utils.RandomVector(A.position, B.position);
    }

    public override void FinishWork()
    {
        Reduction();
    }

    public override void Reduction()
    {
        works--;

        ObjectPool.instance.ReturnObject(location.GetChild(location.childCount - 1).gameObject);
    }
    #endregion

    public void Stick()
    {
        Collider[] cols = Physics.OverlapBox(transform.position + center, cubeSize / 2);

        foreach (Collider col in cols)
        {
            //Debug.Log(col.name);
            if (col.transform.parent == null && col.CompareTag("Pikmin"))
            {
                if (col.GetComponent<Pikmin>().PikminTarget != null) continue;

                col.transform.position += (transform.position - col.transform.position).normalized * 0.15f;
                col.GetComponent<Pikmin>().AttackPikmin(this);
            }
        }
    }

    public void GetDamaged(int value)
    {
        if (isDie) return;

        hp -= value;
        //textMesh.text = hp.ToString();
        if (hp <= 0)
        {
            isDie = true;

            while (factory.childCount > 0)
            {
                factory.GetChild(0).GetComponent<Pikmin>().Init();
            }
        }
    }

    public float infoSize { get; set; }
    public ObjectType objectType { get; set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centerPt.position + center, cubeSize);
    }
}
