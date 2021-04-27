using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [Header("LevelLoaderUI")]
    public GameObject loadingPanel;
    private Slider slider;
    private Image backImg;
    private bool isLoad;
    private int nextSceneIndex;

    private IEnumerator fadeCoroutine;

    protected virtual void Start()
    {
        backImg = loadingPanel.GetComponent<Image>();
        slider = loadingPanel.GetComponentInChildren<Slider>();

        slider.gameObject.SetActive(false);

        fadeCoroutine = FadeImg(Color.clear);
        StartCoroutine(fadeCoroutine);
    }

    public void LoadLevel(int sceneIndex)
    {
        if (isLoad) return;
        isLoad = true;

        nextSceneIndex = sceneIndex;
        loadingPanel.SetActive(true);

        StartCoroutine(LoadingDelay());
    }

    IEnumerator FadeImg(Color color)
    {
        while (backImg.color != color)
        {
            backImg.color = Color.Lerp(backImg.color, color, 2f * Time.fixedDeltaTime);

            yield return null;
        }
    }

    IEnumerator LoadingDelay()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = FadeImg(Color.black);

        yield return StartCoroutine(fadeCoroutine);

        StartCoroutine(LoadAsynchronously());
    }

    IEnumerator LoadAsynchronously()
    {
        List<float> fList = Utils.RandomFloats(0.01f, 0.3f, 5);
        slider.gameObject.SetActive(true);

        while (!Utils.AlmostNumber(slider.value, 1f))
        {
            float randomValue = fList[Random.Range(0, fList.Count)];

            slider.value = Mathf.Clamp01(slider.value + Time.fixedDeltaTime * randomValue);

            yield return null;
        }

        SceneManager.LoadSceneAsync(nextSceneIndex);
    }
}
