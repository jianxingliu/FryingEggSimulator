using UnityEngine;
using TMPro;

public class NoteTrigger : MonoBehaviour
{
    public GameObject noteUI;   // UI面板
    public Transform player;
    public float interactionDistance = 3f;
    public float facingThreshold = 0.7f; // 面对门的方向阈值（越接近1越严格）
    public TMP_Text doorHintText;  // 拖入提示文本
    public AudioSource paperSound;

    int openFrame = 0;

    void Start()
    {
        if (doorHintText != null)
            doorHintText.enabled = false;
        Debug.Log(doorHintText.text);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 toDoor = (transform.position - player.position).normalized;
        Vector3 playerForward = player.forward;

        bool isFacing = Vector3.Dot(playerForward, toDoor) > facingThreshold;
        // Debug.Log($"Distance: {distance}, Facing: {Vector3.Dot(playerForward, toDoor)}");
        if (distance <= interactionDistance && isFacing)
        {
            if (doorHintText != null)
            {
                doorHintText.text = "Press 'F' to read";
                doorHintText.enabled = true;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                paperSound.Play();
                noteUI.SetActive(true);
                Time.timeScale = 0f; // 暂停游戏可选（避免误操作）
                if (doorHintText != null)
                    doorHintText.enabled = false; // 关闭提示文本
            }
            if (noteUI.activeSelf)
            {
                openFrame++;
            }

        }
        else
        {
            if (doorHintText != null)
                doorHintText.enabled = false;
        }

        if (openFrame >= 2 && Input.GetKeyDown(KeyCode.F))
        {
            noteUI.SetActive(false);
            Time.timeScale = 1f;
            paperSound.Play();
            openFrame = 0;
        }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         isPlayerNear = true;
    //         // 可弹出提示：按 F 阅读
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         isPlayerNear = false;
    //         noteUI.SetActive(false);
    //         Time.timeScale = 1f;
    //     }
    // }
}

