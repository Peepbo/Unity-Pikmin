using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOffset : MonoBehaviour
{
    Material mat;
    Vector2 vec;

    Vector3 endSize = new Vector3(2.8f, 2.8f, 2.8f);

    public static bool disActive = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        mat = GetComponent<Renderer>().material;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(!disActive)
        {
            vec.y -= Time.deltaTime;
            mat.SetTextureOffset("_MainTex", vec);
            transform.localScale = Vector3.Lerp(transform.localScale, endSize, Time.deltaTime * 5f);
        }

        else
        {
            vec.y += Time.deltaTime;
            mat.SetTextureOffset("_MainTex", vec);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 2.5f);

            if (transform.localScale.magnitude < 1.0f)
            {
                disActive = false;
                gameObject.SetActive(false);
            }
        }
    }
}