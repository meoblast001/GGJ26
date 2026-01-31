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

        void FixedUpdate()
        {
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
            spriteRenderer.sprite = isCurrentPlayer ? maskedSprite : unmaskedSprite;
        }
    }
}
