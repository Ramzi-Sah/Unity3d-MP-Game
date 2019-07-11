using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const float PING_REQUEST_TIME = 0.8f;
    public const float RESET_POS_TIME = 0.15f;

    Client client;
    Player player;

    void Start() {
        client = GameObject.Find("Client").GetComponent<Client>();

        // reset player position on server every RESET_POS_TIME
        StartCoroutine(updatePlayerPos());
        StartCoroutine(sendPingRequest());
    }

    private IEnumerator updatePlayerPos() {

        // wait until player connects
        yield return new WaitUntil(() => client.getPlayerFromClientID(client.clientID) != null);
        player = client.getPlayerFromClientID(client.clientID).GetComponent<Player>();

        while (true) {
            // wait RESET_POS_TIME
            yield return new WaitForSeconds(RESET_POS_TIME);

            // broadcast position of player
            ClientNET_SendFunction.SendPositionRotationY(player.transform.position, player.transform.eulerAngles.y); // broad Cast new position
            // Debug.Log("setting " + player + " to pos:" + player.transform.position + " and rotY to:" + player.transform.rotation.y);

        }

    }

    private IEnumerator sendPingRequest() {
        while (true) {
            // wait PING_REQUEST_TIME
            yield return new WaitForSeconds(PING_REQUEST_TIME);

            // send ping request
            ClientNET_SendFunction.sendPing();

            // Debug.Log("sending ping request");

        }

    }

}
