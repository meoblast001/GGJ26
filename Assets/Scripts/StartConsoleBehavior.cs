using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartConsoleBehavior : MonoBehaviour
{
    [SerializeField] private GameObject waitingForClientDisplay;
    [SerializeField] private string gameplaySceneName;

    void Start()
    {
        NetworkController networkController = NetworkController.Instance;
        networkController.Initialize();
        networkController.OnClientConnected += OnClientConnected;
    }
    void OnClientConnected()
    {
        waitingForClientDisplay.SetActive(false);
        SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
