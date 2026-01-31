using UnityEngine;

namespace Console
{
    public class GuardCharacterController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D characterRigidbody;
        [SerializeField] private float speed;

        public int Id { get; set; }
        public Vector2 MovementDirection { get; set; }

        void FixedUpdate()
        {
            characterRigidbody.linearVelocity = speed * Time.fixedDeltaTime * MovementDirection;
        }
    }
}
