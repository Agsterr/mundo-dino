using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer
{
    /// <summary>
    /// Limita sessões a 4 jogadores e rejeita conexões extras.
    /// </summary>
    public class NetworkSessionConfig : MonoBehaviour
    {
        private void Start()
        {
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            NetworkManager.Singleton.ConnectionApprovalCallback = ApproveConnection;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback = null;
            }
        }

        private static void ApproveConnection(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            int connectedCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            bool hasRoom = connectedCount < PlayerSpawnPoints.MaxGroupSize;

            response.Approved = hasRoom;
            response.CreatePlayerObject = hasRoom;
            response.Pending = false;

            if (!hasRoom)
            {
                Debug.LogWarning("Conexão rejeitada: sessão cheia (máx. 4 jogadores).");
            }
        }
    }
}
