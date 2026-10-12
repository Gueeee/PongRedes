using UnityEngine;
using Unity.Netcode;

public class GolBehaviour : MonoBehaviour
{
    [Tooltip("Coloque 0 para o golo que dá ponto ao Jogador 1, e 1 para o Jogador 2")]
    public int playerToScore;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Apenas o Servidor calcula colisões de golo e reinicia a bola
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;

        if (collision.CompareTag("Ball"))
        {
            // Adiciona o ponto no GameManager
            GameManager.Instance.AddScore(playerToScore);

            // Reinicia a bola
            BallBehaviour ballScript = collision.GetComponent<BallBehaviour>();
            if (ballScript != null)
            {
                ballScript.Launch();
            }
        }
    }
}