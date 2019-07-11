using UnityEngine;

public static class NET_REC_MESSEGES {
    public static byte PONG_RESPONSE = 0;

    public static byte PLAYER_CONNECTED = 1;
    public static byte PLAYER_DISCONNECTED = 2;
    public static byte CREATE_PLAYER_LIST = 3;

    public static byte UPDATE_POS = 4;
    public static byte UPDATE_MOV = 5;

};

public static class ClientNET_RecieaveFunction {
    static Client client;

    public static void init(Client curentClient) {
        client = curentClient;
    }

    public static void OnDataRecieaved(byte[] data) {

        // recieave pong
        if (data[0] == NET_REC_MESSEGES.PONG_RESPONSE) {
            recieavePong(data);
        }

        // player connected message
        if (data[0] == NET_REC_MESSEGES.PLAYER_CONNECTED) {
            playerConnected(data);
        }

        // player disconnected message
        if (data[0] == NET_REC_MESSEGES.PLAYER_DISCONNECTED) {
            playerDisConnected(data);
        }

        // player jip create players alredy in connected
        if (data[0] == NET_REC_MESSEGES.CREATE_PLAYER_LIST) {
            jipCreatePlayersList(data);
        }


        // update position message
        if (data[0] == NET_REC_MESSEGES.UPDATE_POS) {
            UpdatePosition(data);
        }

        // update movment message
        if (data[0] == NET_REC_MESSEGES.UPDATE_MOV) {
            UpdateMovement(data);
        }


    }

    /*------------------------ handle connection ------------------------------*/
    // recieave pong response
    public static void recieavePong(byte[] data) {
        float pingLatency = byteArrayTofloat(data, 1); // float 4 bytes
        float PongSendedTime = byteArrayTofloat(data, 5); // float 4 bytes

        // calculate time taken for ping request to arrive
        float pongLatency = Time.time - PongSendedTime;
        float totalPingRequestLatency = pongLatency + pingLatency;

        // Debug.Log("recieaved pong request. total latency: " + totalPingRequestLatency * 100 + "ms.");

        // set GUI
        GUI.setLatencyText(totalPingRequestLatency);
    }


    // on player connected
    private static void playerConnected(byte[] data) {
        int ClientId = byteArrayToInt(data, 1); // int 4 bytes
        Vector3 SpawnPosition = byteArrayToVector3(data, 5); // vector3 12 bytes
        bool playable = false;

        // check if is playable
        if (GameObject.Find("Players").transform.childCount == 1) {
            playable = true;

            // set player client id
            client.clientID = ClientId;
        }

        client.initNewPlayer(ClientId, playable, SpawnPosition); // create player

        Debug.Log("player " + ClientId + " Connected.");
    }

    // on player disconnected
    private static void playerDisConnected(byte[] data) {
        int ClientId = byteArrayToInt(data, 1);
        client.DestroyPlayer(ClientId); // destroy pawn

        Debug.Log("player " + ClientId + " disconnected.");
    }

    // for join in progress create alredy connected players
    private static void jipCreatePlayersList(byte[] data) {
        int ClientId = byteArrayToInt(data, 1); // int 4 bytes
        Vector3 ClientNewPosition = byteArrayToVector3(data, 5); // vector3 12 bytes

        client.initNewPlayer(ClientId, false, ClientNewPosition); // create new non playable pawn
        // Debug.Log("create pawn " + ClientId + " on pos:" + ClientNewPosition);
    }

    // on update pos recieaved
    private static void UpdatePosition(byte[] data) {
        int ClientId = byteArrayToInt(data, 1); // int 2 bytes
        Vector3 ClientNewPosition = byteArrayToVector3(data, 5); // vector3 12 bytes
        float ClientNewRotationY = byteArrayTofloat(data, 17); // float 4 bytes

        // check if it is the player
        if (client.getPlayerFromClientID(ClientId) != null) {
            Player pawn = client.getPlayerFromClientID(ClientId).GetComponent<Player>();
            if (ClientId != client.clientID) {
                // update position
                pawn.interpolateToPosition(ClientNewPosition);
                pawn.setRotationY(ClientNewRotationY);

                // Debug.Log("Update player " + ClientId + " to position : " + ClientNewPosition + " rotation: " + ClientNewRotationY + ".");
            }
        }

    }

    // on update movment message recieaved
    private static void UpdateMovement(byte[] data) {
        int ClientId = byteArrayToInt(data, 1); // int 2 bytes
        Vector3 ClientNewPosition = byteArrayToVector3(data, 5); // vector3 12 bytes
        float ClientNewRotationY = byteArrayTofloat(data, 17); // float 4 bytes
        int ClientKey = byteArrayToInt(data, 21); // vector3 12 bytes

        // check if it is the player
        if (client.getPlayerFromClientID(ClientId) != null) {
            Player pawn = client.getPlayerFromClientID(ClientId).GetComponent<Player>();
            if (ClientId != client.clientID) {
                // update position
                pawn.setPosition(ClientNewPosition);
                pawn.setRotationY(ClientNewRotationY);
                pawn.setKey(ClientKey);
            }
        }

        // Debug.Log("updating player movement " + ClientId + " ClientNewPosition to:" + ClientNewPosition + " ClientNewRotationY to:" + ClientNewRotationY + " ClientKey to:" + ClientKey);
    }


    /*---------------------------- UTILS -----------------------------------*/
    static Vector3 byteArrayToVector3(byte[] data, int index) {
        Vector3 vec = new Vector3();

        float x = System.BitConverter.ToSingle(data, index);
        float y = System.BitConverter.ToSingle(data, index + 4);
        float z = System.BitConverter.ToSingle(data, index + 8);

        vec.Set(x, y, z);
        return vec;
    }

    static int byteArrayToInt(byte[] data, int index) {
        return System.BitConverter.ToInt32(data, index);
    }

    static float byteArrayTofloat(byte[] data, int index) {
        return System.BitConverter.ToSingle(data, index);
    }

}
