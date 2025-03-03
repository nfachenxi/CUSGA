using System.IO;
using UnityEngine;

public static class Config
{
    // 配置文件名
    private const string CONFIG_FILE_NAME = "config.json";
    
    // 获取配置文件路径，使用 Application.persistentDataPath 确保跨平台兼容性
    private static string configFilePath => Path.Combine(Application.persistentDataPath, CONFIG_FILE_NAME);
    
    // 存储配置数据的对象
    private static ConfigData configData;

    // 静态构造函数，在第一次访问 Config 类时自动调用
    static Config()
    {
        LoadConfig();
    }

    // 加载配置文件
    public static void LoadConfig()
    {
        if (File.Exists(configFilePath))
        {
            string jsonString = File.ReadAllText(configFilePath);
            configData = JsonUtility.FromJson<ConfigData>(jsonString);
        }
        else
        {
            // 使用默认值初始化配置
            configData = new ConfigData(true, true, 0, 0); // 默认音量为 0（最大音量）
            SaveConfig();
        }
    }

    // 保存配置文件
    private static void SaveConfig()
    {
        // 将配置对象序列化为 JSON 字符串，并写入文件
        string jsonString = JsonUtility.ToJson(configData);
        File.WriteAllText(configFilePath, jsonString);
    }

    // 音乐开关属性
    public static bool MusicOn
    {
        get => configData.MusicOn;
        set
        {
            if (configData.MusicOn != value)
            {
                configData.MusicOn = value;
                SaveConfig(); // 保存更改后的配置
                SoundManager.Instance.MusicOn = value; // 更新 SoundManager 中的状态
            }
        }
    }

    // 音效开关属性
    public static bool SoundOn
    {
        get => configData.SoundOn;
        set
        {
            if (configData.SoundOn != value)
            {
                configData.SoundOn = value;
                SaveConfig(); // 保存更改后的配置
                SoundManager.Instance.SoundOn = value; // 更新 SoundManager 中的状态
            }
        }
    }

    // 音乐音量属性
    public static int MusicVolume
    {
        get => configData.MusicVolume;
        set
        {
            if (configData.MusicVolume != value)
            {
                configData.MusicVolume = Mathf.Clamp(value, -80, 0); // 确保值在范围内
                SaveConfig();
                SoundManager.Instance.MusicVolume = value;
            }
        }
    }

    public static int SoundVolume
    {
        get => configData.SoundVolume;
        set
        {
            if (configData.SoundVolume != value)
            {
                configData.SoundVolume = Mathf.Clamp(value, -80, 0); // 确保值在范围内
                SaveConfig();
                SoundManager.Instance.SoundVolume = value;
            }
        }
    }
}