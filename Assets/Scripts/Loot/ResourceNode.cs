using OpenWorldDinoSurvival.Inventory;
using UnityEngine;

namespace OpenWorldDinoSurvival.Loot
{
    public class ResourceNode : MonoBehaviour
    {
        [SerializeField] private int itemId = ItemIds.Fiber;
        [SerializeField] private int harvestAmount = 2;
        [SerializeField] private float respawnTime = 20f;
        [SerializeField] private Color activeColor = new Color(0.2f, 0.7f, 0.25f);
        [SerializeField] private Color depletedColor = new Color(0.35f, 0.35f, 0.35f);

        private Renderer _renderer;
        private bool _depleted;
        private float _respawnTimer;

        public void Configure(int newItemId, int newAmount, Color color, float respawnSeconds = 20f)
        {
            itemId = newItemId;
            harvestAmount = newAmount;
            activeColor = color;
            respawnTime = respawnSeconds;
            ApplyColor(activeColor);
        }

        public bool CanHarvest => !_depleted;
        public int ItemId => itemId;
        public int HarvestAmount => harvestAmount;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            ApplyColor(activeColor);
        }

        private void Update()
        {
            if (!_depleted)
            {
                return;
            }

            _respawnTimer -= Time.deltaTime;
            if (_respawnTimer <= 0f)
            {
                _depleted = false;
                ApplyColor(activeColor);
            }
        }

        public bool TryHarvest()
        {
            if (_depleted)
            {
                return false;
            }

            _depleted = true;
            _respawnTimer = respawnTime;
            ApplyColor(depletedColor);
            return true;
        }

        private void ApplyColor(Color color)
        {
            if (_renderer != null)
            {
                _renderer.material.color = color;
            }
        }
    }
}
