using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public static Vector3 pos;
    public GameObject lightVulume;
    public ParticleSystem particle;
    public GameObject smoke;

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

    public void Smoke()
    {
        StartCoroutine(delayActive(1.0f, smoke));
    }

    public void turnOn()
    {
        lightVulume.SetActive(true);
        var em = particle.emission;
        em.enabled = true;
    }

    public void turnOff()
    {
        var em = particle.emission;
        em.enabled = false;
    }
}
