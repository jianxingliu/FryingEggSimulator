using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class StartMenu : MonoBehaviour
{

    public void StartGame()
    {
        // 加载游戏主场景（确保它已添加到 Build Settings 中）
        Debug.Log("Loading...");
        SceneManager.LoadScene("DemoScene"); // 改成你的场景名
    }

    public void QuitGame()
    {
        // 退出游戏
        Debug.Log("Quitting...");
        Application.Quit();
        
        // 如果在编辑器中运行，可以使用以下代码来停止播放模式
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
