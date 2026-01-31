using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartConsoleBehavior : MonoBehaviour
{
    [SerializeField] private GameObject waitingForClientDisplay;
    [SerializeField] private string gameplaySceneName;

    void Start()
    {
        StartCoroutine(AwaitConnection());
    }

    private IEnumerator AwaitConnection()
    {
        Debug.Log("Waiting for client to connect");
        yield return new WaitForSeconds(3);
        //Wait for client. Then..
        OnClientConnected();
    }

    private void OnClientConnected()
    {
        Debug.Log("Client connected");
        waitingForClientDisplay.SetActive(false);
        SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
