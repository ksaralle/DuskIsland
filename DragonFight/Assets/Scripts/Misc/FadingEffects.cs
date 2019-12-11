using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class FadingEffects : MonoBehaviour
{
    #region Variables
    [SerializeField]
    [Tooltip("how fast the scene fades")]
    private float fadeSpeed = 0.8f;

    [SerializeField]
    [Tooltip("the image used to fade")]
    public Image fadeImage;

    public enum FadeDirection
    {
        In, //Alpha = 1
        Out //Alpha = 0
    } 
    #endregion

    void OnEnable()
    {
        StartCoroutine(Fade(FadeDirection.Out));
    }


    private IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = 0;
        float fadeEndValue = 1;
        if (fadeDirection == FadeDirection.Out) //1 to 0
        {
            alpha = 1;
            fadeEndValue = 0;
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            fadeImage.enabled = false;
        } else //0 to 1
        {
            fadeImage.enabled = true;
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }
        
    }

    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string nextScene)
    {
        yield return Fade(fadeDirection);
        SceneManager.LoadScene(nextScene);
    }

    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out)? -1 : 1);

    }

}
