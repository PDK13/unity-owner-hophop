using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : SingletonManager<SoundManager>
{
    [Serializable]
    private class AudioData
    {
        public AudioSource Source;
        public float Volumn;

        public AudioData(AudioSource Source, float Volumn)
        {
            this.Source = Source;
            this.Volumn = Volumn;
        }
    }

    private float m_mainMusicVolumn = 1f;
    private float m_mainSoundVolumn = 1f;

    public float MainMusicVolumn => m_mainMusicVolumn;

    public float MainSoundVolumn => m_mainSoundVolumn;

    private bool m_mainMusicMute = false;
    private bool m_mainSoundMute = false;

    public float MainMusicMute => m_mainMusicVolumn;

    public float MainSoundMute => m_mainSoundVolumn;

    private AudioData m_music;
    private List<AudioData> m_sound = new List<AudioData>();

#if UNITY_EDITOR

    [SerializeField] private AudioClip m_audioMusic = null;
    [SerializeField] private AudioClip m_audioSound = null;

#endif

    private void Awake()
    {
        SetInstance();
    }

    #region Music

    public static void SetMusic(AudioClip Music, float FixedVolumn = 1f)
    {
        SetMusicStop();
        //
        GameObject Object = QGameObject.SetCreate(Music.name);
        AudioSource Audio = QComponent.GetComponent<AudioSource>(Object);
        Audio.clip = Music;
        Audio.loop = true;
        Audio.volume = Mathf.Clamp(FixedVolumn, 0, 1) * Instance.m_mainMusicVolumn;
        Audio.spatialBlend = 0;
        Audio.mute = Instance.m_mainMusicMute;
        Audio.Play();
        //
        Instance.m_music = new AudioData(Audio, FixedVolumn);
    }

    public static void SetMusicStop()
    {
        if (Instance.m_music == null)
            return;
        //
        Destroy(Instance.m_music.Source.gameObject);
        Instance.m_music = null;
    }

    public static void SetMusicMute(bool Mute)
    {
        Instance.m_mainMusicMute = Mute;
        //
        if (Instance.m_music == null)
            return;
        //
        Instance.m_music.Source.mute = Mute;
    }

    public static void SetMusicVolumn(float MainVolumn = 1f)
    {
        Instance.m_mainMusicVolumn = Mathf.Clamp(MainVolumn, 0, 1);
        //
        if (Instance.m_music == null)
            return;
        //
        Instance.m_music.Source.volume = Instance.m_music.Volumn * Instance.m_mainMusicVolumn;
    }

    #endregion

    #region Sound

    public static void SetSound2D(AudioClip Sound, bool Loop, float FixedVolumn = 1f)
    {
        GameObject Object = QGameObject.SetCreate(Sound.name);
        AudioSource Audio = QComponent.GetComponent<AudioSource>(Object);
        Audio.clip = Sound;
        Audio.loop = Loop;
        Audio.volume = Mathf.Clamp(FixedVolumn, 0, 1) * Instance.m_mainSoundVolumn;
        Audio.spatialBlend = 0;
        Audio.mute = Instance.m_mainSoundMute;
        Audio.Play();
        //
        Instance.m_sound.Add(new AudioData(Audio, FixedVolumn));
        //
        if (!Loop)
            Instance.StartCoroutine(Instance.ISetSoundStop(Audio));
    }

    public static void SetSound3D(AudioClip Sound, Vector2 Pos, float Distance, bool Loop, float FixedVolumn = 1f)
    {
        GameObject Object = QGameObject.SetCreate(Sound.name);
        AudioSource Audio = QComponent.GetComponent<AudioSource>(Object);
        Audio.clip = Sound;
        Audio.loop = Loop;
        Audio.volume = Mathf.Clamp(FixedVolumn, 0, 1) * Instance.m_mainSoundVolumn;
        Audio.spatialBlend = 1;
        Audio.transform.position = Pos;
        Audio.maxDistance = Distance;
        Audio.Play();
        //
        Instance.m_sound.Add(new AudioData(Audio, FixedVolumn));
        //
        if (!Loop)
            Instance.StartCoroutine(Instance.ISetSoundStop(Audio));
    }

    private IEnumerator ISetSoundStop(AudioSource Audio)
    {
        yield return new WaitUntil(() => !Audio.isPlaying);
        Destroy(Audio.gameObject);
    }

    public static void SetSoundStop()
    {
        Instance.StopAllCoroutines();
        foreach (AudioData Sound in Instance.m_sound)
        {
            if (Sound.Source == null)
                continue;
            //
            Destroy(Sound.Source.gameObject);
        }
        Instance.m_sound.Clear();
    }

    public static void SetSoundMute(bool Mute)
    {
        Instance.m_mainSoundMute = Mute;
        //
        foreach (AudioData Sound in Instance.m_sound)
        {
            if (Sound.Source == null)
                continue;
            //
            Sound.Source.mute = Mute;
        }
    }

    public static void SetSoundVolumn(float MainVolumn = 1f)
    {
        Instance.m_mainSoundVolumn = Mathf.Clamp(MainVolumn, 0, 1);
        //
        foreach (AudioData Sound in Instance.m_sound)
        {
            if (Sound.Source == null)
                continue;
            //
            Sound.Source.volume = Sound.Volumn * Instance.m_mainSoundVolumn;
        }
    }

    #endregion

#if UNITY_EDITOR

    public void SetEditorMusic()
    {
        SoundManager.SetMusic(SoundManager.Instance.m_audioMusic);
    }

    //

    public void SetEditorSoundLoop()
    {
        SoundManager.SetSound2D(SoundManager.Instance.m_audioSound, true);
    }

    public void SetEditorSoundNotLoop()
    {
        SoundManager.SetSound2D(SoundManager.Instance.m_audioSound, false);
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    private SoundManager m_target;

    private SerializedProperty m_audioMusic;
    private SerializedProperty m_audioSound;

    private void OnEnable()
    {
        m_target = target as SoundManager;
        //
        m_audioMusic = QUnityEditorCustom.GetField(this, "m_audioMusic");
        m_audioSound = QUnityEditorCustom.GetField(this, "m_audioSound");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_audioMusic);
        if (QUnityEditor.SetButton("Play Music"))
            SoundManager.Instance.SetEditorMusic();
        //
        QUnityEditorCustom.SetField(m_audioSound);
        if (QUnityEditor.SetButton("Play Sound Loop"))
            SoundManager.Instance.SetEditorSoundLoop();
        if (QUnityEditor.SetButton("Play Sound Not Loop"))
            SoundManager.Instance.SetEditorSoundNotLoop();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif