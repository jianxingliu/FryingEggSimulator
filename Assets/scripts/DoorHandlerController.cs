using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHandleController : MonoBehaviour
{
    public Transform door;         // 要旋转的门
    public Transform pivot;        // 门的旋转轴中心（通常是门的一边）
    public float rotationSpeed = -30f; // 门打开速度（角度每秒）
    public float maxOpenAngle = 90f;
    public float minOpenAngle = 0f;

    private XRBaseInteractor interactor = null;
    private bool isGrabbed = false;

    void OnEnable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnGrab);
        GetComponent<XRGrabInteractable>().selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnGrab);
        GetComponent<XRGrabInteractable>().selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject.transform.GetComponent<XRBaseInteractor>();
        isGrabbed = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
        isGrabbed = false;
    }

    void Update()
    {
        if (isGrabbed && interactor != null)
        {
            // 计算抓手运动方向并映射为门旋转
            Vector3 handDir = interactor.transform.position - pivot.position;
            float horizontalMovement = Vector3.Dot(handDir.normalized, pivot.right);
            float angleDelta = horizontalMovement * rotationSpeed * Time.deltaTime;

            float currentYAngle = door.localEulerAngles.y;
            currentYAngle = (currentYAngle > 180) ? currentYAngle - 360 : currentYAngle; // 保持在 -180~180

            float newAngle = Mathf.Clamp(currentYAngle + angleDelta, minOpenAngle, maxOpenAngle);

            door.localEulerAngles = new Vector3(0f, newAngle, 0f);
        }
    }
}

