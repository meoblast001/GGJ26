using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Console
{
    public class ConsoleSceneController : MonoBehaviour
    {
        [SerializeField] GuardManager guardManager;
        [SerializeField] PlayerHealth playerHealth;
        [SerializeField] ExitDoorController exitDoorController;
        [SerializeField] PlayerInputManager playerInputManager;
        [SerializeField] TMP_Text winText;
        [SerializeField] TMP_Text loseText;
        [SerializeField] Slider healthSlider;

        [Header("Scenes")]
        [SerializeField] private string gameOverNextScene;

        private bool gameOver = false;

        void Awake()
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(false);

            exitDoorController.OnGuardReachesExit += OnGuardReachesExit;
            playerHealth.OnHealthZero += OnHealthZero;
        }

        void Update()
        {
            healthSlider.value = playerHealth.Health;
        }

        private void OnGuardReachesExit(int guardId)
        {
            if (!gameOver && guardId == guardManager.PlayerGuardId)
                StartCoroutine(GameOverSequenceCoroutine(winText));
        }

        private void OnHealthZero()
        {
            if (!gameOver)
                StartCoroutine(GameOverSequenceCoroutine(loseText));
        }

        private IEnumerator GameOverSequenceCoroutine(TMP_Text gameOverText)
        {
            gameOver = true;
            gameOverText.gameObject.SetActive(true);
            playerInputManager.Disable();
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(gameOverNextScene, LoadSceneMode.Single);
        }
    }
}
