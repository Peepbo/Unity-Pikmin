using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public  Transform    sprite;
    private Transform    cameraTransform;
    private Vector3      endSize = new Vector3(0.5f, 0.5f, 0f);
    private float        objSize;

    private void Start() => cameraTransform = Camera.main.transform;

    private void LookCamera() => sprite.LookAt(cameraTransform);

    public void Checker(Transform obj)
    {
        LookCamera();

        if (obj != null)
        {
            Show(obj);

            if(objSize == 0) objSize = obj.GetComponent<IObject>().infoSize;
        }
        else
        {
            Hide();
        }
    }

    private void Show(Transform obj)
    {
        sprite.position = obj.position;
        sprite.localScale = Vector3.Lerp(sprite.localScale, endSize * objSize, Time.deltaTime * 5f);
    }

    private void Hide()
    {
        sprite.localScale = Vector3.Lerp(sprite.localScale, Vector3.zero, Time.deltaTime * 9f);
        objSize = 0;
    }
}