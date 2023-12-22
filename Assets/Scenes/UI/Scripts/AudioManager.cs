using UnityEngine;
using System;
[Serializable]
public class Sounds
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds;

    public float volume;

    public string nameClip;

    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        foreach (Sounds s in sounds)
        {
            s.volume = volume;
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        Play(nameClip);
    }
    public void ChangeVolume(float volume)
    {
        foreach (Sounds s in sounds)
        {
            s.volume = volume;
            s.source.volume = volume;
        }
    }
    public void Play(string name)
    {
        foreach (var sound in sounds)
        {
            sound.source.Stop();
        }
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " does not exist");
            return;
        }
        s.source.Play();
    }
}
