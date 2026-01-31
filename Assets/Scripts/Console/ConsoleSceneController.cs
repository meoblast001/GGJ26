using System.Collections;
using TMPro;
using UnityEngine;

namespace Console
{
    public class ConsoleSceneController : MonoBehaviour
    {
        [SerializeField] GuardManager guardManager;
        [SerializeField] ExitDoorController exitDoorController;
        [SerializeField] PlayerInputManager playerInputManager;
        [SerializeField] TMP_Text winText;
        [SerializeField] TMP_Text loseText;

        private bool gameOver = false;

        void Awake()
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(false);

            exitDoorController.OnGuardReachesExit += OnGuardReachesExit;
        }

        private void OnGuardReachesExit(int guardId)
        {
            if (!gameOver && guardId == guardManager.PlayerGuardId)
                StartCoroutine(GameOverSequenceCoroutine(winText));
        }

        private IEnumerator GameOverSequenceCoroutine(TMP_Text gameOverText)
        {
            gameOver = true;
            gameOverText.gameObject.SetActive(true);
            playerInputManager.Disable();
            yield return new WaitForSeconds(5);
            Application.Quit();
        }
    }
}
