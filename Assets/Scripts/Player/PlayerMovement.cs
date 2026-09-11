using UnityEngine;
using UnityEngine.InputSystem;

namespace OpenWorldDinoSurvival.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movimento")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.2f;

        [Header("Referências")]
        [SerializeField] private Transform cameraTransform;

        private CharacterController _controller;
        private PlayerWings _wings;
        private Vector2 _moveInput;
        private bool _sprintInput;
        private bool _jumpRequested;
        private float _verticalVelocity;

        public bool IsAirborne => _controller != null && !_controller.isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _wings = GetComponent<PlayerWings>();

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            _wings?.TickFuel(IsAirborne);
            ApplyGravity();
            Move();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            _sprintInput = context.ReadValueAsButton();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (_wings != null && _wings.IsFlying && IsAirborne)
            {
                if (context.started)
                {
                    _wings.SetAscendHeld(true);
                }
                else if (context.canceled)
                {
                    _wings.SetAscendHeld(false);
                }

                return;
            }

            if (context.performed && _controller.isGrounded)
            {
                _jumpRequested = true;
            }
        }

        private void Move()
        {
            Vector3 input = new Vector3(_moveInput.x, 0f, _moveInput.y);
            if (input.sqrMagnitude < 0.01f)
            {
                _controller.Move(new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
                return;
            }

            input.Normalize();

            float speed = _sprintInput ? sprintSpeed : walkSpeed;
            if (_wings != null && _wings.IsFlying && IsAirborne)
            {
                speed = _wings.FlySpeed;
            }
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * input.z + cameraRight * input.x;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 velocity = moveDirection * speed;
            velocity.y = _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            if (_jumpRequested)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _jumpRequested = false;
            }

            float gravityForce = gravity;
            if (_wings != null && _wings.IsFlying && IsAirborne)
            {
                gravityForce = _wings.GlideGravity;
                if (_wings.AscendHeld)
                {
                    _verticalVelocity = _wings.AscendSpeed;
                }
            }

            _verticalVelocity += gravityForce * Time.deltaTime;
        }
    }
}
