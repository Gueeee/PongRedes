using UnityEngine;
using Unity.Netcode;

public class BallBehaviour : NetworkBehaviour {
    public float speed = 8f;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            Launch();
        }
    }

    public void Launch() {
        if (!IsServer) return; 

        transform.position = Vector2.zero;
        
        float x = Random.value < 0.5f ? -1f : 1f;
        float y = Random.Range(-0.8f, 0.8f);
        
        Vector2 direction = new Vector2(x, y).normalized;
        rb.linearVelocity = direction * speed;
    }
}