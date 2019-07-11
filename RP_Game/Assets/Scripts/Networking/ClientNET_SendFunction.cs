using UnityEngine;

public static class NET_SEND_MESSEGES {
    public static byte PING_REQUEST = 0;

    public static byte UPDATE_POS = 1;
    public static byte UPDATE_MOV = 2;

};

public static class ClientNET_SendFunction {
    static Client client;
    static byte[] buffer;

    public static void init(Client curentClient) {
        client = curentClient;
    }

    /*------------------------ handle connection ------------------------------*/
    // send ping request
    public static void sendPing() {
        buffer = new byte[Client.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.PING_REQUEST; // byte
        intToByteArray(client.clientID).CopyTo(buffer, 1); // int 4 bytes
        floatToByteArray(Time.time).CopyTo(buffer, 5); // float 4 bytes

        client.SendServerUnrealibale(buffer);
        // Debug.Log("sending ping request.");
    }




    /*------------------------ handle player movements ------------------------------*/

    public static void SendPositionRotationY(Vector3 pos, float rotY) {
        buffer = new byte[Client.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.UPDATE_POS; // byte
        intToByteArray(client.clientID).CopyTo(buffer, 1); // int 4 bytes
        vector3ToByteArray(pos).CopyTo(buffer, 5); // vector3 12 bytes
        floatToByteArray(rotY).CopyTo(buffer, 17); // float 4 bytes

        client.SendServerUnrealibale(buffer);
        // Debug.Log("Update Position player " + client.clientID + " to position : " + pos);
    }

    public static void SendMovement(Vector3 pos, float rotY, int key) {
        buffer = new byte[Client.BUFFER_SIZE]; // empty buffer

        buffer[0] = NET_SEND_MESSEGES.UPDATE_MOV; // byte
        intToByteArray(client.clientID).CopyTo(buffer, 1); // int 4 bytes
        vector3ToByteArray(pos).CopyTo(buffer, 5); // vector3 12 bytes
        floatToByteArray(rotY).CopyTo(buffer, 17); // float 4 bytes
        intToByteArray(key).CopyTo(buffer, 21); // int 4 bytes

        client.SendServer(buffer);
        // Debug.Log("Update Position player " + client.clientID + " to position: " + pos + " rotY: " + rotY + " key: " + key);
    }

    /*---------------------------- UTILS -----------------------------------*/

    static byte[] vector3ToByteArray(Vector3 position) {
        byte[] data = new byte[12]; // 1 float = 4 bytes so 3 floats = 12 bytes
        byte[] valX_bytes_array = System.BitConverter.GetBytes(position.x);
        byte[] valY_bytes_array = System.BitConverter.GetBytes(position.y);
        byte[] valZ_bytes_array = System.BitConverter.GetBytes(position.z);

        // FIXME: optimise
        data[0] = valX_bytes_array[0];
        data[1] = valX_bytes_array[1];
        data[2] = valX_bytes_array[2];
        data[3] = valX_bytes_array[3];

        data[4] = valY_bytes_array[0];
        data[5] = valY_bytes_array[1];
        data[6] = valY_bytes_array[2];
        data[7] = valY_bytes_array[3];

        data[8] = valZ_bytes_array[0];
        data[9] = valZ_bytes_array[1];
        data[10] = valZ_bytes_array[2];
        data[11] = valZ_bytes_array[3];

        return data;
    }

    static byte[] intToByteArray(int val) {
        byte[] data = new byte[4]; // int = 4 bytes
        byte[] val_bytes_array = System.BitConverter.GetBytes(val);

        data[0] = val_bytes_array[0];
        data[1] = val_bytes_array[1];
        data[2] = val_bytes_array[2];
        data[3] = val_bytes_array[3];

        return data;
    }

    static byte[] floatToByteArray(float val) {
        byte[] data = new byte[4]; // float = 4 bytes
        byte[] val_bytes_array = System.BitConverter.GetBytes(val);

        data[0] = val_bytes_array[0];
        data[1] = val_bytes_array[1];
        data[2] = val_bytes_array[2];
        data[3] = val_bytes_array[3];

        return data;
    }

}
