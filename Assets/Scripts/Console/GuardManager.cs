using System.Linq;
using UnityEngine;

namespace Console
{
    public class GuardManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        [Header("Managers")]
        [SerializeField] private PlayerInputManager playerInputManager;

        [Header("Configuration")]
        [SerializeField] private GuardCharacterController playerStartGuard;
        [SerializeField] private float guardSwitchMaxRadius;

        private GuardCharacterController[] guardCharacterControllers;
        private GuardCharacterController playerCurrentGuard;

        void Awake()
        {
            guardCharacterControllers = GetComponentsInChildren<GuardCharacterController>();

            for (int i = 0; i < guardCharacterControllers.Length; ++i)
                guardCharacterControllers[i].Id = i + 1;

            DoSwitchGuard(playerStartGuard);

            playerInputManager.OnSwitchGuard += OnSwitchGuard;
        }

        void Update()
        {
            playerCurrentGuard.MovementDirection = playerInputManager.WalkVector;
        }

        private void OnSwitchGuard()
        {
            var closestGuard = guardCharacterControllers.Where(guard => guard != playerCurrentGuard)
                .Select(guard =>
                {
                    var distance = Vector2.Distance(playerCurrentGuard.transform.position, guard.transform.position);
                    return (guard, distance);
                })
                .OrderBy(pair => pair.distance)
                .First();
            if (closestGuard.distance <= guardSwitchMaxRadius)
                DoSwitchGuard(closestGuard.guard);
        }

        private void DoSwitchGuard(GuardCharacterController newPlayerGuard)
        {
            playerCurrentGuard = newPlayerGuard;
            mainCamera.transform.SetParent(playerCurrentGuard.transform, worldPositionStays: false);
        }
    }
}
