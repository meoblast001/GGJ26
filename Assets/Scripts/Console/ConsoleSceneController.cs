using Network;
using UnityEngine;

namespace Console
{
    public class ConsoleSceneController : MonoBehaviour
    {
        [SerializeField] GuardManager guardManager;
        [SerializeField] ExitDoorController exitDoorController;

        void Awake()
        {
            exitDoorController.OnGuardReachesExit += OnGuardReachesExit;
        }

        private void OnGuardReachesExit(int guardId)
        {
            if (guardId == guardManager.PlayerGuardId)
                Debug.Log("You win!");
        }
    }
}
