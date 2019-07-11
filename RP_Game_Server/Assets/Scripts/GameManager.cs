using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public const float RESET_POS_TIME = 0.15f; // 0.1f is good

    GameObject players;

    void Start() {
        players = GameObject.Find("Players");

        // reset positions every RESET_POS_TIME
        StartCoroutine(updatePlayersPos());
    }

    Player player;
    private IEnumerator updatePlayersPos() {

        while (true) {
            // Debug.Log("updating players position");

            yield return new WaitForSeconds(RESET_POS_TIME);

            for (int i=0; i < players.transform.childCount; i++) {
                // get player
                player = players.transform.GetChild(i).GetComponent<Player>();

                // broadcast position of player
                ServerNET_SendFunction.UpdatePositionRotationY(player.ConnectionId, player.Position, player.Rotation.y);

            }
        }

    }
}
