using System.Net.Sockets;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartConsoleBehavior : MonoBehaviour
{
    [SerializeField] private NetworkController networkController;
    [SerializeField] private GameObject waitingForClientDisplay;
    [SerializeField] private string gameplaySceneName;

    void Start()
    {
        networkController.Initialize();
        networkController.OnClientConnected += OnClientConnected;
    }
    void OnClientConnected()
    {
        waitingForClientDisplay.SetActive(false);
        SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
