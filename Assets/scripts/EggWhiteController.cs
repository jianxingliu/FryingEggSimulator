using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class EggWhiteController : MonoBehaviour
{
    public Material solidMaterial;      // 不透明材质
    public Material transparentMaterial; // 初始透明材质
    public MeshRenderer meshRenderer;
    public XRGrabInteractable grabInteractable;
    public GameObject eggPrefab;        // 生成的新鸡蛋 prefab
    public Transform spawnPoint;        // 新鸡蛋生成位

    private bool isSolid = false;
    private float transitionTime = 3.0f;    // 渐变时间（秒）
    private Color initialColor; // 半透明黄色（RGBA）
    private Color finalColor;       // 完全白色不透明
    private bool isInPan = false;

    void Start()
    {
        initialColor = transparentMaterial.color; // 半透明黄色（RGBA）
        finalColor = solidMaterial.color;

        if (meshRenderer != null && transparentMaterial != null)
            meshRenderer.material = transparentMaterial;

        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }

    public void StartSolidifying(float waitTime)
    {
        if (!isSolid)
        {
            StartCoroutine(Solidify(waitTime));
        }
    }

    // 执行变实操作
    private IEnumerator Solidify(float waitTime)
    {
        // 等待指定时间
        yield return new WaitForSeconds(waitTime);
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            // 平滑过渡颜色和透明度
            // float lerpValue = Mathf.Lerp(0f, 1f, elapsedTime / transitionTime);  // 透明度从透明到不透明
            Color currentColor = Color.Lerp(initialColor, finalColor, elapsedTime / transitionTime); // 从黄色到白色

            // currentColor.a = lerpValue; // 控制透明度渐变
            meshRenderer.material.color = currentColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最终确保蛋白完全不透明，且颜色为白色
        meshRenderer.material.color = finalColor;

        // 执行蛋白凝固操作
        MakeSolidAndGrabbable();
    }

    // 将蛋白变为不透明并可抓取
    private void MakeSolidAndGrabbable()
    {
        if (isSolid) return;

        isSolid = true;

        if (meshRenderer != null && solidMaterial != null)
            meshRenderer.material = solidMaterial;

        if (grabInteractable != null)
            grabInteractable.enabled = true;
    }

    IEnumerator CheckPanEntryAndRespawn()
    {
        Debug.Log("检查鸡蛋是否掉入锅内...");
        yield return new WaitForSeconds(3f);

        if (!isInPan)
        {
            ScoreManager.Instance.AddScore(-10);
            Debug.Log("鸡蛋未掉入锅内，重新生成新鸡蛋。");
            Instantiate(eggPrefab, spawnPoint.position, Quaternion.identity);
        }

    }

    public void SetInPan(bool inPan)
    {
        isInPan = inPan;
    }

    public void OnEggBreak()
    {
        StartCoroutine(CheckPanEntryAndRespawn());
    }
}


