using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSetting : MonoBehaviour
{
    Transform main;
    TextMesh tMesh;
    private void Awake()
    {
        tMesh = GetComponent<TextMesh>();
    }

    private void Start()
    {
        main = Camera.main.transform;
    }

    // Update is called once per frame
    void Update() => transform.rotation = main.rotation;

    public void ChangeText(string txt) => tMesh.text = txt;
}