using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public  Transform    sprite;
    private Transform    cameraTransform;
    private Vector3      endSize = new Vector3(0.5f, 0.5f, 0f);

    private void Start() => cameraTransform = Camera.main.transform;

    private void LookCamera() => transform.LookAt(cameraTransform);

    public void Show(Vector3 ObjectPos)
    {
        LookCamera();

        sprite.position = ObjectPos;
        sprite.localScale = Vector3.Lerp(sprite.localScale, endSize, Time.deltaTime * 5f);
    }

    public void Hide()
    {
        LookCamera();

        sprite.localScale = Vector3.Lerp(sprite.localScale, Vector3.zero, Time.deltaTime * 9f);
    }
}