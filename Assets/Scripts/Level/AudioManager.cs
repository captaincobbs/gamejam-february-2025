using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public bool LogEvents = true;

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
        if (!sound.IsNull)
        {
            if (LogEvents)
            {
                Debug.Log($"Event Played: {sound.Path}");
            }

            RuntimeManager.PlayOneShot(sound, Vector3.zero);
        }
        {
            Debug.LogWarning("Event Reference is null");
        }
    }

    public void SetParameterWithValue(string name, float value)
    {
        if (LogEvents)
        {
            Debug.Log($"Parameter Updated: {name} to {value}");
        }

        RuntimeManager.StudioSystem.setParameterByName(name, value);
    }

    public void SetParameterWithLabel(string name, string label)
    {
        if (LogEvents)
        {
            Debug.Log($"Parameter Updated: {name} to {label}");
        }

        RuntimeManager.StudioSystem.setParameterByNameWithLabel(name, label);
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
}
