using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EggBreaker : MonoBehaviour
{
    public GameObject crackedEggPrefab;    // 破碎鸡蛋的 prefab（包含蛋白+蛋黄）
    public float breakImpactThreshold = 5f;

    private Rigidbody eggRb;
    private bool isBroken = false;
    public float delayBeforeBreak = 5f;
    public ParticleSystem warningEffect; 

    void Start()
    {
        eggRb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        // 获取碰撞冲击力
        float impactForce = collision.relativeVelocity.magnitude;
        Debug.Log($"impact Force: {impactForce}");
        if (impactForce > breakImpactThreshold)
        {
            StartCoroutine(DelayedBreak());
        }
    }

    IEnumerator DelayedBreak()
    {
        isBroken = true;

        // 启用粒子
        if (warningEffect != null)
            warningEffect.Play();

        yield return new WaitForSeconds(delayBeforeBreak);

        // 粒子可自动停止或销毁
        if (warningEffect != null)
            warningEffect.Stop();

        BreakEgg();  // 执行碎裂逻辑
    }

    void BreakEgg()
    {
        isBroken = true;

        // 实例化破碎鸡蛋（蛋白 + 蛋黄）
        GameObject crackedEgg = Instantiate(crackedEggPrefab, transform.position, Quaternion.identity);

        // 复制旋转和速度
        Rigidbody yolkRb = crackedEgg.GetComponentInChildren<Rigidbody>();
        if (yolkRb != null && eggRb != null)
        {
            yolkRb.velocity = eggRb.velocity;
            yolkRb.angularVelocity = eggRb.angularVelocity;
        }

        EggWhiteController eggWhite = crackedEgg.GetComponentInChildren<EggWhiteController>();
        eggWhite.OnEggBreak();

        // 销毁未破鸡蛋
        Destroy(gameObject);
    }
}
