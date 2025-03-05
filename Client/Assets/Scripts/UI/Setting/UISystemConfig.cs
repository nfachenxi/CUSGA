using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindow
{
    // Button组件用于切换音乐和音效开关
    public Button buttonMusicOn; // 音乐开启按钮
    public Button buttonMusicOff; // 音乐关闭按钮
    public Button buttonSoundOn; // 音效开启按钮
    public Button buttonSoundOff; // 音效关闭按钮

    // Slider组件用于调整音量，范围为 -80 到 0
    public Slider sliderMusic; // 音乐音量滑块
    public Slider sliderSound; // 音效音量滑块

    private void Start()
    {
        // 确保在Start方法中重新加载配置
        Config.LoadConfig();

        // 初始化按钮状态
        UpdateButtonStates();

        // 设置滑块到保存的音量值，这里不再需要转换
        if (sliderMusic != null) sliderMusic.value = Config.MusicVolume;
        if (sliderSound != null) sliderSound.value = Config.SoundVolume;

        // 添加点击事件监听器（避免重复添加）
        AddEventListeners();
    }

    private void AddEventListeners()
    {
        if (buttonMusicOn != null) buttonMusicOn.onClick.RemoveAllListeners();
        if (buttonMusicOff != null) buttonMusicOff.onClick.RemoveAllListeners();
        if (buttonSoundOn != null) buttonSoundOn.onClick.RemoveAllListeners();
        if (buttonSoundOff != null) buttonSoundOff.onClick.RemoveAllListeners();

        if (sliderMusic != null) sliderMusic.onValueChanged.RemoveAllListeners();
        if (sliderSound != null) sliderSound.onValueChanged.RemoveAllListeners();

        if (buttonMusicOn != null) buttonMusicOn.onClick.AddListener(() => ToggleMusic(true));
        if (buttonMusicOff != null) buttonMusicOff.onClick.AddListener(() => ToggleMusic(false));
        if (buttonSoundOn != null) buttonSoundOn.onClick.AddListener(() => ToggleSound(true));
        if (buttonSoundOff != null) buttonSoundOff.onClick.AddListener(() => ToggleSound(false));

        if (sliderMusic != null) sliderMusic.onValueChanged.AddListener(MusicVolume);
        if (sliderSound != null) sliderSound.onValueChanged.AddListener(SoundVolume);
    }

    public override void OnYesClick()
    {
        // 播放UI确认声音并调用基类方法
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        base.OnYesClick();
    }

    private void UpdateButtonStates()
    {
        // 根据当前设置更新按钮的交互状态
        if (buttonMusicOn != null && buttonMusicOff != null)
        {
            buttonMusicOn.gameObject.SetActive(!Config.MusicOn); // 如果音乐已关闭，则显示"开启"按钮
            buttonMusicOff.gameObject.SetActive(Config.MusicOn); // 如果音乐已开启，则显示"关闭"按钮
        }
        if (buttonSoundOn != null && buttonSoundOff != null)
        {
            buttonSoundOn.gameObject.SetActive(!Config.SoundOn); // 如果音效已关闭，则显示"开启"按钮
            buttonSoundOff.gameObject.SetActive(Config.SoundOn); // 如果音效已开启，则显示"关闭"按钮
        }
    }

    public void ToggleMusic(bool on)
    {
        // 只有在状态改变时才进行更新
        if (Config.MusicOn != on)
        {
            Config.MusicOn = on;
            UpdateButtonStates();
            PlaySound(); // 播放点击音效
        }
    }

    public void ToggleSound(bool on)
    {
        // 只有在状态改变时才进行更新
        if (Config.SoundOn != on)
        {
            Config.SoundOn = on;
            UpdateButtonStates();
            PlaySound(); // 播放点击音效
        }
    }

    public void MusicVolume(float vol)
    {
        // 滑块值已经符合音量范围，无需额外转换
        int volume = Mathf.RoundToInt(vol);
        if (Config.MusicVolume != volume)
        {
            Config.MusicVolume = volume;
            PlaySound(); // 播放点击音效
        }
    }

    public void SoundVolume(float vol)
    {
        // 滑块值已经符合音量范围，无需额外转换
        int volume = Mathf.RoundToInt(vol);
        if (Config.SoundVolume != volume)
        {
            Config.SoundVolume = volume;
            PlaySound(); // 播放点击音效
        }
    }

    float lastPlay = 0;
    private void PlaySound()
    {
        if (Time.realtimeSinceStartup - lastPlay > 0.3f)
        {
            lastPlay = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        }
    }
}