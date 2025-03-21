using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

	class UIElement
	{
		public string Resources;
		public bool CaChe;
		public GameObject Instance;
	}

	private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

	public UIManager()
	{
		//this.UIResources.Add(typeof(UITest), new UIElement() {Resources = "UI/UITest", CaChe = true });
		this.UIResources.Add(typeof(UIInfo), new UIElement() {Resources = "UI/UIInfo", CaChe = false });
		this.UIResources.Add(typeof(UISystemConfig), new UIElement() {Resources = "UI/UISystemConfig", CaChe = false });
		this.UIResources.Add(typeof(UISystemConfigLevel), new UIElement() {Resources = "UI/UISystemConfigLevel", CaChe = false });
		this.UIResources.Add(typeof(UIBaseMap), new UIElement() {Resources = "UI/UIBaseMap", CaChe = true });
    }

    ~UIManager()
	{
	}

    /// <summary>
    /// 显示指定类型的预制体
    /// </summary>
    public T Show<T>() where T : Component
    {
        return Show(typeof(T)) as T;
    }

    /// <summary>
    /// 显示指定类型的预制体 (非泛型版本)
    /// </summary>
    public Component Show(Type type)
    {
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null)
                    return null;
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent(type);
        }
        return null;
    }

    public void Close(Type type)
	{
		if(UIResources.ContainsKey(type))
		{
            UIElement info = UIResources[type];
			if(info.CaChe)
			{
				info.Instance.SetActive(false);
			}
			else
			{
				GameObject.Destroy(info.Instance);
				info.Instance = null;
            }
        }	
	}

    public bool IsOpen(Type type)
    {
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            return info.Instance != null && info.Instance.activeSelf;
        }
        return false;
    }

    public void CloseIfOpen(Type type)
    {
        if (IsOpen(type))
        {
            Close(type);
        }
    }

}
