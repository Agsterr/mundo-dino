using OpenWorldDinoSurvival.Dinosaurs;
using OpenWorldDinoSurvival.Player;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerAbilities : NetworkBehaviour
    {
        [SerializeField] private PlayerMeleeCombat meleeCombat;
        [SerializeField] private PlayerDinosaurDomination domination;
        [SerializeField] private NetworkPlayerHealth health;

        private NetworkObject _dominatedDino;

        private void Awake()
        {
            meleeCombat ??= GetComponent<PlayerMeleeCombat>();
            domination ??= GetComponent<PlayerDinosaurDomination>();
            health ??= GetComponent<NetworkPlayerHealth>();
        }

        [ServerRpc]
        public void RequestMeleeAttackServerRpc(bool heavy, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || meleeCombat == null || domination.IsDominating)
            {
                return;
            }

            float damage = heavy ? 40f : 18f;
            meleeCombat.PerformMeleeAttack(damage);
        }

        [ServerRpc]
        public void RequestDodgeServerRpc(Vector3 direction, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || meleeCombat == null)
            {
                return;
            }

            meleeCombat.ApplyDodge(direction.normalized);
            ApplyDodgeClientRpc(direction, BuildOwnerRpcParams());
        }

        [ClientRpc]
        private void ApplyDodgeClientRpc(Vector3 direction, ClientRpcParams rpcParams = default)
        {
            meleeCombat?.ApplyDodge(direction);
        }

        [ServerRpc]
        public void RequestDominateServerRpc(ulong dinosaurNetworkId, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || _dominatedDino != null)
            {
                return;
            }

            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(dinosaurNetworkId, out NetworkObject dinoObject))
            {
                return;
            }

            DinosaurMountable mountable = dinoObject.GetComponent<DinosaurMountable>();
            if (mountable == null || !mountable.CanBeDominatedBy(transform))
            {
                return;
            }

            mountable.TryDominate(OwnerClientId, transform);
            _dominatedDino = dinoObject;
            ConfirmDominationClientRpc(dinosaurNetworkId, BuildOwnerRpcParams());
        }

        [ServerRpc]
        public void RequestReleaseDominationServerRpc(ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            ReleaseDominationServer();
        }

        [ClientRpc]
        private void ConfirmDominationClientRpc(ulong dinosaurNetworkId, ClientRpcParams rpcParams = default)
        {
            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(dinosaurNetworkId, out NetworkObject dinoObject))
            {
                return;
            }

            DinosaurMountable mountable = dinoObject.GetComponent<DinosaurMountable>();
            domination?.BeginDomination(mountable);
        }

        [ClientRpc]
        private void ConfirmReleaseDominationClientRpc(ClientRpcParams rpcParams = default)
        {
            domination?.EndDomination();
        }

        public void ReleaseDominationServer()
        {
            if (_dominatedDino != null)
            {
                DinosaurMountable mountable = _dominatedDino.GetComponent<DinosaurMountable>();
                mountable?.ReleasePossession();
                _dominatedDino = null;
            }

            ConfirmReleaseDominationClientRpc(BuildOwnerRpcParams());
        }

        private ClientRpcParams BuildOwnerRpcParams()
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { OwnerClientId }
                }
            };
        }

        [ServerRpc]
        public void RelayDinoInputServerRpc(Vector2 move, bool sprint, bool attack, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || _dominatedDino == null)
            {
                return;
            }

            DinosaurMountable mountable = _dominatedDino.GetComponent<DinosaurMountable>();
            mountable?.ApplyControlInput(move, sprint, attack);
        }

        public bool IsInvulnerable()
        {
            return meleeCombat != null && meleeCombat.IsInvulnerable;
        }
    }
}
