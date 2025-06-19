using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class EggCookingController : MonoBehaviour
{
    public TMP_Text messageText;                // 用于显示提示信息
    public Collider panTriggerZone;             // 锅的触发区域
    public float[] phaseDurations = { 10f, 5f };  // 2个阶段的时间
    public float flipThreshold = -0.7f;          // Dot < -0.7 表示翻面成功
    public float[] burnThresholds = { 5f, 8f };

    private bool[] burnedFlags = { false, false };                              // 每面是否已烤糊
    private int currentPhase = 0;
    private bool isCooking = false;
    private bool isEggInPan = false;
    private bool currentlyInside = false;
    private float exitTimer = 0f;
    private Transform eggTransform;
    private Color OriginalColor = Color.white, BurnedColor = Color.black;
    private float oriHeight = 0f, maxHeight = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Egg"))
        {
            eggTransform = other.transform;
            currentlyInside = true;
            // Debug.Log("Egg entered");
            if (!isCooking)
                StartCoroutine(CookingRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Egg"))
        {
            currentlyInside = false;
            oriHeight = eggTransform.position.y;
        }
    }

    private string oldMessage = null;
    void Update()
    {
        if (!isCooking)
            return;

        if (currentlyInside)
        {
            if (oldMessage != null && !isEggInPan)
                messageText.text = oldMessage;
            oldMessage = null;
            isEggInPan = true;
            exitTimer = 0f;

        }
        else
        {
            exitTimer += Time.deltaTime;
            maxHeight = Mathf.Max(maxHeight, eggTransform.position.y);
            if (exitTimer > 1f)
            {
                maxHeight = oriHeight;
                isEggInPan = false;
                if (oldMessage == null)
                {
                    oldMessage = messageText.text;
                    ScoreManager.Instance.AddScore(-10);
                }

                // Debug.Log(oldMessage);
                messageText.text = "Oh no! Pick up the egg!";
            }
        }

    }

    private IEnumerator CookingRoutine()
    {
        isCooking = true;

        for (int i = 0; i < phaseDurations.Length; i++)
        {
            currentPhase = i + 1;

            float[] timePassed = { 0f, 0f };
            while (timePassed.Min() < phaseDurations[i])
            {
                // 若鸡蛋不在锅中，暂停计时
                yield return new WaitUntil(() => currentlyInside);
                // Debug.Log($"transform {eggTransform.position}");
                if (burnedFlags[0])
                {
                    timePassed[0] = phaseDurations[i];
                }
                if (burnedFlags[1])
                {
                    timePassed[1] = phaseDurations[i];
                }
                if (Vector3.Dot(eggTransform.up, Vector3.up) < 0)
                {
                    if (timePassed[0] > phaseDurations[i] && !burnedFlags[1])
                    {
                        yield return StartCoroutine(WaitForFlip());
                        messageText.text = $"Stage {currentPhase}, A side";
                    }
                    else if (messageText.text == $"Stage {currentPhase}, B side" && currentlyInside)
                    {
                        messageText.text = $"Stage {currentPhase}, A side";
                        Debug.Log($"oriHeight {oriHeight}, maxHeight {maxHeight}");
                        ScoreManager.Instance.AddScore(5 + (int)((maxHeight - oriHeight) * 10f));
                        maxHeight = 0f;
                    }
                    else
                    {
                        messageText.text = $"Stage {currentPhase}, A side";
                    }
                    timePassed[0] += Time.deltaTime;
                }
                else if (Vector3.Dot(eggTransform.up, Vector3.up) > 0)
                {
                    if (timePassed[1] > phaseDurations[i] && !burnedFlags[0])
                    {
                        yield return StartCoroutine(WaitForFlip());
                        messageText.text = $"Stage {currentPhase}, B side";
                    }
                    else if (messageText.text == $"Stage {currentPhase}, A side" && currentlyInside)
                    {
                        messageText.text = $"Stage {currentPhase}, B side";
                        Debug.Log($"oriHeight {oriHeight}, maxHeight {maxHeight}");
                        ScoreManager.Instance.AddScore(5 + (int)((maxHeight - oriHeight) * 10f));
                        maxHeight = 0f;
                    }
                    else
                    {
                        messageText.text = $"Stage {currentPhase}, B side";
                    }
                    timePassed[1] += Time.deltaTime;
                }

                yield return null;
            }

            if (i < phaseDurations.Length - 1 && (!burnedFlags[0] || !burnedFlags[1]))
            {
                messageText.text = "Flip for next stage...";
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(WaitForFlip());
            }

        }

        if (burnedFlags[0] && burnedFlags[1])
        {
            messageText.text = "OK...just let it go. Place it on the plate anyway.";
        }
        else
        {
            messageText.text = "Frying complete! Now place it on the plate.";
        }
        int NumFriedSides = burnedFlags.Count(b => !b);
        ScoreManager.Instance.AddScore(NumFriedSides * 40);
        yield break;
    }

    float[] overFryTime = { 0f, 0f };
    private IEnumerator WaitForFlip()
    {
        float dot = Vector3.Dot(eggTransform.up, Vector3.up);
        int side = dot < 0 ? 0 : 1;
        if (!burnedFlags[side])
        {
            messageText.text = $"Please flip before this side is burnt...";
        }

        // 初始面朝上或下的方向（自动判断第一次翻面目标）
        bool waitForDown = dot > 0;

        while (true)
        {
            dot = Vector3.Dot(eggTransform.up, Vector3.up);
            if (!currentlyInside)
            {
                yield return null;
                continue;
            }

            if (waitForDown && dot < flipThreshold)
                break;
            if (!waitForDown && dot > -flipThreshold)
                break;

            overFryTime[side] += Time.deltaTime;
            if (overFryTime[side] > burnThresholds[currentPhase - 1] || burnedFlags[side])
            {
                burnedFlags[side] = true;
                messageText.text = $"Stop frying this side. It's already burnt...";
                int NumBurnedSides = burnedFlags.Count(b => b);
                Renderer rend = eggTransform.GetComponentInChildren<Renderer>();
                rend.material.color = Color.Lerp(OriginalColor, BurnedColor, NumBurnedSides * 0.5f);
            }
            if (burnedFlags[0] && burnedFlags[1])
            {
                yield return new WaitForSeconds(1f);
                yield break;
            }

            yield return null;
        }

        messageText.text = "Flip successful, continue frying...";
        yield return new WaitForSeconds(1f);
    }

    public bool IsCookingFinished()
    {
        return currentPhase >= phaseDurations.Length;
    }

    public void QuitCooking()
    {
        isCooking = false;
    }
    public bool isCookedWell()
    {
        return !burnedFlags[0] && !burnedFlags[1];
    }
}

