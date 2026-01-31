using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static void SetLocalPosition2D(this Transform transform, Vector2 position)
        {
            transform.localPosition = new Vector3(position.x, position.y, transform.localPosition.z);
        }
    }
}
