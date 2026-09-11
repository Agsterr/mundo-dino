using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.UI;
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
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private CraftingHUD craftingHud;

        private void Reset()
        {
            movement = GetComponent<PlayerMovement>();
            cameraController = GetComponentInChildren<ThirdPersonCamera>();
            weaponController = GetComponent<PlayerWeaponController>();
            interactor = GetComponent<PlayerInteractor>();
            craftingHud = GetComponent<CraftingHUD>();
        }

        private void Awake()
        {
            interactor ??= GetComponent<PlayerInteractor>();
            craftingHud ??= GetComponent<CraftingHUD>();
        }

        public void OnMove(InputAction.CallbackContext context) => movement.OnMove(context);
        public void OnLook(InputAction.CallbackContext context) => cameraController.OnLook(context);
        public void OnSprint(InputAction.CallbackContext context) => movement.OnSprint(context);
        public void OnJump(InputAction.CallbackContext context) => movement.OnJump(context);
        public void OnFire(InputAction.CallbackContext context) => weaponController.OnFire(context);
        public void OnReload(InputAction.CallbackContext context) => weaponController.OnReload(context);
        public void OnSwitchPistol(InputAction.CallbackContext context) => weaponController.OnSwitchPistol(context);
        public void OnSwitchRifle(InputAction.CallbackContext context) => weaponController.OnSwitchRifle(context);

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                interactor?.TryInteract();
            }
        }

        public void OnCraftMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                craftingHud?.ToggleMenu();
            }
        }
    }
}
