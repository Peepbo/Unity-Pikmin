using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whistle : MonoBehaviour
{
    [Header("Whistle")]
    public GameObject   cylinder;
    public GameObject   topParticle;
    public GameObject   bottomParticle;
    public Transform    radiusPivot;

    private Vector3     cylinderEndScale = new Vector3(7f, 2f, 7f);
    private Vector3     particleEndScale = new Vector3(1.43f, 1.43f, 1.43f);

    public float GetRadius { get; private set; }

    private void Start()
    {
        cylinder.transform.localScale = topParticle.transform.localScale = 
            bottomParticle.transform.localScale = Vector3.zero;
        GetRadius = 0;
    }

    public void Play()
    {
        cylinder.transform.position = MouseController.instance.GetHit;

        if (cylinder.transform.localScale.y > 1.99f) return;

        GetRadius = (MouseController.instance.cursor3D.position - radiusPivot.position).magnitude;

        if (!cylinder.activeSelf)
        {
            cylinder.SetActive(true);
            topParticle.SetActive(true);
            bottomParticle.SetActive(true);
        }

        cylinder.transform.localScale = Vector3.Lerp(cylinder.transform.localScale, cylinderEndScale, Time.deltaTime * 5f);

        topParticle.transform.localScale = bottomParticle.transform.localScale = 
            Vector3.Lerp(topParticle.transform.localScale, particleEndScale, Time.deltaTime * 5f);
    }

    public void Stop()
    {
        if (!cylinder.activeSelf) return;

        transform.position = MouseController.instance.GetHit;

        GetRadius = (MouseController.instance.cursor3D.position - radiusPivot.position).magnitude;

        cylinder.transform.localScale = Vector3.Lerp(cylinder.transform.localScale, Vector3.zero, Time.deltaTime * 8f);

        topParticle.transform.localScale = bottomParticle.transform.localScale =
            Vector3.Lerp(topParticle.transform.localScale, Vector3.zero, Time.deltaTime * 8f);

        if (cylinder.transform.localScale.y < 0.1f)
        {
            cylinder.SetActive(false);
            topParticle.SetActive(false);
            bottomParticle.SetActive(false);

            GetRadius = 0;
        }
    }
}