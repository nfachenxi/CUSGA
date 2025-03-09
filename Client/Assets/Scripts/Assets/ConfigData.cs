

// 使用 [Serializable] 属性使这个类能够被序列化为 JSON 格式
[System.Serializable]
public class ConfigData
{
    // 音乐是否开启
    public bool MusicOn;
    
    // 音效是否开启
    public bool SoundOn;
    
    // 音乐音量
    public int MusicVolume;
    
    // 音效音量
    public int SoundVolume;

    // 构造函数用于初始化配置数据
    public ConfigData(bool musicOn, bool soundOn, int musicVolume, int soundVolume)
    {
        MusicOn = musicOn;
        SoundOn = soundOn;
        MusicVolume = musicVolume;
        SoundVolume = soundVolume;
    }
}