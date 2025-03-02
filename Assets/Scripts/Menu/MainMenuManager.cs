using Assets.Scripts.Level;
using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] public EventReference OnLoad;
        [SerializeField] public EventReference OnLevelSelected;
        [SerializeField] public EventReference OnSelect;
        [SerializeField] public EventReference OnUnload;
        void Start()
        {
            LoadLevelButton.MainMenu = this;
            AudioManager.Instance.PlayOneShot(OnLoad, $"MainMenu.{nameof(OnLoad)}");
        }

        private void OnDestroy()
        {
            AudioManager.Instance.PlayOneShot(OnUnload, $"MainMenu.{nameof(OnUnload)}");
        }
    }
}