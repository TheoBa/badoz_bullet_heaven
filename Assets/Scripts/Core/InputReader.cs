using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BulletHeaven.Core
{
    public class InputReader : MonoBehaviour
    {
        public static InputReader Instance { get; private set; }

        public Vector2 MoveInput { get; private set; }

        public event Action OnPausePressed;

        private InputSystem_Actions _actions;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _actions = new InputSystem_Actions();
        }

        void OnEnable()
        {
            _actions.Player.Pause.performed += OnPause;
            _actions.Player.Enable();
        }

        void OnDisable()
        {
            MoveInput = Vector2.zero;
            _actions.Player.Pause.performed -= OnPause;
            _actions.Player.Disable();
        }

        void OnDestroy() => _actions?.Dispose();

        void Update() => MoveInput = _actions.Player.Move.ReadValue<Vector2>();

        private void OnPause(InputAction.CallbackContext ctx) => OnPausePressed?.Invoke();
    }
}
