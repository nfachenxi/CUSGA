using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioMixer audioMixer;
    public AudioSource musicAudioSource;
    public AudioSource soundAudioSource;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    private bool musicOn;
    private bool soundOn;
    private int musicVolume;
    private int soundVolume;

    protected override void OnStart()
    {
        // 确保配置被加载并应用于 SoundManager
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        Config.LoadConfig();

        // 初始化音乐和音效的状态和音量
        MusicOn = Config.MusicOn;
        SoundOn = Config.SoundOn;
        MusicVolume = Config.MusicVolume;
        SoundVolume = Config.SoundVolume;
    }

    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            if (musicOn != value)
            {
                musicOn = value;
                this.MusicMute(!musicOn);
            }
        }
    }

    public bool SoundOn
    {
        get { return soundOn; }
        set
        {
            if (soundOn != value)
            {
                soundOn = value;
                this.SoundMute(!soundOn);
            }
        }
    }

    public int MusicVolume
    {
        get { return musicVolume; }
        set
        {
            if (musicVolume != value)
            {
                musicVolume = Mathf.Clamp(value, -80, 0); // 确保音量值在有效范围内
                SetVolume("MusicVolume", musicVolume);
                if (!musicOn) this.MusicMute(true); // 如果音乐关闭，则保持静音
            }
        }
    }

    public int SoundVolume
    {
        get { return soundVolume; }
        set
        {
            if (soundVolume != value)
            {
                soundVolume = Mathf.Clamp(value, -80, 0); // 确保音量值在有效范围内
                SetVolume("SoundVolume", soundVolume);
                if (!soundOn) this.SoundMute(true); // 如果音效关闭，则保持静音
            }
        }
    }

    public void MusicMute(bool mute)
    {
        SetVolume("MusicVolume", mute ? -80 : musicVolume);
    }

    public void SoundMute(bool mute)
    {
        SetVolume("SoundVolume", mute ? -80 : soundVolume);
    }

    private void SetVolume(string name, int value)
    {
        // 将整数值直接设置为浮点数，因为AudioMixer期望的是dB值
        this.audioMixer.SetFloat(name, value);
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlayMusic: '{0}' not existed.", name);
            return;
        }
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlaySound: '{0}' not existed.", name);
            return;
        }
        soundAudioSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }
    protected void PlayClipOnAudioSource(AudioSource source, AudioClip clip, bool isLoop)
    {
        source.clip = clip;
        source.loop = isLoop;
        source.Play();
    }
}