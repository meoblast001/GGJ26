using System.Net.Sockets;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartConsoleBehavior : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject waitingForClientDisplay;
    [SerializeField] private string gameplaySceneName;

    void Awake()
    {
        networkManager.NetworkServer.OnClientConnected += OnClientConnected;
    }

    private void OnClientConnected(TcpClient tcpClient)
    {
        waitingForClientDisplay.SetActive(false);
        SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
