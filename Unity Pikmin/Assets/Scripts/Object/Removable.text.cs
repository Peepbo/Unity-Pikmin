using UnityEngine;
using TMPro;

partial class Removable
{
    [Header("Text Setting")]
    public GameObject textMeshObj;
    private TextMeshPro tmPro;
    private Color colorA = new Color(1f, 57f / 255, 57f / 255);
    private Color colorB = new Color(0f, 51f / 255, 1f);
    private bool changeColor;

    private void TextAwake()
    {
        tmPro = textMeshObj.GetComponent<TextMeshPro>();
        tmPro.enabled = false;
    }

    private void SetText()
    {
        if (works <= 0) tmPro.enabled = false;

        else
        {
            tmPro.enabled = true;

            if (works >= needs) changeColor = true;
            tmPro.SetText(works + "\n―\n" + needs);
        }
    }

    private void ColorUpdate()
    {
        textMeshObj.transform.rotation = Quaternion.identity;

        if (changeColor == false)
        {
            tmPro.color = Color.Lerp(tmPro.color, colorA, Time.deltaTime * 4f);
        }
        else
        {
            tmPro.color = Color.Lerp(tmPro.color, colorB, Time.deltaTime * 4f);
        }
    }
}