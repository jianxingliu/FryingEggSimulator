using UnityEngine;

public class SaltBottle : MonoBehaviour
{
    public ParticleSystem saltParticles;
    public float angleThreshold = 120f; // 角度阈值：瓶口向下超过此角度才撒盐

    void Update()
    {
        // 瓶口方向是 transform.up，与世界重力方向 Vector3.down 做夹角判断
        float angle = Vector3.Angle(transform.up, Vector3.down);

        if (angle < angleThreshold)
        {
            if (!saltParticles.isPlaying)
                saltParticles.Play();
        }
        else
        {
            if (saltParticles.isPlaying)
                saltParticles.Stop();
        }
    }
}

