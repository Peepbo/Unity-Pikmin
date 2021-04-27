using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UiManager : LevelLoader
{
    [Header("PikminUI")]
    public TextMeshProUGUI allPikminsNumber;
    public TextMeshProUGUI stayPikminsNumber;
    public TextMeshProUGUI orderPikminsNumber;
    private Action textAct;

    [Header("MenuUI")]
    public GameObject menuPanel;
    public Button menuButton, continueButton, lobbyButton;

    protected override void Start()
    {
        base.Start();

        textAct = () =>
        {
            SetText(allPikminsNumber, PlayerController.instance.allNums.ToString());
            SetText(stayPikminsNumber, PlayerController.instance.myPikminCount.ToString());
            SetText(orderPikminsNumber, PlayerController.instance.orderNums.ToString());
        };

        menuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            menuPanel.SetActive(true);
        });

        continueButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            menuPanel.SetActive(false);
        });

        lobbyButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            base.LoadLevel(0);
        });
    }

    private void Update() => textAct();

    private void SetText(TextMeshProUGUI tmPro, string value) { tmPro.text = value; }
}