using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    public PlayerController player;

    public TextMeshProUGUI allPikminsNumber;
    public TextMeshProUGUI orderPikminsNumber;

    private void Update()
    {
        SetText(allPikminsNumber, player.allNums.ToString());
        SetText(orderPikminsNumber, player.orderNums.ToString());
    }

    private void SetText(TextMeshProUGUI tmPro, string value) { tmPro.text = value; }
}