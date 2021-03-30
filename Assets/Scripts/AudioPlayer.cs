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
        [Range(0f, 1f)]
        public float volume;
    }

    public AudioCommand[] cases;

    private Dictionary<string, AudioCommand> dictionary;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        dictionary = new Dictionary<string, AudioCommand>();
    }

    void Start()
    {
        foreach (AudioCommand command in cases)
        {
            dictionary.Add(command.name, command);
        }
        cases = null;
    }


    public void Play(string name)
    {
        AudioCommand command;
        if (dictionary.TryGetValue(name, out command))
        {
            source.PlayOneShot(command.clip, source.volume * command.volume);
        }
    }
}
