using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Multiplayer.Chat;
using OpenWorldDinoSurvival.Multiplayer.Voice;
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
        [SerializeField] private ChatHUD chatHud;
        [SerializeField] private VoiceCallHUD voiceCallHud;

        private void Reset()
        {
            movement = GetComponent<PlayerMovement>();
            cameraController = GetComponentInChildren<ThirdPersonCamera>();
            weaponController = GetComponent<PlayerWeaponController>();
            interactor = GetComponent<PlayerInteractor>();
            craftingHud = GetComponent<CraftingHUD>();
            chatHud = GetComponent<ChatHUD>();
            voiceCallHud = GetComponent<VoiceCallHUD>();
        }

        private void Awake()
        {
            interactor ??= GetComponent<PlayerInteractor>();
            craftingHud ??= GetComponent<CraftingHUD>();
            chatHud ??= GetComponent<ChatHUD>();
            voiceCallHud ??= GetComponent<VoiceCallHUD>();
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

        public void OnChat(InputAction.CallbackContext context)
        {
            if (!context.performed || chatHud == null)
            {
                return;
            }

            chatHud.ToggleChatFocus();
        }

        public void OnSubmitChat(InputAction.CallbackContext context)
        {
            if (!context.performed || chatHud == null)
            {
                return;
            }

            chatHud.SendCurrentInput();
        }

        public void OnVoiceCall(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                voiceCallHud?.ToggleCallToDefaultTarget();
            }
        }

        public void OnPushToTalk(InputAction.CallbackContext context)
        {
            if (voiceCallHud == null)
            {
                return;
            }

            if (context.started)
            {
                voiceCallHud.SetPushToTalk(true);
            }
            else if (context.canceled)
            {
                voiceCallHud.SetPushToTalk(false);
            }
        }
    }
}
