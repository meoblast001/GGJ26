using System;
using UnityEngine;

namespace Console
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float healthReductionPerSecond;

        public float Health { get; private set; } = 1f;

        public event Action OnHealthZero;

        private bool hasLost = false;

        void Update()
        {
            if (!hasLost)
            {
                Health -= Time.deltaTime * healthReductionPerSecond;
                if (Health <= 0)
                {
                    Health = 0;
                    hasLost = true;
                    OnHealthZero?.Invoke();
                }
            }
        }

        public void RefillHealth()
        {
            Health = 1f;
        }
    }
}
