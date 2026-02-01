using System.Collections;
using System.Threading.Tasks;
using Mobile;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMobileBehavior : MonoBehaviour
{
    private NetworkController networkManager;
    [SerializeField] private TMP_InputField hostInput;
    [SerializeField] private ConnectButton connectButton;
    [SerializeField] private string gameplaySceneName;

    private bool isConnected = false;

    void Start()
    {
        NetworkController networkController = NetworkController.Instance;
        networkManager.Initialize();
        connectButton.OnPress += OnConnectButtonPress;
    }

    private void OnConnectButtonPress()
    {
        Debug.Log("Trying to connect");

        var host = hostInput.text;
        networkManager.OnClientConnected += () => isConnected = true;
        Task.Run(() => networkManager.StartClient(host));
        StartCoroutine(AwaitConnectedToServer());
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
