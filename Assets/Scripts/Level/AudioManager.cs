using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Level
{
    [ExecuteInEditMode]
    public class AudioManager : MonoBehaviour
    {
        private void Start()
        {
            if (IsFMODInitialized())
            {
                LogEvents = true;
            }
        }

        public bool LogEvents = false;

        public void PlayOneShot(EventReference sound, string varName)
        {
            if (!sound.IsNull)
            {
                if (LogEvents)
                {
                    Debug.Log($"Event Played: {sound.Path}");
                }

                RuntimeManager.PlayOneShot(sound, Vector3.zero);
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
                    else
                    {
                        Debug.LogError("This Scene does not have an AudioManager");
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

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
        #endregion
    }
}