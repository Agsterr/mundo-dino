using OpenWorldDinoSurvival.Systems;
using UnityEngine;

namespace OpenWorldDinoSurvival.World
{
    /// <summary>
    /// Fonte de água: beber restaura sede (Milestone 7).
    /// </summary>
    public class WaterSource : MonoBehaviour
    {
        [SerializeField] private float thirstRestore = 35f;
        [SerializeField] private float drinkCooldown = 2f;

        private float _nextDrinkTime;

        public bool CanDrink => Time.time >= _nextDrinkTime;

        public bool TryDrink(NetworkPlayerSurvival survival)
        {
            if (!CanDrink || survival == null)
            {
                return false;
            }

            survival.RestoreThirst(thirstRestore);
            _nextDrinkTime = Time.time + drinkCooldown;
            return true;
        }
    }
}
