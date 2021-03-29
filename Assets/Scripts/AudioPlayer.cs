using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public struct AudioCommand
    {
        public string name;
        public AudioClip clip;
    }

    public AudioCommand[] cases;

    private Dictionary<string, AudioClip> dictionary;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        dictionary = new Dictionary<string, AudioClip>();
    }

    void Start()
    {
        foreach (AudioCommand command in cases)
        {
            dictionary.Add(command.name, command.clip);
        }
        cases = null;
    }


    public void Play(string name)
    {
        AudioClip clip;
        if (dictionary.TryGetValue(name, out clip))
        {
            source.PlayOneShot(clip, source.volume);
        }
    }
}
