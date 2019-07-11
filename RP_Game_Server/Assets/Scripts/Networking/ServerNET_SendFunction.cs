using UnityEngine;

public static class NET_SEND_MESSEGES {
    public static byte PONG_RESPONSE = 0;

    public static byte PLAYER_CONNECTED = 1;
    public static byte PLAYER_DISCONNECTED = 2;
    public static byte CREATE_PLAYER_LIST = 3;

    public static byte UPDATE_POS = 4;
    public static byte UPDATE_MOV = 5;
};

public static class ServerNET_SendFunction {

    public static Vector3 spawnPoint = new Vector3(150, 60, 120);

    static Server server;
    static byte[] buffer;

    public static void init(Server _server) {
        server = _server;
    }

    /*------------------------ handle connection ------------------------------*/
    // send pong request
    public static void sendPong(int ClientId, float pingLatency) {
        buffer = new byte[Server.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.PONG_RESPONSE; // byte
        floatToByteArray(pingLatency).CopyTo(buffer, 1); // float 4 bytes
        floatToByteArray(Time.time).CopyTo(buffer, 5); // float 4 bytes

        server.SendClient(ClientId, buffer);
        // Debug.Log("sending pong response to: " + ClientId + ".");
    }

    // on player connected
    public static void playerConnected(int ClientId) {
        byte[] buffer = new byte[Server.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.PLAYER_CONNECTED; // byte
        intToByteArray(ClientId).CopyTo(buffer, 1); // int 4 bytes
        vector3ToByteArray(spawnPoint).CopyTo(buffer, 5); // Vector3 12 bytes

        // Debug.Log("player " + ClientId + " connected send to all.");
        sendToAll(buffer, 0);

        createConnectedPlayers_JIP(ClientId); // for join in progress players
    }

    // on player disconnected
    public static void playerDisConnected(int ClientId) {
        byte[] buffer = new byte[Server.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.PLAYER_DISCONNECTED; // byte
        intToByteArray(ClientId).CopyTo(buffer, 1); // int 2 bytes

        // Debug.Log("player " + ClientId + " disconnected send to all.");
        sendToAll(buffer, ClientId);
    }

    // for join in progress create alredy connected players
    public static void createConnectedPlayers_JIP(int _clientId) {
        buffer = new byte[Server.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.CREATE_PLAYER_LIST; // byte

        // for (int i=1; i < Server.MAX_USER; i++) {
        foreach(int clientId in Server.clients) {
            if (clientId != _clientId) {
                intToByteArray(clientId).CopyTo(buffer, 1); // int 4 bytes

                Vector3 playerPos = server.getPlayerFromClientID(clientId).GetComponent<Player>().Position; // position of player
                vector3ToByteArray(playerPos).CopyTo(buffer, 5); // Vector3 12 bytes

                // Debug.Log("player " + _clientId + " is JIP sending player " + i + " info Pos: "+ playerPos);
                server.SendClient(_clientId, buffer);
            }
        }


    }


    /*------------------------ handle player movements ------------------------------*/
    public static void UpdatePositionRotationY(int ClientId, Vector3 newPos, float newRotY) {
        buffer = new byte[Server.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.UPDATE_POS; // byte
        intToByteArray(ClientId).CopyTo(buffer, 1); // int 4 bytes
        vector3ToByteArray(newPos).CopyTo(buffer, 5); // vector3 12 bytes
        floatToByteArray(newRotY).CopyTo(buffer, 17); // float 4 bytes

        sendToAllUnRealable(buffer, ClientId);
        // Debug.Log("send to all player " + ClientId + " position : " + newPos + " rotation: " + newRotY + ".");
    }

    public static void SendMovement(int ClientId, Vector3 pos, float rotY, int key) {
        buffer = new byte[Server.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.UPDATE_MOV; // byte
        intToByteArray(ClientId).CopyTo(buffer, 1); // int 4 bytes
        vector3ToByteArray(pos).CopyTo(buffer, 5); // vector3 12 bytes
        floatToByteArray(rotY).CopyTo(buffer, 17); // float 4 bytes
        intToByteArray(key).CopyTo(buffer, 21); // int 4 bytes

        sendToAll(buffer, ClientId);
        // Debug.Log("Update Position player " + ClientId + " to position: " + pos + " rotY: " + rotY + " key: " + key);
    }


    /*---------------------------------------------------------------*/
    // get all players and send to them packets
    private static void sendToAll(byte[] buffer, int AllButClientID) {
        foreach(int clientId in Server.clients) {
            if (clientId != AllButClientID) { // check if is not sending to same player
                server.SendClient(clientId, buffer);
            }
        }
    }

    // get all players and send to them packets as unrelaiable
    private static void sendToAllUnRealable(byte[] buffer, int AllButClientID) {
        foreach(int clientId in Server.clients) {
            if (clientId != AllButClientID) { // check if is not sending to same player
                server.SendServerUnrealibale(clientId, buffer);
            }
        }
    }

    /*---------------------------- UTILS -----------------------------------*/
    static byte[] vector3ToByteArray(Vector3 position) {
        byte[] data = new byte[12]; // 1 float = 4 bytes so 3 floats = 12 bytes

        // FIXME: optimise
        data[0] = System.BitConverter.GetBytes(position.x)[0];
        data[1] = System.BitConverter.GetBytes(position.x)[1];
        data[2] = System.BitConverter.GetBytes(position.x)[2];
        data[3] = System.BitConverter.GetBytes(position.x)[3];

        data[4] = System.BitConverter.GetBytes(position.y)[0];
        data[5] = System.BitConverter.GetBytes(position.y)[1];
        data[6] = System.BitConverter.GetBytes(position.y)[2];
        data[7] = System.BitConverter.GetBytes(position.y)[3];

        data[8] = System.BitConverter.GetBytes(position.z)[0];
        data[9] = System.BitConverter.GetBytes(position.z)[1];
        data[10] = System.BitConverter.GetBytes(position.z)[2];
        data[11] = System.BitConverter.GetBytes(position.z)[3];

        return data;
    }

    static byte[] intToByteArray(int val) {
        byte[] data = new byte[4]; // int = 4 bytes

        data[0] = System.BitConverter.GetBytes(val)[0];
        data[1] = System.BitConverter.GetBytes(val)[1];
        data[2] = System.BitConverter.GetBytes(val)[2];
        data[3] = System.BitConverter.GetBytes(val)[3];

        return data;
    }

    static byte[] floatToByteArray(float val) {
        byte[] data = new byte[4]; // float = 4 bytes

        data[0] = System.BitConverter.GetBytes(val)[0];
        data[1] = System.BitConverter.GetBytes(val)[1];
        data[2] = System.BitConverter.GetBytes(val)[2];
        data[3] = System.BitConverter.GetBytes(val)[3];

        return data;
    }

}
