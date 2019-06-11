using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require that all objects with this script needs a audiosource component
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;    //Set Get Static Instance to call from other scripts [ALL scripts]   
    private AudioSource audioSource;        //Main audiosource component that sits on the 'AudioManager' object

    [SerializeField]
    private AudioClip[] audioClips;         //ALL audio clips in one list

    [SerializeField]
    private Transform cameraTransform;

    private void Awake()
    {
        //Dondestroyonload, set instance to this scripts, get component of audiosource
        DontDestroyOnLoad(this);
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    /// <summary>
    /// PlayAudioSource()
    /// Play audio from the audiosource with already a clip in stock
    /// and change the volme of the audiosource
    /// </summary>
    /// <param name="audioclip"></param>
    /// <param name="volume"></param>
    public void PlayAudioClip(AudioClip audioclip, float volume)
    {
        audioSource.PlayOneShot(audioclip, volume);
    }
    /// <summary>
    /// Play audio with own audiosource of that gameobject
    /// </summary>
    /// <param name="audiosource"></param>
    /// <param name="audioclip"></param>
    public void PlayAudioClip(AudioSource audiosource, AudioClip audioclip)
    {
        audiosource.PlayOneShot(audioclip);
    }
    /// <summary>
    /// Play an audio clip with given name
    /// </summary>
    /// <param Clip Name="audioclipname"></param>
    /// <param Position="position"></param>
    public void PlayAudioClip(string audioclipname, Transform position)
    {
        float distance = Vector3.Distance(position.position, cameraTransform.position);
        float volume = 1 - (distance / 100);
        AudioClip clip = (CheckContainAudio(audioclipname));

        if (clip && audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.PlayOneShot(clip, volume);
        }
        else if (clip && audioSource.clip == clip && !audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.PlayOneShot(clip, volume);
        }
    }
    /// <summary>
    /// Return an audioclip if given name is in the audioclips list
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private AudioClip CheckContainAudio(string name)
    {
        AudioClip clip = null;
        for (int i = 0; i < audioClips.Length; i++)
        {
            if (audioClips[i].name == name)
            {
                clip = audioClips[i];
            }
        }
        return clip;
    }
}
