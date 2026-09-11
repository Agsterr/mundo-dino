using OpenWorldDinoSurvival.Systems;
using UnityEngine;

namespace OpenWorldDinoSurvival.Loot
{
    public class LootOnDeath : MonoBehaviour
    {
        [SerializeField] private LootDropEntry[] drops;

        public void SetDrops(LootDropEntry[] newDrops)
        {
            drops = newDrops;
        }

        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            if (_health != null)
            {
                _health.OnDied += HandleDied;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (drops == null)
            {
                return;
            }

            Vector3 origin = transform.position + Vector3.up * 0.5f;
            foreach (LootDropEntry drop in drops)
            {
                if (drop.amount <= 0)
                {
                    continue;
                }

                if (drop.chance < 1f && Random.value > drop.chance)
                {
                    continue;
                }

                LootSpawner.SpawnItem(drop.itemId, drop.amount, origin, drop.color);
            }
        }
    }

    [System.Serializable]
    public struct LootDropEntry
    {
        public int itemId;
        public int amount;
        [Range(0f, 1f)] public float chance;
        public Color color;
    }
}
