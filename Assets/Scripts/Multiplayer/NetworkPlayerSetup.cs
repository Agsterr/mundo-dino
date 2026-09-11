using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Multiplayer.Chat;
using OpenWorldDinoSurvival.Multiplayer.Voice;
using OpenWorldDinoSurvival.Player;
using OpenWorldDinoSurvival.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OpenWorldDinoSurvival.Multiplayer
{
    /// <summary>
    /// Habilita input/câmera só para o dono e define spawn por jogador.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerSetup : NetworkBehaviour
    {
        [SerializeField] private Vector3 firstPlayerSpawn = new Vector3(0f, 1f, 0f);
        [SerializeField] private Vector3 secondPlayerSpawn = new Vector3(6f, 1f, 0f);

        [Header("Componentes do dono")]
        [SerializeField] private PlayerInputController inputController;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerWeaponController weaponController;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private ThirdPersonCamera thirdPersonCamera;
        [SerializeField] private WeaponHUD weaponHud;
        [SerializeField] private CraftingHUD craftingHud;
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private ChatHUD chatHud;
        [SerializeField] private VoiceCallHUD voiceCallHud;
        [SerializeField] private AbilitiesHUD abilitiesHud;

        private void Reset()
        {
            inputController = GetComponent<PlayerInputController>();
            movement = GetComponent<PlayerMovement>();
            weaponController = GetComponent<PlayerWeaponController>();
            playerInput = GetComponent<PlayerInput>();
            playerCamera = GetComponentInChildren<Camera>();
            audioListener = GetComponentInChildren<AudioListener>();
            thirdPersonCamera = GetComponentInChildren<ThirdPersonCamera>();
            weaponHud = GetComponent<WeaponHUD>();
            craftingHud = GetComponent<CraftingHUD>();
            interactor = GetComponent<PlayerInteractor>();
            chatHud = GetComponent<ChatHUD>();
            voiceCallHud = GetComponent<VoiceCallHUD>();
            abilitiesHud = GetComponent<AbilitiesHUD>();
        }

        public override void OnNetworkSpawn()
        {
            bool isOwner = IsOwner;

            if (inputController != null)
            {
                inputController.enabled = isOwner;
            }

            if (movement != null)
            {
                movement.enabled = isOwner;
            }

            if (weaponController != null)
            {
                weaponController.enabled = isOwner;
            }

            if (playerInput != null)
            {
                playerInput.enabled = isOwner;
            }

            if (playerCamera != null)
            {
                playerCamera.enabled = isOwner;
            }

            if (audioListener != null)
            {
                audioListener.enabled = isOwner;
            }

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = isOwner;
            }

            if (weaponHud != null)
            {
                weaponHud.enabled = isOwner;
            }

            if (craftingHud != null)
            {
                craftingHud.enabled = isOwner;
            }

            if (interactor != null)
            {
                interactor.enabled = isOwner;
            }

            if (chatHud != null)
            {
                chatHud.enabled = isOwner;
            }

            if (voiceCallHud != null)
            {
                voiceCallHud.enabled = isOwner;
            }

            if (abilitiesHud != null)
            {
                abilitiesHud.enabled = isOwner;
            }

            if (isOwner)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (IsServer)
            {
                Vector3 spawn = OwnerClientId == 0 ? firstPlayerSpawn : secondPlayerSpawn;
                transform.position = spawn;
            }
        }
    }
}
