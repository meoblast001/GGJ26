using UnityEngine;

namespace Console
{
    public class GuardManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private PlayerInputManager playerInputManager;

        [Header("Initial State Configuration")]
        [SerializeField] private GuardCharacterController playerStartGuard;

        private GuardCharacterController[] guardCharacterControllers;

        void Awake()
        {
            guardCharacterControllers = GetComponentsInChildren<GuardCharacterController>();
        }

        void Update()
        {
            playerStartGuard.MovementDirection = playerInputManager.WalkVector;
        }
    }
}
