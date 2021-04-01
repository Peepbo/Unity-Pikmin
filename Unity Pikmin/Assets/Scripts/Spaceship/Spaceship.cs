using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public static Spaceship instance;

    public GameObject seed;
    public Transform topPos;
    public GameObject lightVulume;
    public ParticleSystem particle;
    public GameObject smoke;
    public Transform endPos;
    public int seedNum;

    private void Awake()
    {
        instance = this;
    }

    public void ActiveDissemination(int plusSeed)
    {
        seedNum += plusSeed;
        StartCoroutine(dissemination(1.25f));
    }

    private IEnumerator dissemination(float time)
    {
        yield return new WaitForSeconds(1f);
        while(seedNum > 0)
        {
            Big();
            GameObject obj = ObjectPool.instance.BorrowObject("Seed");
            obj.transform.position = topPos.position;
            obj.transform.parent = null;
            //Instantiate(seed, topPos.position, Quaternion.identity);
            seedNum--;
            yield return new WaitForSeconds(time);
        }
    }

    private void delayActive()
    {
        smoke.SetActive(true);
    }

    public void Big()
    {
        transform.GetComponent<Animator>().SetTrigger("Big");
    }

    private void Smoke()
    {
        Invoke("delayActive", 1.0f);
        //StartCoroutine(delayActive(1.0f, smoke));
    }

    private void TurnOn()
    {
        lightVulume.SetActive(true);
        var em = particle.emission;
        em.enabled = true;
    }

    public void PlayEffect()
    {
        TurnOn();
        Smoke();
    }

    public void StopEffect()
    {
        var em = particle.emission;
        em.enabled = false;
    }
}