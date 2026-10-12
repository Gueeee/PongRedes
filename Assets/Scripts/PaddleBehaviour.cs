using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PaddleBehaviour : NetworkBehaviour {
    public float speed = 8f;
    public float yBound = 3.8f; 
    
    private Rigidbody2D rb;

    public NetworkVariable<float> netInputY = new NetworkVariable<float>( 0f,  NetworkVariableReadPermission.Everyone,  NetworkVariableWritePermission.Owner);

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            if (OwnerClientId == 0) transform.position = new Vector2(-7f, 0f); 
            else transform.position = new Vector2(7f, 0f); 
        }
    }

    void Update() {
        if (!IsOwner) return;

        float moveY = 0f;

        if (Keyboard.current != null) {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;
        }

        netInputY.Value = moveY;
    }

    void FixedUpdate() {
        if (IsOwner) {
            Vector2 novaPosicaoDono = rb.position + new Vector2(0, netInputY.Value * speed * Time.fixedDeltaTime);
            novaPosicaoDono.y = Mathf.Clamp(novaPosicaoDono.y, -yBound, yBound);
            rb.MovePosition(novaPosicaoDono);
        }

        if (IsServer && !IsOwner) {
            Vector2 novaPosicaoServidor = rb.position + new Vector2(0, netInputY.Value * speed * Time.fixedDeltaTime);
            novaPosicaoServidor.y = Mathf.Clamp(novaPosicaoServidor.y, -yBound, yBound);
            rb.MovePosition(novaPosicaoServidor);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Ball")) {
            float hitPoint = collision.transform.position.y - transform.position.y;
            float paddleHeight = GetComponent<Collider2D>().bounds.size.y;
            float y = (hitPoint / paddleHeight) * 2f; 
            float x = transform.position.x < 0 ? 1f : -1f;

            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            BallBehaviour ballScript = collision.gameObject.GetComponent<BallBehaviour>();
            
            if (ballRb != null && ballScript != null) {
                ballRb.linearVelocity = new Vector2(x, y).normalized * ballScript.speed;
            }
        }
    }
}