using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Level.UI
{

    public class WinScreen : MonoBehaviour
    {
        [SerializeField] public Button NextLevelButton;
        [SerializeField] public Button MainMenuButton;
        [SerializeField] public Button RestartButton;

        private void Start()
        {
            NextLevelButton.onClick.AddListener(NextLevel);
            MainMenuButton.onClick.AddListener(ReturnToMainMenu);
            RestartButton.onClick.AddListener(RestartLevel);
        }

        void ReturnToMainMenu()
        {
            Debug.Log("Return to menu");
            SceneManager.LoadScene("MainScene");
        }

        void NextLevel()
        {
            int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextScene < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                ReturnToMainMenu();
            }
        }

        void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
