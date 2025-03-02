using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Level.UI
{
    public class LoseScreen : MonoBehaviour
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button retryButton;

        void Start()
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            retryButton.onClick.AddListener(() =>
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            });
        }

        void ReturnToMainMenu()
        {
            Debug.Log("Return to menu");
            SceneManager.LoadScene("MainScene");
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
    }
}
