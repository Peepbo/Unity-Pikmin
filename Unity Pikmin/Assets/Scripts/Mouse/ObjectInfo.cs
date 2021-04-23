using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectInfo : MonoBehaviour
{
    public  Transform    sprite;
    private Transform    cameraTransform;

    private Vector3      objPosition;
    private Vector3      endSize = new Vector3(0.5f, 0.5f, 0f);

    private ObjectType   objType;
    private Removable    removableObj;
    private TextMeshPro  removableTextMesh;
    private float        objSize;

    private bool         isView;

    private void Start() => cameraTransform = Camera.main.transform;

    private void LookCamera() => sprite.LookAt(cameraTransform);

    public void Checker(Transform obj)
    {
        LookCamera();

        if (obj == null)
        {
            Hide();
            removableObj = null;
            removableTextMesh = null;
            isView = false;
            return;
        }

        if (isView == false)
        {
            isView = true;

            objPosition = obj.position;
            objType = obj.GetComponent<IObject>().objectType;

            if(objType == ObjectType.MOVEABLE_OBJ)
            {
                removableObj = obj.GetComponent<Removable>();
                removableTextMesh = removableObj.textMeshObj.GetComponent<TextMeshPro>();
            }
        }

        else
        {
            Show();

            if (objSize == 0) objSize = obj.GetComponent<IObject>().infoSize;
        }
    }

    private void Show()
    {
        sprite.position = objPosition;
        sprite.localScale = Vector3.Lerp(sprite.localScale, endSize * objSize, Time.deltaTime * 5f);

        if (removableObj != null)
            removableTextMesh.enabled = true;
    }

    private void Hide()
    {
        sprite.localScale = Vector3.Lerp(sprite.localScale, Vector3.zero, Time.deltaTime * 9f);
        objSize = 0;

        if (removableObj != null && removableObj.factory.childCount == 0)
            removableTextMesh.enabled = false;
    }
}