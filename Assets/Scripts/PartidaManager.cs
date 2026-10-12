using UnityEngine;
using Unity.Netcode;

public class PartidaManager : NetworkBehaviour
{
    public GameObject ballPrefab;

    public override void OnNetworkSpawn()
    {
        // Apenas o servidor gere a lógica de início de partida
        if (!IsServer) return;

        // Se por algum motivo já houver 2 jogadores assim que a cena carregar
        if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2)
        {
            SpawnBall();
        }
        else
        {
            // Se não, o servidor subscreve o evento para ser avisado sempre que alguém entrar
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Verifica se chegámos aos 2 jogadores
        if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2)
        {
            SpawnBall();
            
            // Muito importante: Cancela a subscrição do evento para não criar mais 
            // bolas caso entrem mais pessoas na sala (espectadores, por exemplo)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void SpawnBall()
    {
        GameObject ball = Instantiate(ballPrefab);
        ball.GetComponent<NetworkObject>().Spawn();
    }

    public override void OnNetworkDespawn()
    {
        // Limpeza de segurança: se o servidor for desligado, removemos o evento 
        // para evitar vazamentos de memória (memory leaks)
        if (IsServer)
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
    }
}