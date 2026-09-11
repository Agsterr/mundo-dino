using OpenWorldDinoSurvival.Systems;
using UnityEngine;

namespace OpenWorldDinoSurvival.World
{
    /// <summary>
    /// Cria alvos de treino na cena ao iniciar (Milestone 2).
    /// </summary>
    public class TrainingRangeSetup : MonoBehaviour
    {
        [SerializeField] private Vector3[] targetPositions =
        {
            new Vector3(-8f, 1f, 20f),
            new Vector3(0f, 1f, 25f),
            new Vector3(8f, 1f, 20f),
        };

        [SerializeField] private float targetHealth = 100f;
        [SerializeField] private Color targetColor = new Color(0.8f, 0.2f, 0.2f);

        private void Start()
        {
            for (int i = 0; i < targetPositions.Length; i++)
            {
                CreateTarget($"Target_{i + 1}", targetPositions[i]);
            }
        }

        private void CreateTarget(string name, Vector3 position)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = name;
            target.transform.position = position;
            target.transform.localScale = new Vector3(1.5f, 2f, 0.5f);

            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = targetColor;
            }

            Health health = target.AddComponent<Health>();
            health.Initialize(targetHealth);
        }
    }
}
