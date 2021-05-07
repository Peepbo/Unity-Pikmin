using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class test : MonoBehaviour
{
    float second, milliseconds;
    string secondString, millisecondsString;

    TextMeshProUGUI tmp;

    private void Awake()
    {
        second = 60f;
        milliseconds = 0f;
        secondString = "60";
        millisecondsString = "00";

        tmp = GetComponent<TextMeshProUGUI>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        second -= Time.deltaTime;
        secondString = ((int)second).ToString();
        Debug.Log(Utils.DecimalRoundDown(second, 2) - (int)second);
        milliseconds = Utils.DecimalRoundDown(second, 2) - (int)second;
        //milliseconds = (int)((second - (int)second) % Time.frameCount * 100);
        string lengthCheckString = milliseconds.ToString();
        millisecondsString = lengthCheckString.Length == 1? lengthCheckString + "0" : lengthCheckString;

        tmp.text = secondString + ":" + millisecondsString;
    }
}
