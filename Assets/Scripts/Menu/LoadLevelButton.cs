using Assets.Scripts.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class LoadLevelButton : MonoBehaviour
    {
        [SerializeField] private string levelName;

        public static MainMenuManager MainMenu;
        void Start()
        {
            MainMenu = FindFirstObjectByType<MainMenuManager>();

            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(LoadLevel);
            }
        }

        private void LoadLevel()
        {
            if (SceneExists(levelName))
            {
                AudioManager.Instance.PlayOneShot(MainMenu.OnLevelSelected, levelName);
                SceneManager.LoadScene(levelName);
            }
            else
            {
                Debug.LogError($"Requested Scene '{levelName}' does not exist");
            }
        }

        private bool SceneExists(string sceneName) => SceneUtility.GetBuildIndexByScenePath(sceneName) != -1;
    }
}