using System.Collections.Generic;
﻿using UnityEngine;
﻿using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    // config
    private static int PORT = 2727;
    public static int BUFFER_SIZE = 25;
    public static int MAX_USER = 100;

    private bool isStarted = false;

    public GameObject PlayersHolder;
    public static List<int> clients = new List<int>();

    private byte reliableChanel, unReliableChanel;
    private int hostId;
    byte error;

    #region MonoBehaviour
    void Start() {
        init();

    }
    void Update() {
        UpdateMessagePump();

    }
    #endregion

    public void init() {
        // protocol init
        NetworkTransport.Init();
        ServerNET_RecieaveFunction.init(this);
        ServerNET_SendFunction.init(this);

        ConnectionConfig cc = new ConnectionConfig();
        reliableChanel = cc.AddChannel(QosType.Reliable); // ReliableSequenced, Reliable
        unReliableChanel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        // server socket
        hostId = NetworkTransport.AddHost(topo, PORT, null);
        isStarted = true;

        Debug.Log("opening connection on port:" + PORT + ".");
    }

    public void shutdown() {
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    int recieavedHostID, connectionId, chanelId, DataSize;
    byte[] recvBuffer;
    NetworkEventType networkEvntType;
    public void UpdateMessagePump() {
        if (!isStarted)
            return;

        recvBuffer = new byte[BUFFER_SIZE];

        networkEvntType = NetworkTransport.Receive(out recieavedHostID, out connectionId, out chanelId, recvBuffer, BUFFER_SIZE, out DataSize, out error);
        switch (networkEvntType) {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                OnClientConnect(connectionId);

                Debug.Log("user " + connectionId + " connected.");
                break;

            case NetworkEventType.DisconnectEvent:
                OnClientDisconnect(connectionId);

                Debug.Log("user " + connectionId + " disconnected.");
                break;

            case NetworkEventType.DataEvent:
                // Debug.Log("recieaved from: " + connectionId + " | data type: " + recvBuffer[0]);
                ServerNET_RecieaveFunction.OnDataRecieaved(recvBuffer);
                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("unexpected network event type.");
                break;
        }
    }

    public void SendClient(int connectionId, byte[] buffer) {

        if (buffer.Length > BUFFER_SIZE) {
            Debug.Log("triying to send messege too big!");
        } else {
            // send data
            NetworkTransport.Send(hostId, connectionId, reliableChanel, buffer, BUFFER_SIZE, out error);
            // Debug.Log("sending data type:" + buffer[0]);
        }
    }

    public void SendServerUnrealibale(int connectionId, byte[] buffer) {
        if (!isStarted)
            return;

        if (buffer.Length > BUFFER_SIZE) {
            Debug.Log("triying to send messege too big");
        } else {
            // send data
            NetworkTransport.Send(hostId, connectionId, unReliableChanel, buffer, BUFFER_SIZE, out error);
            // Debug.Log("sending data type:" + buffer[0]);

        }
    }

    /*---------------------------- Event Handlers ---------------------------------*/

    public void OnClientConnect(int connectionId) {
        clients.Add(connectionId); // add client to clients list
        PlayersHolder.GetComponent<PlayerHandler>().InitNewPlayer(connectionId); // add it to server hiarchy
        ServerNET_SendFunction.playerConnected(connectionId); // send it to connected players

        Debug.Log("player " + connectionId + " connected.");
    }

    public void OnClientDisconnect(int connectionId) {
        clients.Remove(connectionId); // remove client from clients list
        PlayersHolder.GetComponent<PlayerHandler>().RemovePlayer(connectionId); // remove it to server hiarchy
        ServerNET_SendFunction.playerDisConnected(connectionId); // dont send it to disconnected players

        Debug.Log("player " + connectionId + " disconnected.");
    }

    /*---------------------------- UTILS -----------------------------------*/
    public GameObject getPlayerFromClientID(int ClientID) {
        GameObject players = GameObject.Find("Players");

        for (int i=0; i < players.transform.childCount; i++) {
            Player player = players.transform.GetChild(i).GetComponent<Player>();
            if (player.ConnectionId == ClientID) {
                return players.transform.GetChild(i).gameObject;
            }

        }

        Debug.Log("getPlayerFromClientID null ClientID:" + ClientID);
        return null;
    }




}
