using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private string levelName;
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
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
