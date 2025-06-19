using UnityEngine;
using TMPro;

public class EggPlacementDetector : MonoBehaviour
{
    public TMP_Text messageText;
    public string eggTag = "Egg";
    public EggCookingController cookingController; // 引用之前的煎蛋控制器

    private bool placedInPlate = false;

    void OnTriggerEnter(Collider other)
    {
        if (placedInPlate) return;
        Debug.Log($"On trigger enter {other.name}");
        if (other.CompareTag(eggTag))
        {
            if (cookingController != null && cookingController.IsCookingFinished())
            {
                messageText.text = "Now enjoy your egg and your life!";
                // 伪代码：在某个事件里调用
                FindObjectOfType<BackgroundMusicManager>().SwitchToCookedBGM();
                placedInPlate = true;
                cookingController.QuitCooking();
                ScoreManager.Instance.AddScore(20);
            }
            else
            {
                messageText.text = "The eggs are not cooked yet";
            }
        }
    }

    public bool isPlaced()
    {
        return placedInPlate && cookingController != null && cookingController.isCookedWell();
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         FindObjectOfType<BackgroundMusicManager>().SwitchToCookedBGM();
    //     }
    // }
}

