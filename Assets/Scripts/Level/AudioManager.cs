using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Level
{
    [ExecuteInEditMode]
    public class AudioManager : MonoBehaviour
    {
        private void Start()
        {
            if (Application.isPlaying)
            {
                if (IsFMODInitialized())
                {
                    LogEvents = true;
                }
            }
        }

        public bool LogEvents = false;

        public void PlayOneShot(EventReference sound, string varName)
        {
            if (!sound.IsNull)
            {
#if UNITY_EDITOR
                if (LogEvents)
                {
                    Debug.Log($"Event Played: {sound.Path}");
                }
#endif
                if (Application.isPlaying)
                {
                    RuntimeManager.PlayOneShot(sound, Vector3.zero);
                }
            }
            else
            {
                Debug.LogWarning($"Event Reference '{varName}' is null");
            }
        }

        public void SetParameterWithValue(string name, float value)
        {
            if (LogEvents)
            {
                Debug.Log($"Parameter Updated: {name} to {value}");
            }

            if (Application.isPlaying)
            {
                RuntimeManager.StudioSystem.setParameterByName(name, value);
            }
        }

        public void SetParameterWithLabel(string name, string label)
        {
            if (LogEvents)
            {
                Debug.Log($"Parameter Updated: {name} to {label}");
            }

            if (Application.isPlaying)
            {
                RuntimeManager.StudioSystem.setParameterByNameWithLabel(name, label);
            }
        }

        bool IsFMODInitialized() => RuntimeManager.StudioSystem.isValid();

        #region Singleton
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    AudioManager found = FindFirstObjectByType<AudioManager>();

                    if (found != null)
                    {
                        instance = found;
                    }
                }

                return instance;
            }
        }

        void Awake()
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
        #endregion
    }
}