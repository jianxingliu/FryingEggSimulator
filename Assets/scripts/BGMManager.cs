using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip normalBGM;
    public AudioClip cookedBGM;
    public float transitionDuration = 2f;

    private bool hasSwitched = false;

    void Start()
    {
        if (audioSource != null && normalBGM != null)
        {
            audioSource.clip = normalBGM;
            audioSource.loop = true;
            audioSource.volume = 0.1f;
            audioSource.Play();
        }
    }

    // 外部调用此方法触发切换
    public void SwitchToCookedBGM()
    {
        if (!hasSwitched && cookedBGM != null)
        {
            hasSwitched = true;
            StartCoroutine(SmoothTransition(cookedBGM));
        }
    }

    private IEnumerator SmoothTransition(AudioClip newClip)
    {
        // 淡出当前音乐
        float startVolume = audioSource.volume;

        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / transitionDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // 淡入新音乐
        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, startVolume, t / transitionDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
    }
}
