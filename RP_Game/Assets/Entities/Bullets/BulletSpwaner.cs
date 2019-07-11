using System.Collections;
using UnityEngine;

public class BulletSpwaner : MonoBehaviour {
    public float timeToRespawn = 1f;
    public GameObject bulletPrefab;

    void Start() {
        StartCoroutine(spawnBullet());
    }

    IEnumerator spawnBullet() {
        while (true) {
            yield return new WaitForSeconds(timeToRespawn);
            Instantiate(bulletPrefab, GameObject.Find("Bullets").transform);
        }
    }

}
