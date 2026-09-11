using UnityEngine;

namespace OpenWorldDinoSurvival.Player
{
    /// <summary>
    /// Asas: voo e planar com combustível limitado.
    /// </summary>
    public class PlayerWings : MonoBehaviour
    {
        [SerializeField] private float maxFuel = 100f;
        [SerializeField] private float fuelDrainPerSecond = 12f;
        [SerializeField] private float fuelRechargePerSecond = 18f;
        [SerializeField] private float flySpeed = 10f;
        [SerializeField] private float ascendSpeed = 6f;
        [SerializeField] private float glideGravity = -4f;

        private bool _wingsDeployed;
        private float _currentFuel;
        private GameObject _leftWing;
        private GameObject _rightWing;
        private PlayerMovement _movement;

        public bool WingsDeployed => _wingsDeployed;
        public bool IsFlying => _wingsDeployed && _currentFuel > 0f;
        public float CurrentFuel => _currentFuel;
        public float MaxFuel => maxFuel;
        public float GlideGravity => glideGravity;
        public float FlySpeed => flySpeed;
        public float AscendSpeed => ascendSpeed;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _currentFuel = maxFuel;
            CreateWingVisuals();
        }

        public void ToggleWings()
        {
            _wingsDeployed = !_wingsDeployed;
            UpdateWingVisuals();
        }

        public void SetAscendHeld(bool held)
        {
            AscendHeld = held;
        }

        public bool AscendHeld { get; private set; }

        public void TickFuel(bool isAirborne)
        {
            if (_wingsDeployed && isAirborne && _currentFuel > 0f)
            {
                _currentFuel = Mathf.Max(0f, _currentFuel - fuelDrainPerSecond * Time.deltaTime);
                if (_currentFuel <= 0f)
                {
                    _wingsDeployed = false;
                    UpdateWingVisuals();
                }

                return;
            }

            if (!_wingsDeployed && _currentFuel < maxFuel)
            {
                _currentFuel = Mathf.Min(maxFuel, _currentFuel + fuelRechargePerSecond * Time.deltaTime);
            }
        }

        private void CreateWingVisuals()
        {
            _leftWing = CreateWing("LeftWing", new Vector3(-0.8f, 1.3f, 0f), new Vector3(0f, 0f, 25f));
            _rightWing = CreateWing("RightWing", new Vector3(0.8f, 1.3f, 0f), new Vector3(0f, 0f, -25f));
            UpdateWingVisuals();
        }

        private GameObject CreateWing(string wingName, Vector3 localPosition, Vector3 localEuler)
        {
            GameObject wing = GameObject.CreatePrimitive(PrimitiveType.Quad);
            wing.name = wingName;
            wing.transform.SetParent(transform);
            wing.transform.localPosition = localPosition;
            wing.transform.localRotation = Quaternion.Euler(localEuler);
            wing.transform.localScale = new Vector3(1.2f, 0.5f, 1f);
            Destroy(wing.GetComponent<Collider>());
            Renderer renderer = wing.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.75f, 0.85f, 1f, 0.85f);
            }

            return wing;
        }

        private void UpdateWingVisuals()
        {
            if (_leftWing != null)
            {
                _leftWing.SetActive(_wingsDeployed);
            }

            if (_rightWing != null)
            {
                _rightWing.SetActive(_wingsDeployed);
            }
        }
    }
}
