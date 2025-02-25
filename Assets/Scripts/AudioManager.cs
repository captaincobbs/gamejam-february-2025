using FMOD.Studio;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple AudioManagers in this scene");

            Instance = this;
        }
    }

    private void InstancePlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public static void PlayOneShot(EventReference sound) => Instance?.InstancePlayOneShot(sound);
}
