using UnityEngine;



public class MouseController : MonoBehaviour
{
    //Singleton Class
    public static MouseController instance;
    
    [Header("Cursor")]
    public Transform     cursor3D;

    [Header("Whistle")]
    public  Whistle      whistle;

    private Vector3      lastPosition;
    private LineRenderer lineVisual;

    [Header("Object Info")]
    public ObjectInfo    objectInfo;

    [Header("Player Hand")]
    public Transform     handPos;

    private Camera       cam;
    private RaycastHit   hit;
    private Ray          ray;
    private int          layerMask;

    private void Awake() => lineVisual = GetComponent<LineRenderer>();

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;

        cursor3D.gameObject.SetActive(true);
        cam = Camera.main;
        lastPosition = PlayerController.instance.FootPos.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        cursor3D.position = GetHit;

        whistle.Checker(Input.GetMouseButton(0));

        objectInfo.Checker(GetObjectHit());

        UpdateLine();
    }

    private void UpdateLine()
    {
        lineVisual.SetPosition(0, PlayerController.instance.UserTransform.position);
        lineVisual.SetPosition(1, GetHit);
    }

    //마우스가 가리키고 있는 곳을 반환한다.
    public Vector3 GetHit
    {
        get
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);

            layerMask = 1 << LayerMask.NameToLayer("ground") | 1 << LayerMask.NameToLayer("object");

            if (Physics.Raycast(ray, out hit, 100f, layerMask)) 
            {
                lastPosition = hit.point;
                return  hit.point;
            }

            return lastPosition;
        }
    }

    //마우스가 가리키고 있는 곳 중 (옮길 수 있는)물체의 스크립트를 반환한다.
    public Removable GetRemovableHit()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        layerMask = 1 << LayerMask.NameToLayer("object");

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
            return hit.transform.GetComponent<Removable>();

        return null;
    }

    public Transform GetObjectHit()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        layerMask = 1 << LayerMask.NameToLayer("object");

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
            return hit.transform;

        return null;
    }

    public MouseWheel GetWheel()
    {
        if (Input.mouseScrollDelta.y == 0) return MouseWheel.STAY;
        if (Input.mouseScrollDelta.y > 0) return MouseWheel.UP;
        
        return MouseWheel.DOWN;
    }
}