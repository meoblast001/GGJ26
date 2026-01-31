using System.Collections;
using System.Threading.Tasks;
using Mobile;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMobileBehavior : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField hostInput;
    [SerializeField] private ConnectButton connectButton;
    [SerializeField] private string gameplaySceneName;

    private bool isConnected = false;

    void Start()
    {
        connectButton.OnPress += OnConnectButtonPress;
    }

    private void OnConnectButtonPress()
    {
        Debug.Log("Trying to connect");

        var host = hostInput.text;
        networkManager.NetworkClient.OnConnected += () => isConnected = true;
        Task.Run(() => networkManager.NetworkClient.Connect(host, networkManager.tcpPort));
        AwaitConnectedToServer();
    }

    private IEnumerator AwaitConnectedToServer()
    {
        Debug.Log("Awaiting connection");
        while (!isConnected)
            yield return null;
        OnConnectedToServer();
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Connected");
        SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
