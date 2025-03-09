using UnityEngine;

public abstract class UIWindow : MonoBehaviour {
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler Onclose;

    public virtual System.Type type { get { return this.GetType(); } }

    public GameObject Root;

    public enum WindowResult
    {
        None = 0,
        Yes,
        No,
    }

    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(this.type);
        if (this.Onclose != null)
            this.Onclose(this, result);
        this.Onclose = null;
    }

    public virtual void OnCloseClick()
    {
        this.Close();
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }
    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
    }
    public virtual void OnNoClick()
    {
        this.Close(WindowResult.No);
    }

}