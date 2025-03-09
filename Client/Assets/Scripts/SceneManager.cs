using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneManager : MonoSingleton<SceneManager>
{
    UnityAction<float> onProgress;
    public Slider progressBar;
    public GameObject loadingPanel;
    public TextMeshProUGUI process;

    // Use this for initialization
    protected override void OnStart()
    {
    }



    public void LoadScene(string sceneName)
    {
        SoundManager.Instance.StopMusic();
        // 设置进度更新委托
        onProgress = (progress) =>
        {
            if (progressBar != null)
            {
                progressBar.value = progress * 100;  // 假设Slider的最大值设置为100
            }
            if (process != null)
            {
                process.text = "Loading... " + (int)(progress * 100) + "%";  // 更新进度百分比
            }
        };
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        Debug.LogFormat("LoadLevel: {0}", sceneName);
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        if (async != null)
        {
            async.allowSceneActivation = false;

            async.completed += LevelLoadCompleted;

            while (!async.isDone)
            {
                float progress = Mathf.Clamp01(async.progress / 0.9f); // Unity将进度设为0.9f作为完成标志
                if (onProgress != null)
                {
                    onProgress(progress); // 通过委托更新进度
                }

                if (progress >= 0.9f) // 当实际进度达到0.9（即接近完成）时
                {
                    async.allowSceneActivation = true; // 允许场景激活
                }

                yield return null;
            }
        }

        // 隐藏加载面板
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void LevelLoadCompleted(AsyncOperation obj)
    {
        if (onProgress != null)
            onProgress(1f);
        Debug.Log("LevelLoadCompleted:" + obj.progress);

    }
}