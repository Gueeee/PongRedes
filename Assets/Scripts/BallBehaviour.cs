using UnityEngine;
using Unity.Netcode;

public class BallBehaviour : NetworkBehaviour
{
    public float speed = 8f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // A bola só deve ser lançada pelo Servidor quando este a cria
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Launch();
        }
    }

    public void Launch() {
        // Garante que só o servidor pode chamar esta função
        if (!IsServer) return; 

        transform.position = Vector2.zero;
        
        float x = Random.value < 0.5f ? -1f : 1f;
        float y = Random.Range(-0.8f, 0.8f);
        
        Vector2 direction = new Vector2(x, y).normalized;
        rb.linearVelocity = direction * speed;
    }
}