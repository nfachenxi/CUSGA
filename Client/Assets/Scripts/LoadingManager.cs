using System.Collections;
using Common;
using UnityEngine;

public class LoadingManager : MonoBehaviour {
    

    public LoadingPanel loadingPanel;
    public GameMenuPanel gameMenuPanel;
    
    private static bool isBack = false;

    // Use this for initialization
    IEnumerator Start()
    {
        if (!isBack)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
            UnityLogger.Init();
            Log.Init("Unity");
            Log.Info("LoadingManager start");
        
            loadingPanel.gameObject.SetActive(true);
            yield return new WaitForSeconds(12f);
            gameMenuPanel.gameObject.SetActive(true);
            SoundManager.Instance.PlayMusic(SoundDefine.Music_MainMenu);
            yield return DataManager.Instance.LoadData();
            isBack = true;
        }
        else
        {
            {
                gameMenuPanel.gameObject.SetActive(true);
                SoundManager.Instance.PlayMusic(SoundDefine.Music_MainMenu);
            }
        }
        yield return null;
    }

    void Update()
    {

    }
}
