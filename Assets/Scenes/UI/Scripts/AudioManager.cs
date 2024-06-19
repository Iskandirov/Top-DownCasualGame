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
}
[DefaultExecutionOrder(11)]
public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds, sfx;
    public AudioSource musicObj, sfxObj;

    public float volumeMusic;
    public float volumeSFX;

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

        PlayMusic(nameClip);
    }
    public void ChangeVolume(float volume,AudioSource source)
    {
        source.volume = volume;
    }
    public void MusicStop()
    {
        musicObj.Stop();
    }
    public void PlayMusic(string name)
    {
        MusicStop();
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " does not exist");
            return;
        }
        musicObj.clip = s.clip;
        musicObj.Play();
    }
    public void PlaySFX(string name)
    {
        Sounds s = Array.Find(sfx, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " does not exist");
            return;
        }
        sfxObj.PlayOneShot(s.clip);
    }
}
