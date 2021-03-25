using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public  Transform    sprite;
    private Transform    cameraTransform;
    private Vector3      endSize = new Vector3(0.5f, 0.5f, 0f);

    private void Start() => cameraTransform = Camera.main.transform;

    private void LookCamera() => sprite.LookAt(cameraTransform);

    public void Checker(Removable script)
    {
        LookCamera();

        if (script != null)
        {
            Show(script);
            ChangeValue(script);
        }
        else
        {
            Hide();
        }
    }

    private void Show(Removable script)
    {
        sprite.position = script.transform.position;
        sprite.localScale = Vector3.Lerp(sprite.localScale, endSize, Time.deltaTime * 5f);
    }

    private void ChangeValue(Removable script)
    {
        switch (MouseController.instance.GetWheel())
        {
            case MouseWheel.STAY:
                return;
            case MouseWheel.UP:
                script.TextNum++;
                break;
            case MouseWheel.DOWN:
                script.TextNum--;
                break;
        }
    }

    private void Hide()
    {
        sprite.localScale = Vector3.Lerp(sprite.localScale, Vector3.zero, Time.deltaTime * 9f);
    }
}