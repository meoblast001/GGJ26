using System.Collections;
using System.Linq;
using Extensions;
using UnityEngine;

namespace Console
{
    public class GuardManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        [Header("Managers")]
        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private Transform WaypointPool;
        [SerializeField] private PlayerHealth playerHealth;

        [Header("Configuration")]
        [SerializeField] private GuardCharacterController playerStartGuard;
        [SerializeField] private float guardSwitchMaxRadius;
        [SerializeField] private float guardSwitchDurationSeconds;

        private GuardCharacterController[] guardCharacterControllers;
        private GuardCharacterController playerCurrentGuard;

        public int PlayerGuardId => playerCurrentGuard.Id;

        void Awake()
        {
            guardCharacterControllers = GetComponentsInChildren<GuardCharacterController>();

            for (int i = 0; i < guardCharacterControllers.Length; ++i)
            {
                var guardCharacterController = guardCharacterControllers[i];
                guardCharacterController.Id = i + 1;
                foreach (var waypoint in guardCharacterController.PathWaypoints)
                    waypoint.SetParent(WaypointPool, worldPositionStays: true);
            }

            Debug.Log("Switching to first guard");
            DoSwitchGuard(playerStartGuard, centerCameraImmediately: true);

            playerInputManager.OnSwitchGuard += OnSwitchGuard;
        }

        void Start()
        {
            foreach (var guardCharacterController in guardCharacterControllers)
            {
                Debug.Log($"Guard {guardCharacterController.Id} has waypoints: {guardCharacterController.PathWaypoints.Length}");
                foreach (var waypoint in guardCharacterController.PathWaypoints)
                    waypoint.SetParent(WaypointPool, worldPositionStays: true);
            }
        }

        void Update()
        {
            playerCurrentGuard.MovementDirection = playerInputManager.WalkVector;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(playerStartGuard.transform.position, guardSwitchMaxRadius);
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

        private void DoSwitchGuard(GuardCharacterController newPlayerGuard, bool centerCameraImmediately = false)
        {
            if (playerCurrentGuard != null)
            {
                playerCurrentGuard.SetIsCurrentPlayer(false);
                playerCurrentGuard.MovementDirection = Vector2.zero;
            }

            playerCurrentGuard = newPlayerGuard;
            playerCurrentGuard.SetIsCurrentPlayer(true);

            playerHealth.RefillHealth();

            if (centerCameraImmediately)
                mainCamera.transform.SetParent(playerCurrentGuard.transform, worldPositionStays: false);
            else
            {
                mainCamera.transform.SetParent(playerCurrentGuard.transform, worldPositionStays: true);
                StartCoroutine(InterpolateCameraToOrigin(guardSwitchDurationSeconds));
            }
        }

        private IEnumerator InterpolateCameraToOrigin(float duration)
        {
            var cameraTransform = mainCamera.transform;
            var startLocalPosition = cameraTransform.localPosition;
            var elapsedSeconds = 0f;

            while (elapsedSeconds < duration)
            {
                elapsedSeconds += Time.deltaTime;
                cameraTransform.SetLocalPosition2D(Vector2.Lerp(startLocalPosition, Vector2.zero,
                    elapsedSeconds / duration));
                yield return null;
            }

            cameraTransform.SetLocalPosition2D(Vector2.zero);
        }
    }
}
