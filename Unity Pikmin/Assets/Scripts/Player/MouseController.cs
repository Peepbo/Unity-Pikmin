using UnityEngine;

public class MouseController : MonoBehaviour
{
    //Singleton Class
    public static MouseController instance;
    
    [Header("Cursor")]
    public Transform     cursor3D;
    
    [Header("Whistle")]
    public GameObject    cylinder;
    public GameObject    topParticle;
    public GameObject    bottomParticle;
    public Transform     radiusPivot;

    private Vector3      cylinderEndScale = new Vector3(7f, 2f, 7f);
    private Vector3      particleEndScale = new Vector3(1.43f, 1.43f, 1.43f);
    private LineRenderer lineVisual;

    [Header("Player Hand")]
    public Transform     handPos;

    private Camera       cam;
    private RaycastHit   hit;
    private Ray          ray;
    private int          layerMask;

    private void Awake() => lineVisual = GetComponent<LineRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        cursor3D.gameObject.SetActive(true);
        cam = Camera.main;
        GetRadius = 0;
    }

    // Update is called once per frame
    void Update()
    {
        cursor3D.position = GetHit;

        if (Input.GetMouseButton(0))
        {
            PlayWhistle();
        }
        else
        {
            StopWhistle();
        }

        if (cylinder.activeSelf)
        {
            GetRadius = (cursor3D.position - radiusPivot.position).magnitude;
            cylinder.transform.position = GetHit;
        }
        else
        {
            GetRadius = 0;
        }

        lineVisual.SetPosition(0, PlayerController.UserTransform.position);
        lineVisual.SetPosition(1, GetHit);
    }

    #region Hit
    //마우스가 가리키고 있는 곳을 반환한다.
    public Vector3 GetHit
    {
        get
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);

            layerMask = 1 << LayerMask.NameToLayer("ground") | 1 << LayerMask.NameToLayer("object");

            if (Physics.Raycast(ray, out hit, 100f, layerMask)) 
                return hit.point;

            return Vector3.zero;
        }
    }

    //마우스가 가리키고 있는 곳 중 (옮길 수 있는)물체의 스크립트를 반환한다.
    public Removable GetRemovableHit
    {
        get
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);

            layerMask = 1 << LayerMask.NameToLayer("object");

            if(Physics.Raycast(ray, out hit, 100f, layerMask))
                return hit.transform.GetComponent<Removable>();

            return null;
        }
    }
    #endregion

    public float GetRadius { get; private set; }

    #region Whistle
    void PlayWhistle()
    {
        if (cylinder.transform.localScale.y > 1.99f) return;

        if(!cylinder.activeSelf)
        {
            cylinder.SetActive(true);
            topParticle.SetActive(true);
            bottomParticle.SetActive(true);
        }

        cylinder.transform.localScale = Vector3.Lerp(cylinder.transform.localScale, cylinderEndScale, Time.deltaTime * 5f);

        topParticle.transform.localScale = bottomParticle.transform.localScale =
            Vector3.Lerp(topParticle.transform.localScale, particleEndScale, Time.deltaTime * 5f);
    }

    void StopWhistle()
    {
        if (!cylinder.activeSelf) return;

        cylinder.transform.localScale = Vector3.Lerp(cylinder.transform.localScale, Vector3.zero, Time.deltaTime * 8f);

        topParticle.transform.localScale = bottomParticle.transform.localScale =
            Vector3.Lerp(topParticle.transform.localScale, Vector3.zero, Time.deltaTime * 8f);

        if (cylinder.transform.localScale.y < 0.1f)
        {
            cylinder.SetActive(false);
            topParticle.SetActive(false);
            bottomParticle.SetActive(false);
        }
    }
    #endregion
}