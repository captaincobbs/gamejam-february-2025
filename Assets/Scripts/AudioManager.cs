using FMOD.Studio;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("This Scene does not have an AudioManager");
            }

            return instance;
        }
    }

    public void PlayOneShot(EventReference sound)
    {
        Debug.Log($"Event Played: {sound.Path}");
        RuntimeManager.PlayOneShot(sound, Vector3.zero);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
