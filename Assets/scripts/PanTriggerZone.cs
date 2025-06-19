using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanTriggerZone : MonoBehaviour
{
    public float timeBeforeSolidifies = 3f;  // 等待凝固的时间（秒）
    public GameObject saltUI;

    private bool isCooking = false;
    private bool addedSalt = false;
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"On trigger enter {other.name}");
        EggWhiteController eggWhite = other.GetComponent<EggWhiteController>();
        if (eggWhite != null)
        {
            eggWhite.StartSolidifying(timeBeforeSolidifies);
            eggWhite.SetInPan(true);
            isCooking = true;
        }
    }

    IEnumerator SaltMessage()
    {
        saltUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        saltUI.SetActive(false);
    }

    void OnParticleCollision(GameObject other)
    {
        if (!addedSalt && isCooking && other.CompareTag("Salt"))
        {
            Debug.Log("salt added");
            addedSalt = true;
            ScoreManager.Instance.AddScore(20);
            StartCoroutine(SaltMessage());
        }
    }

}
