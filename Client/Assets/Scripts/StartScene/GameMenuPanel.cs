using UnityEngine;

public class GameMenuPanel : MonoBehaviour
{
    public void OnClickInfo()
    {
        UIManager.Instance.Show<UIInfo>();
    }

    public void OnClickExit()
    {
        Debug.Log("退出游戏...");
        Application.Quit(); // 退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 如果是在编辑器中运行，则停止播放模式
#endif
        
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISystemConfig>();
    }

    public void OnClickStartGame()
    {
        SceneManager.Instance.LoadScene("");
    }

    public void OnClickUpSkill()
    {
        
    }

    public void OnClickSelectArchive()
    {
        
    }

    public void OnClickIllustrated()//点击图鉴
    {
        
    }

    public void OnClickRecording()
    {
        
    }
}
