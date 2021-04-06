using UnityEngine;
using TMPro;
using System;

public class UiManager : MonoBehaviour
{
    [Header("PikminUI")]
    public TextMeshProUGUI allPikminsNumber;
    public TextMeshProUGUI stayPikminsNumber;
    public TextMeshProUGUI orderPikminsNumber;
    private Action textAct;

    private void Start()
    {
        textAct = () =>
        {
            SetText(allPikminsNumber, PlayerController.instance.allNums.ToString());
            SetText(stayPikminsNumber, PlayerController.instance.myPikminCount.ToString());
            SetText(orderPikminsNumber, PlayerController.instance.orderNums.ToString());
        };
    }

    private void Update() => textAct();

    private void SetText(TextMeshProUGUI tmPro, string value) { tmPro.text = value; }
}