using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    public TextMeshPro tmPro;

    public Color colorA;
    public Color colorB;

    public bool btn;
    public bool trigger;

    public Animator anim;

    void Update()
    {
        if(btn)
        {
            tmPro.color = Color.Lerp(tmPro.color, colorB, Time.deltaTime * 4f);
        }
        else
        {
            tmPro.color = Color.Lerp(tmPro.color, colorA, Time.deltaTime * 4f);
        }

        if(trigger)
        {
            trigger = false;
            anim.SetTrigger("Big");
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}