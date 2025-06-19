using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI thankYouText;
    public float fadeDuration = 2f;
    public float textDelay = 0.5f;
    public float textFadeDuration = 1f;

    public void StartWhiteFade()
    {
        StartCoroutine(FadeToWhite());
    }

    IEnumerator FadeToWhite()
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // 确保最终是全白
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);

        yield return new WaitForSeconds(textDelay);

        // Step 3: 文字淡入
        t = 0f;
        Color textColor = thankYouText.color;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / textFadeDuration);
            thankYouText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }
        thankYouText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
    }
}

