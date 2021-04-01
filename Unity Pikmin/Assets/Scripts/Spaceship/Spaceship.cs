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
            Instantiate(seed, topPos.position, Quaternion.identity);
            seedNum--;
            yield return new WaitForSeconds(time);
        }
    }

    private IEnumerator delayActive(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(true);
        transform.parent.GetComponent<Animator>().SetTrigger("Big");
    }

    public void Big()
    {
        transform.parent.GetComponent<Animator>().SetTrigger("Big");
    }

    private void Smoke()
    {
        StartCoroutine(delayActive(1.0f, smoke));
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