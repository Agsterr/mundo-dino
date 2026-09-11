using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.World;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Systems
{
    /// <summary>
    /// Fome e sede autoritativas no servidor (Milestone 7).
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerSurvival : NetworkBehaviour
    {
        [SerializeField] private float hungerDrainPerSecond = 0.45f;
        [SerializeField] private float thirstDrainPerSecond = 0.65f;
        [SerializeField] private float starvationDamagePerSecond = 8f;

        private readonly NetworkVariable<float> _hunger = new(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<float> _thirst = new(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private NetworkPlayerHealth _health;
        private float _nextStarvationTick;

        public float Hunger => _hunger.Value;
        public float Thirst => _thirst.Value;
        public bool IsStarving => _hunger.Value <= 0f;
        public bool IsDehydrated => _thirst.Value <= 0f;

        private void Awake()
        {
            _health = GetComponent<NetworkPlayerHealth>();
        }

        private void Update()
        {
            if (!IsServer || _health == null || !_health.IsAlive)
            {
                return;
            }

            DrainNeeds(Time.deltaTime);
            ApplyStarvationDamage();
        }

        public void RestoreHunger(float amount)
        {
            if (!IsServer || amount <= 0f)
            {
                return;
            }

            _hunger.Value = Mathf.Min(100f, _hunger.Value + amount);
        }

        public void RestoreThirst(float amount)
        {
            if (!IsServer || amount <= 0f)
            {
                return;
            }

            _thirst.Value = Mathf.Min(100f, _thirst.Value + amount);
        }

        public void ResetNeeds()
        {
            if (!IsServer)
            {
                return;
            }

            _hunger.Value = 100f;
            _thirst.Value = 100f;
        }

        [ServerRpc]
        public void RequestDrinkWaterServerRpc(ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            WaterSource source = FindClosestWaterSource();
            source?.TryDrink(this);
        }

        private WaterSource FindClosestWaterSource()
        {
            WaterSource closest = null;
            float bestDistance = 2.5f;
            WaterSource[] sources = FindObjectsByType<WaterSource>(FindObjectsSortMode.None);

            foreach (WaterSource source in sources)
            {
                float distance = Vector3.Distance(transform.position, source.transform.position);
                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    closest = source;
                }
            }

            return closest;
        }

        private void DrainNeeds(float deltaTime)
        {
            _hunger.Value = Mathf.Max(0f, _hunger.Value - hungerDrainPerSecond * deltaTime);
            _thirst.Value = Mathf.Max(0f, _thirst.Value - thirstDrainPerSecond * deltaTime);
        }

        private void ApplyStarvationDamage()
        {
            if (!IsStarving && !IsDehydrated)
            {
                return;
            }

            if (Time.time < _nextStarvationTick)
            {
                return;
            }

            _nextStarvationTick = Time.time + 1f;
            _health.TakeDamage(starvationDamagePerSecond);
        }
    }
}
