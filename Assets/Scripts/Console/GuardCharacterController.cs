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

        void FixedUpdate()
        {
            characterRigidbody.linearVelocity = speed * Time.fixedDeltaTime * MovementDirection;
        }

        public void SetIsCurrentPlayer(bool isCurrentPlayer)
        {
            spriteRenderer.sprite = isCurrentPlayer ? maskedSprite : unmaskedSprite;
        }
    }
}
