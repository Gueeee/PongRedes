using UnityEngine;
using UnityEngine.InputSystem; // Continuamos a usar o novo sistema
using Unity.Netcode;

public class PaddleBehaviour : NetworkBehaviour 
{
    public float speed = 8f;
    public float yBound = 3.8f; 
    
    private Rigidbody2D rb;

    public NetworkVariable<float> netInputY = new NetworkVariable<float>(
        0f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner 
    );

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        // Removemos a necessidade de desativar o PlayerInput, pois não o vamos usar para já
        if (IsServer)
        {
            if (OwnerClientId == 0) transform.position = new Vector2(-7f, 0f); 
            else transform.position = new Vector2(7f, 0f); 
        }
    }

    void Update()
    {
        // Apenas o Dono (seja na janela do Host ou do Client) lê o seu próprio teclado
        if (!IsOwner) return;

        float moveY = 0f;

        // Força a leitura direta das teclas W/S ou Setas do teclado no Novo Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveY = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveY = -1f;
        }

        // Atualiza a variável de rede (que será enviada para o Servidor)
        netInputY.Value = moveY;
    }

    void FixedUpdate() {
        // 1. PREVISÃO DO CLIENTE: O dono da raquete (seja Host ou Client)
        // move o seu próprio objeto IMEDIATAMENTE no seu ecrã.
        if (IsOwner)
        {
            Vector2 novaPosicaoDono = rb.position + new Vector2(0, netInputY.Value * speed * Time.fixedDeltaTime);
            novaPosicaoDono.y = Mathf.Clamp(novaPosicaoDono.y, -yBound, yBound);
            rb.MovePosition(novaPosicaoDono);
        }

        // 2. O Servidor processa o movimento para a réplica do outro jogador
        if (IsServer && !IsOwner)
        {
            Vector2 novaPosicaoServidor = rb.position + new Vector2(0, netInputY.Value * speed * Time.fixedDeltaTime);
            novaPosicaoServidor.y = Mathf.Clamp(novaPosicaoServidor.y, -yBound, yBound);
            rb.MovePosition(novaPosicaoServidor);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            float hitPoint = collision.transform.position.y - transform.position.y;
            float paddleHeight = GetComponent<Collider2D>().bounds.size.y;
            float y = (hitPoint / paddleHeight) * 2f; 
            float x = transform.position.x < 0 ? 1f : -1f;

            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            BallBehaviour ballScript = collision.gameObject.GetComponent<BallBehaviour>();
            
            if (ballRb != null && ballScript != null)
            {
                ballRb.linearVelocity = new Vector2(x, y).normalized * ballScript.speed;
            }
        }
    }
}