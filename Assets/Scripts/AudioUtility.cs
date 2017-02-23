using UnityEngine;
using System.Collections;
using System.Linq;  // for FirstOrDefault

public class AudioUtility : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioSource[] audioSources;

    /// <summary>
    ///     Fill in the audio source array and first audio source.
    /// </summary>
    private void FetchAudioSources()
    {
        audioSources = GetComponents<AudioSource>();
        audioSource = audioSources.FirstOrDefault();
    }


    /// <summary>
    ///     Gets the primary (first) audio source component.
    /// </summary>
    public AudioSource AudioSource
    {
        get
        {
            FetchAudioSources();
            return audioSource;
        }
    }

    /// <summary>
    ///     Gets all audio source components.
    /// </summary>
    public AudioSource[] AudioSources
    {
        get
        {
            FetchAudioSources();
            return audioSources;
        }
    }

    /// <summary>
    ///     Gets a random audio source component of this gameobject.
    /// </summary>
    public AudioSource RandomAudioSource
    {
        get
        {
            FetchAudioSources();

            AudioSource ret = null;

            int amount = audioSources.Length;
            if (amount > 0)
            {
                ret = audioSources[Random.Range(0, amount)];
            }

            return ret;
        }
    }
}
