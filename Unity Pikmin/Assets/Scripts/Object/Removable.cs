using UnityEngine;
using UnityEditor;

public partial class Removable : MonoBehaviour,IInteractionObject
{
    [Header("Gizmo Settings")]
    public Color gColor;
    public float gSize;
    public float gYpos;
    private float gAngle;

    [Header("Removable Settings")]
    public bool useObjectpool;
    public int seedNum;
    public int needs;
    private int works;
    private float angle;
    public Transform factory, location;

    [Header("Object Settings")]
    public float objSize;

    private void OnEnable()
    {
        infoSize = objSize;
        objectType = ObjectType.MOVEABLE_OBJ;

        AgentAwake();
        TextAwake();
    }

    private void Update()
    {
        AgentUpdate();
        ColorUpdate();
    }

    #region Interaction
    // Pikmin을 factory에 넣음
    public void Arrangement()
    {
        Relocation();
    }

    // 공간을 확장하고, location 재 지정
    public void Expansion()
    {
        works++;

        SetText();

        GameObject _colObj = ObjectPool.instance.BorrowObject(ObjectPoolType.COLLIDER);
        _colObj.transform.parent = location;

        FixLocation();
    }

    // Pikmin을 Object에서 해제함
    public void FinishWork()
    {
        // 공간 축소
        Reduction();
    }

    // 공간을 축소하고, location 재 지정함
    public void Reduction()
    {
        works--;

        SetText();

        FixLocation();
        Relocation();
        ObjectPool.instance.ReturnObject(location.GetChild(location.childCount - 1).gameObject);
    }

    public float infoSize { get; set; }
    public ObjectType objectType { get; set; }
    #endregion

    // Pikmin이 날아갈 위치를 반환함
    public Vector3 ThrowPos()
    {
        // 공간 확장
        Expansion();

        return location.GetChild(location.childCount - 1).position;
    }

    // 각각의 Pikmin 위치를 해당 location 위치에 지정 (Pikmin[i] -> location[i])
    public void Relocation()
    {
        Pikmin _child = null;

        for (int i = 0; i < factory.childCount; i++)
        {
            _child = factory.GetChild(i).GetComponent<Pikmin>();
            _child.PikminTarget = location.GetChild(i);
        }
    }

    // Location 위치를 works의 개수에 맞춰 재지정함
    private void FixLocation()
    {
        for (int i = 0; i < works; i++)
        {
            angle = i * Utils.PI2 / works;

            location.GetChild(i).position = transform.position + Vector3.down * gYpos;
            location.GetChild(i).position += new Vector3(Mathf.Cos(angle) * gSize, 0, Mathf.Sin(angle) * gSize);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = gColor;
        Handles.DrawWireDisc(transform.position + Vector3.down * gYpos, Vector3.down, gSize);

        for (int i = 0; i < works; i++)
        {
            gAngle = i * Utils.PI2 / works;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * gYpos + new Vector3(Mathf.Cos(gAngle) * gSize,
                0, Mathf.Sin(gAngle) * gSize), 0.2f);
        }
    }
}