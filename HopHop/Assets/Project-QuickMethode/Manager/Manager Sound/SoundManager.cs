using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Can call this Script instancely thought this Name of Script!
    private static SoundManager m_this;

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

    private readonly float m_musicVolumn = 1f;
    private readonly float m_soundVolumn = 1f;
#pragma warning disable IDE0051 // Remove unused private members
    private readonly bool m_musicMute = false;
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
    private readonly bool m_soundMute = false;
#pragma warning restore IDE0051 // Remove unused private members

    private AudioData m_music;
    private readonly List<AudioData> m_sound = new List<AudioData>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_this = this;
    }

    #region Music

    public static void SetMusicStart(AudioClip Music, float Volumn = 1f)
    {
        SetMusicStop();

        GameObject Object = QGameObject.SetCreate(Music.name);
        AudioSource Audio = QComponent.GetComponent<AudioSource>(Object);
        Audio.clip = Music;
        Audio.loop = true;
        Audio.volume = Volumn * m_this.m_musicVolumn;
        Audio.spatialBlend = 0;
        Audio.Play();

        m_this.m_music = new AudioData(Audio, Volumn);
    }

    public static void SetMusicStop()
    {
        if (m_this.m_music == null)
        {
            return;
        }

        Destroy(m_this.m_music.Source.gameObject);
        m_this.m_music = null;
    }

    public static void SetMusicMute(bool Mute)
    {
        if (m_this.m_music == null)
        {
            return;
        }

        m_this.m_music.Source.mute = Mute;
    }

    #endregion

    #region Sound

    public static void SetSoundStart2D(AudioClip Sound, bool Loop, float Volumn = 1f)
    {
        GameObject Object = QGameObject.SetCreate(Sound.name);
        AudioSource Audio = QComponent.GetComponent<AudioSource>(Object);
        Audio.clip = Sound;
        Audio.loop = Loop;
        Audio.volume = Volumn * m_this.m_soundVolumn;
        Audio.spatialBlend = 0;
        Audio.Play();

        m_this.m_sound.Add(new AudioData(Audio, Volumn));

        if (!Loop)
        {
            m_this.StartCoroutine(m_this.ISetSoundStop(Audio));
        }
    }

    public static void SetSoundStart3D(AudioClip Sound, Vector2 Pos, float Distance, bool Loop, float Volumn = 1f)
    {
        GameObject Object = QGameObject.SetCreate(Sound.name);
        AudioSource Audio = QComponent.GetComponent<AudioSource>(Object);
        Audio.clip = Sound;
        Audio.loop = Loop;
        Audio.volume = Volumn * m_this.m_soundVolumn;
        Audio.spatialBlend = 1;
        Audio.transform.position = Pos;
        Audio.maxDistance = Distance;
        Audio.Play();

        m_this.m_sound.Add(new AudioData(Audio, Volumn));

        if (!Loop)
        {
            m_this.StartCoroutine(m_this.ISetSoundStop(Audio));
        }
    }

    private IEnumerator ISetSoundStop(AudioSource Audio)
    {
        yield return new WaitUntil(() => !Audio.isPlaying);
        Destroy(Audio.gameObject);
    }

    public static void SetSoundStop()
    {
        m_this.StopAllCoroutines();
        foreach (AudioData Sound in m_this.m_sound)
        {
            if (Sound.Source != null)
            {
                Destroy(Sound.Source.gameObject);
            }

            m_this.m_sound.Remove(Sound);
        }
    }

    public static void SetSoundMute(bool Mute)
    {
        foreach (AudioData Sound in m_this.m_sound)
        {
            if (Sound.Source != null)
            {
                Sound.Source.mute = Mute;
            }
        }
    }

    #endregion
}