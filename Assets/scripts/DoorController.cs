using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 3f;
    public float openAngle = 90f;
    public float rotationSpeed = 2f;

    public TMP_Text doorHintText;  // 拖入提示文本
    public float facingThreshold = 0.7f; // 面对门的方向阈值（越接近1越严格）
    public AudioSource doorAudioSource;
    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        if (doorHintText != null)
            doorHintText.enabled = false;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 toDoor = (transform.position - player.position).normalized;
        Vector3 playerForward = player.forward;

        bool isFacing = Vector3.Dot(playerForward, toDoor) > facingThreshold;

        if (distance <= interactionDistance && isFacing)
        {
            if (doorHintText != null)
            {
                if (isOpen)
                    doorHintText.text = "Press 'F' to close";
                else
                    doorHintText.text = "Press 'F' to open";
                doorHintText.enabled = true;
            }
                

            if (Input.GetKeyDown(KeyCode.F))
            {
                isOpen = !isOpen;
                // 播放对应的音效
                if (doorAudioSource != null)
                {
                    if(isOpen && doorOpenClip != null)
                        doorAudioSource.PlayOneShot(doorOpenClip);
                    else if(!isOpen && doorCloseClip != null)
                        doorAudioSource.PlayOneShot(doorCloseClip);
                }
            }
        }
        else
        {
            if (doorHintText != null)
                doorHintText.enabled = false;
        }

        // 平滑旋转门
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}


