using System;
using UnityEngine;

namespace Console
{
    public class ExitDoorController : MonoBehaviour
    {
        public event Action<int> OnGuardReachesExit;

        void OnTriggerEnter2D(Collider2D collision)
        {
            var guardCharacter = collision.gameObject.GetComponent<GuardCharacterController>();
            if (guardCharacter == null)
                return;
            
            OnGuardReachesExit?.Invoke(guardCharacter.Id);
        }
    }
}
