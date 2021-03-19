using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public static Spaceship instance;

    public Vector3 pos;
    public GameObject lightVulume;
    public ParticleSystem particle;
    public GameObject smoke;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    private void Update()
    {
        transform.parent.Rotate(0, -30 * Time.deltaTime, 0);
    }

    IEnumerator delayActive(float time, GameObject obj)
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
