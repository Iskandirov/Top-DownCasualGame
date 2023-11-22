using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds;

    public float volume;

    public string nameClip;

    public static AudioManager instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
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
    public void Play(string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " does not exist");
            return;
        }
        s.source.Play();
    } 
    public void Stop(string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " does not exist");
            return;
        }
        s.source.Stop();
    }
}
