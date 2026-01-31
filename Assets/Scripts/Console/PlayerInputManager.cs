using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Console
{
    public class PlayerInputManager : MonoBehaviour
    {
        private ConsoleInputActions inputActions;

        public Vector2 WalkVector { get; private set; } = Vector2.zero;
        public event Action OnSwitchGuard;

        void Awake()
        {
            inputActions = new ConsoleInputActions();

            inputActions.Player.Walk.performed += OnWalkPerformed;
            inputActions.Player.Walk.canceled += ctx => WalkVector = Vector2.zero;
            inputActions.Player.SwitchGuard.performed += OnSwitchGuardPerformed;

            inputActions.Player.Enable();
        }

        public void Disable()
        {
            inputActions.Player.Disable();
        }

        private void OnWalkPerformed(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            WalkVector = movement;
        }

        private void OnSwitchGuardPerformed(InputAction.CallbackContext context)
        {
            OnSwitchGuard?.Invoke();
        }
    }
}
