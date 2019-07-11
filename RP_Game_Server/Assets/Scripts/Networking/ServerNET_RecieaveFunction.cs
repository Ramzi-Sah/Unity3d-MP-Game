using UnityEngine;

public static class NET_REC_MESSEGES {
    public static byte PING_REQUEST = 0;

    public static byte UPDATE_POS = 1;
    public static byte UPDATE_MOV = 2;

};

public static class ServerNET_RecieaveFunction {
    static Server server;

    public static void OnDataRecieaved(byte[] data) {

        // ping request recieaved
        if (data[0] == NET_REC_MESSEGES.PING_REQUEST) {
            recieavePing(data);
        }

        // update pos recieaved
        if (data[0] == NET_REC_MESSEGES.UPDATE_POS) {
            UpdatePosition(data);
        }

        // update movement recieaved
        if (data[0] == NET_REC_MESSEGES.UPDATE_MOV) {
            UpdateMovement(data);
        }



    }

    public static void init(Server _server) {
        server = _server;
    }

    /*------------------------ handle connection ------------------------------*/
    // recieave ping request
    public static void recieavePing(byte[] data) {
        int ClientId = byteArrayToInt(data, 1); // int 4 bytes
        float pingSendedTime = byteArrayTofloat(data, 5); // float 4 bytes

        // calculate time taken for ping request to arrive
        float pingLatency = Time.time - pingSendedTime;

        // Debug.Log("recieaved ping request from " + ClientId + ".");

        // response with pong
        ServerNET_SendFunction.sendPong(ClientId, pingLatency);
    }


    /*------------------------ handle position ------------------------------*/
    // on update pos recieaved
    private static void UpdatePosition(byte[] data) {
        int ClientId = byteArrayToInt(data, 1); // int 4 bytes
        Vector3 ClientNewPosition = byteArrayToVector3(data, 5); // vector3 12 bytes
        float ClientNewRotationY = byteArrayTofloat(data, 17); // float 4 bytes

        // update position
        if (server.getPlayerFromClientID(ClientId) != null) {
            Player pawn = server.getPlayerFromClientID(ClientId).GetComponent<Player>();

            pawn.setPosition(ClientNewPosition);
            pawn.setRotationY(ClientNewRotationY);
        }
        // Debug.Log("updating player " + ClientId + " position to:" + ClientNewPosition);

    }

    // on update movement recieaved
    private static void UpdateMovement(byte[] data) {
        int ClientId = byteArrayToInt(data, 1); // int 4 bytes
        Vector3 ClientNewPosition = byteArrayToVector3(data, 5); // vector3 12 bytes
        float ClientNewRotationY = byteArrayTofloat(data, 17); // float 4 bytes
        int ClientKey = byteArrayToInt(data, 21); // vector3 12 bytes

        // update position
        if (server.getPlayerFromClientID(ClientId) != null) {
            Player pawn = server.getPlayerFromClientID(ClientId).GetComponent<Player>();

            pawn.setPosition(ClientNewPosition);
            pawn.setRotationY(ClientNewRotationY);
            pawn.setKey(ClientKey);
        }
        // Debug.Log("updating player movement " + ClientId + " ClientNewPosition to:" + ClientNewPosition + " ClientNewRotationY to:" + ClientNewRotationY + " ClientKey to:" + ClientKey);

        // send info to all
        ServerNET_SendFunction.SendMovement(ClientId, ClientNewPosition, ClientNewRotationY, ClientKey);
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
