using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private string levelName;
    void Start()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(LoadLevel);
        }
    }

    private void LoadLevel()
    {
        if (SceneExists(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError($"Requested Scene '{levelName}' does not exist");
        }
    }

    private bool SceneExists(string sceneName) => SceneUtility.GetBuildIndexByScenePath(sceneName) != -1;
}
