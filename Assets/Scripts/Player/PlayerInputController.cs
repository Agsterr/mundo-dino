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
        [SerializeField] private PlayerWeaponController weaponController;

        private void Reset()
        {
            movement = GetComponent<PlayerMovement>();
            cameraController = GetComponentInChildren<ThirdPersonCamera>();
            weaponController = GetComponent<PlayerWeaponController>();
        }

        public void OnMove(InputAction.CallbackContext context) => movement.OnMove(context);
        public void OnLook(InputAction.CallbackContext context) => cameraController.OnLook(context);
        public void OnSprint(InputAction.CallbackContext context) => movement.OnSprint(context);
        public void OnJump(InputAction.CallbackContext context) => movement.OnJump(context);
        public void OnFire(InputAction.CallbackContext context) => weaponController.OnFire(context);
        public void OnReload(InputAction.CallbackContext context) => weaponController.OnReload(context);
        public void OnSwitchPistol(InputAction.CallbackContext context) => weaponController.OnSwitchPistol(context);
        public void OnSwitchRifle(InputAction.CallbackContext context) => weaponController.OnSwitchRifle(context);
    }
}
