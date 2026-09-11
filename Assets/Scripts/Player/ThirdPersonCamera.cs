using UnityEngine;
using UnityEngine.InputSystem;

namespace OpenWorldDinoSurvival.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Alvo")]
        [SerializeField] private Transform target;

        [Header("Distância")]
        [SerializeField] private float distance = 4f;
        [SerializeField] private float height = 1.6f;

        [Header("Rotação")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -20f;
        [SerializeField] private float maxVerticalAngle = 60f;

        private float _yaw;
        private float _pitch = 10f;
        private Vector2 _lookInput;

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            _yaw += _lookInput.x * mouseSensitivity;
            _pitch -= _lookInput.y * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, minVerticalAngle, maxVerticalAngle);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 offset = rotation * new Vector3(0f, height, -distance);
            transform.position = target.position + offset;
            transform.rotation = rotation;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
    }
}
