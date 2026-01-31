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

        [Header("Pathing")]
        [SerializeField] private Transform[] pathWaypoints;

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
        private int currentWaypointTargetIdx = 0;
        private Vector3? initialVectorToWaypoint = null;
        private GuardCharacterController targetCharacter = null;

        void FixedUpdate()
        {
            if (!isCurrentPlayer)
                DoMovementAI();
            characterRigidbody.linearVelocity = speed * Time.fixedDeltaTime * MovementDirection;
        }

        void OnDrawGizmosSelected()
        {
            if (pathWaypoints.Length == 0)
                return;
            Gizmos.color = Color.blue;
            var pathLineStrip = pathWaypoints.Where(transform => transform != null)
                .Select(transform => transform.position)
                .Append(pathWaypoints.First().transform.position)
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
                    Debug.Log($"ID = {Id}, dot = {Vector3.Dot(currentVectorToWaypoint, initialVectorToWaypoint.Value)}, v1={currentVectorToWaypoint}, v2={initialVectorToWaypoint.Value}");
                    if (Vector3.Dot(currentVectorToWaypoint, initialVectorToWaypoint.Value) <= 0)
                        currentWaypointTargetIdx = (currentWaypointTargetIdx + 1) % pathWaypoints.Length;
                    // Waypoint does not change, so return.
                    else
                        return;
                }

                var nextWaypoint = pathWaypoints[currentWaypointTargetIdx];
                initialVectorToWaypoint = (nextWaypoint.position - transform.position).normalized;
                Debug.Log($"Calculated initial waypoint vector: id = {Id}, vector = {initialVectorToWaypoint}");
                MovementDirection = new Vector2(initialVectorToWaypoint.Value.x, initialVectorToWaypoint.Value.y);
            }
        }

        private void EndWaypointFollowing()
        {
            Debug.Log($"End waypoint following on {Id}");
            initialVectorToWaypoint = null;
        }
    }
}
