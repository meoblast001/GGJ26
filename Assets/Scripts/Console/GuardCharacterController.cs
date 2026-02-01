using System.Linq;
using UnityEngine;

namespace Console
{
    public class GuardCharacterController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D characterRigidbody;
        [SerializeField] private float speed;

        [Header("Sprites")]
        [SerializeField] private Sprite unmaskedSprite;
        [SerializeField] private Sprite maskedSprite;

        public int Id { get; set; }
        public Vector2 MovementDirection { get; set; }
        public Transform[] PathWaypoints => pathWaypoints;

        private bool IsCurrentPlayer
        {
            set
            {
                isCurrentPlayer = value;
                if (!value)
                    EndWaypointFollowing();
            }
        }

        private bool isCurrentPlayer  = false;
        private Transform[] pathWaypoints;
        private int currentWaypointTargetIdx = 0;
        private Vector3? initialVectorToWaypoint = null;
        private GuardCharacterController targetCharacter = null;

        void Awake()
        {
            pathWaypoints = GetComponentsInChildren<GuardWaypoint>().Select(waypoint => waypoint.transform).ToArray();
        }

        void FixedUpdate()
        {
            if (!isCurrentPlayer)
                DoMovementAI();
            characterRigidbody.linearVelocity = speed * Time.fixedDeltaTime * MovementDirection;
        }

        void OnDrawGizmosSelected()
        {
            var waypoints = GetComponentsInChildren<GuardWaypoint>().Select(waypoint => waypoint.transform).ToArray();
            if (waypoints.Length == 0)
                return;
            Gizmos.color = Color.blue;
            var pathLineStrip = waypoints.Where(transform => transform != null)
                .Select(transform => transform.position)
                .Append(waypoints.First().transform.position)
                .ToArray();
            Gizmos.DrawLineStrip(pathLineStrip, false);
        }

        public void SetIsCurrentPlayer(bool isCurrentPlayer)
        {
            IsCurrentPlayer = isCurrentPlayer;
            spriteRenderer.sprite = isCurrentPlayer ? maskedSprite : unmaskedSprite;
        }

        private void DoMovementAI()
        {
            if (targetCharacter != null)
            {
                EndWaypointFollowing();
                MovementDirection = (targetCharacter.transform.position - transform.position).normalized;
            }
            else
            {
                if (initialVectorToWaypoint.HasValue)
                {
                    var waypoint = pathWaypoints[currentWaypointTargetIdx];
                    var currentVectorToWaypoint = (waypoint.position - transform.position).normalized;
                    // Indicates that the guard has reached or passed the waypoint.
                    if (Vector3.Dot(currentVectorToWaypoint, initialVectorToWaypoint.Value) <= 0)
                    {
                        if (Id == 3)
                        {
                            Debug.Log($"Switching to {(currentWaypointTargetIdx + 1) % pathWaypoints.Length}");
                        }
                        currentWaypointTargetIdx = (currentWaypointTargetIdx + 1) % pathWaypoints.Length;
                    }
                    // Waypoint does not change, so return.
                    else
                        return;
                }

                var nextWaypoint = pathWaypoints[currentWaypointTargetIdx];
                initialVectorToWaypoint = (nextWaypoint.position - transform.position).normalized;
                MovementDirection = new Vector2(initialVectorToWaypoint.Value.x, initialVectorToWaypoint.Value.y);
            }
        }

        private void EndWaypointFollowing()
        {
            initialVectorToWaypoint = null;
        }
    }
}
