using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EggPanInteraction : MonoBehaviour
{
    public Rigidbody panRigidbody;              // 锅的 Rigidbody（需设置）
    public float frictionForceMultiplier = 10f; // 模拟摩擦系数
    public float wallBounceForce = 5f;          // 锅壁反弹力度
    public string panBottomTag = "PanBottom";   // 锅底物体的 Tag
    public string panWallTag = "PanWall";       // 锅壁物体的 Tag

    private Rigidbody eggRb;
    private HashSet<Collider> contactBottoms = new();  // 当前正在接触的锅底列表

    void Start()
    {
        eggRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 仅在接触锅底时才模拟摩擦
        if (contactBottoms.Count > 0 && panRigidbody != null)
        {
            Vector3 relativeVelocity = panRigidbody.velocity - eggRb.velocity;
            relativeVelocity.y = 0;  // 仅考虑水平摩擦力
            Debug.Log(relativeVelocity);

            Vector3 frictionForce = relativeVelocity * frictionForceMultiplier;
            eggRb.AddForce(frictionForce, ForceMode.Force);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 锅底接触：记录接触
        if (collision.collider.CompareTag(panBottomTag))
        {
            contactBottoms.Add(collision.collider);
            Debug.Log("Contact");
        }

        // 锅壁接触：施加反弹力
        if (collision.collider.CompareTag(panWallTag))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 bounceDir = contact.normal;
                eggRb.AddForce(bounceDir * wallBounceForce, ForceMode.Impulse);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 离开锅底：移除记录
        if (collision.collider.CompareTag(panBottomTag))
        {
            contactBottoms.Remove(collision.collider);
            Debug.Log("exit contact");
        }
    }
}

