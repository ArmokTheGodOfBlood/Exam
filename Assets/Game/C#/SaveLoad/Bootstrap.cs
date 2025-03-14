using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string mainScene = "MainScene";

    private void Start()
    {
        SceneManager.LoadSceneAsync(mainScene);
    }
}