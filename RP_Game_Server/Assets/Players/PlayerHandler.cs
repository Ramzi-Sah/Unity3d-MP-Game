using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public void InitNewPlayer(int connectionId) {
        GameObject playerGameObject = Instantiate(PlayerPrefab, transform);

        Player player = playerGameObject.GetComponent<Player>();

        player.ConnectionId = connectionId;
        player.InitPlayer();

    }

    public void RemovePlayer(int connectionId) {

        // destroy disconnected player
        for (int i=0; i < transform.childCount; i++) {

            Player player = transform.GetChild(i).GetComponent<Player>();

            if (player.ConnectionId == connectionId) {
                player.DestroyPlayer();
            };

        }
    }

}
