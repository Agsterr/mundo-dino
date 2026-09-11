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
        [SerializeField] private PlayerWings wings;
        [SerializeField] private PlayerMeleeCombat meleeCombat;
        [SerializeField] private PlayerDinosaurDomination domination;
        [SerializeField] private NetworkPlayerInventory networkInventory;

        private bool _sprintHeld;
        private Vector2 _lastMoveInput;

        private void Reset()
        {
            movement = GetComponent<PlayerMovement>();
            cameraController = GetComponentInChildren<ThirdPersonCamera>();
            weaponController = GetComponent<PlayerWeaponController>();
            interactor = GetComponent<PlayerInteractor>();
            craftingHud = GetComponent<CraftingHUD>();
            chatHud = GetComponent<ChatHUD>();
            voiceCallHud = GetComponent<VoiceCallHUD>();
            wings = GetComponent<PlayerWings>();
            meleeCombat = GetComponent<PlayerMeleeCombat>();
            domination = GetComponent<PlayerDinosaurDomination>();
            networkInventory = GetComponent<NetworkPlayerInventory>();
        }

        private void Awake()
        {
            interactor ??= GetComponent<PlayerInteractor>();
            craftingHud ??= GetComponent<CraftingHUD>();
            chatHud ??= GetComponent<ChatHUD>();
            voiceCallHud ??= GetComponent<VoiceCallHUD>();
            wings ??= GetComponent<PlayerWings>();
            meleeCombat ??= GetComponent<PlayerMeleeCombat>();
            domination ??= GetComponent<PlayerDinosaurDomination>();
            networkInventory ??= GetComponent<NetworkPlayerInventory>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _lastMoveInput = context.ReadValue<Vector2>();
            if (domination != null && domination.IsDominating)
            {
                domination.RelayDinoMove(_lastMoveInput, _sprintHeld);
                return;
            }

            movement.OnMove(context);
        }

        public void OnLook(InputAction.CallbackContext context) => cameraController.OnLook(context);

        public void OnSprint(InputAction.CallbackContext context)
        {
            _sprintHeld = context.ReadValueAsButton();
            if (domination != null && domination.IsDominating)
            {
                domination.RelayDinoMove(_lastMoveInput, _sprintHeld);
                return;
            }

            movement.OnSprint(context);
        }

        public void OnJump(InputAction.CallbackContext context) => movement.OnJump(context);

        public void OnFire(InputAction.CallbackContext context)
        {
            if (domination != null && domination.IsDominating)
            {
                if (context.performed)
                {
                    domination.RelayDinoAttack();
                }

                return;
            }

            weaponController.OnFire(context);
        }

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

        public void OnToggleWings(InputAction.CallbackContext context)
        {
            if (context.performed && wings != null && (domination == null || !domination.IsDominating))
            {
                wings.ToggleWings();
            }
        }

        public void OnLightAttack(InputAction.CallbackContext context)
        {
            if (!context.performed || meleeCombat == null || (domination != null && domination.IsDominating))
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
            {
                meleeCombat.TryHeavyAttack();
                return;
            }

            meleeCombat.TryLightAttack();
        }

        public void OnHeavyAttack(InputAction.CallbackContext context)
        {
            if (context.performed && meleeCombat != null && (domination == null || !domination.IsDominating))
            {
                meleeCombat.TryHeavyAttack();
            }
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (context.performed && meleeCombat != null && (domination == null || !domination.IsDominating))
            {
                meleeCombat.TryDodge();
            }
        }

        public void OnDominate(InputAction.CallbackContext context)
        {
            if (context.performed && domination != null)
            {
                domination.TryToggleDomination();
            }
        }

        public void OnConsumeFood(InputAction.CallbackContext context)
        {
            if (context.performed && networkInventory != null)
            {
                networkInventory.TryConsumeItem(ItemIds.FoodRation);
            }
        }

        public void OnConsumeWater(InputAction.CallbackContext context)
        {
            if (context.performed && networkInventory != null)
            {
                networkInventory.TryConsumeItem(ItemIds.WaterFlask);
            }
        }
    }
}
