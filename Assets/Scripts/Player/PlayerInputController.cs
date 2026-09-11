using UnityEngine;
using UnityEngine.InputSystem;

namespace OpenWorldDinoSurvival.Player
{
    /// <summary>
    /// Conecta o Input System aos componentes do jogador.
    /// </summary>
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private ThirdPersonCamera cameraController;

        private void Reset()
        {
            movement = GetComponent<PlayerMovement>();
            cameraController = GetComponentInChildren<ThirdPersonCamera>();
        }

        public void OnMove(InputAction.CallbackContext context) => movement.OnMove(context);
        public void OnLook(InputAction.CallbackContext context) => cameraController.OnLook(context);
        public void OnSprint(InputAction.CallbackContext context) => movement.OnSprint(context);
        public void OnJump(InputAction.CallbackContext context) => movement.OnJump(context);
    }
}
