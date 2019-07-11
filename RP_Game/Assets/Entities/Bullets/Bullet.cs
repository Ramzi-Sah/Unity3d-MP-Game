using UnityEngine;

public class Bullet : MonoBehaviour {
    public float timeToDestroySeconds = 4f;

    private Rigidbody rb;
    private float spawnTime;

    void Start() {
        rb = GetComponent<Rigidbody>();
        spawnTime = Time.time;

        rb.AddForce(new Vector3(0, 0, 2000f)); // BOOM
    }

    void Update() {
        if (Time.time - spawnTime >= timeToDestroySeconds) {
            Destroy(gameObject);
        }
    }




}
