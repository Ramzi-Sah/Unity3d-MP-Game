using System.Collections;
﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    public string ServerHost = "127.0.0.1";
    public int PORT = 2727;

    // config
    public static int BUFFER_SIZE = 25; // need to be same as server
    private static int MAX_USER = 100; // need to be same as server

    public bool isConnected = false;
    public int clientID = 0;
    public GameObject PlayerPrefab;

    string IPAdress;
    private static Client instance;
    private bool canConnect = false;
    private bool isStarted = false;
    private int connectionId;
    private byte reliableChanel, unReliableChanel;
    private int hostId;
    private byte error;
    private bool obsolete;
    private ConnectionMenu ConnectionMenu; // used to display connection staus


    #region MonoBehaviour
    void Awake() {
        // dont destroy client object when changing scean<
        DontDestroyOnLoad(gameObject);

        ConnectionMenu = GameObject.Find("ConnectionMenu").GetComponent<ConnectionMenu>();
    }
    void Update () {
        UpdateMessagePump();
    }
    #endregion


    public void init() {
        // protocol init
        NetworkTransport.Init();
        ClientNET_RecieaveFunction.init(this);
        ClientNET_SendFunction.init(this);

        ConnectionConfig cc = new ConnectionConfig();
        reliableChanel = cc.AddChannel(QosType.Reliable); // ReliableSequenced, Reliable
        unReliableChanel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);
        hostId = NetworkTransport.AddHost(topo, 0);

        /*--------------------------------------------------*/

        Debug.Log(ServerHost);
        ConnectionMenu.setConnectionStatusMessage("connecting to " + ServerHost);
        if (ServerHost != "localhost" && ServerHost != "127.0.0.1") { // FIXME: check if is on local area
            // check if player is connected
            try {
                IPAdress = System.Net.Dns.GetHostEntry("google.com").AddressList[0].ToString();
                ConnectionMenu.setConnectionStatusMessage("Checking connection status.");
                Debug.Log("Checking connection status.");
            } catch (System.Exception e) {
                ConnectionMenu.setConnectionStatusMessage("error: you are not connected to the internet.");
                Debug.Log("error: you are not connected to the internet.");
                disconnect(); // disconnect client

            }
        }

        // resolve hostname
        try {
            IPAdress = System.Net.Dns.GetHostEntry(ServerHost).AddressList[0].ToString();
            ConnectionMenu.setConnectionStatusMessage("Resolving hostname " + ServerHost);
            canConnect = true;
        } catch (System.Exception e) {
            ConnectionMenu.setConnectionStatusMessage("error: hostname " + ServerHost + " dosen't exist");
            Debug.Log("error: hostname " + ServerHost + "dosen't exist");
            disconnect(); // disconnect client
        }

        /*--------------------------------------------------*/
        if (canConnect) {
            // try to connect
            connectionId = NetworkTransport.Connect(hostId, IPAdress, PORT, 0, out error);
            Debug.Log("try to connect to " + ServerHost + " |  " + IPAdress + ":" + PORT);
            ConnectionMenu.setConnectionStatusMessage("Connecting to " + ServerHost + ":"+ PORT + "...");

            if (error != 0) {
                Debug.Log("can't connect to server | error: " + error + " | retriying...");
                ConnectionMenu.setConnectionStatusMessage("can't connect to server | error: " + error + " | retriying...");
                init();
            } else {
                isStarted = true;
            }
        }
    }

    public void shutdown() {
        isStarted = false;
        isConnected = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessagePump () {
        if (!isStarted)
            return;

        int recieavedHostID;
        int chanelId;

        byte[] recvBuffer = new byte[BUFFER_SIZE];
        int DataSize;

        NetworkEventType type = NetworkTransport.Receive(out recieavedHostID, out connectionId, out chanelId, recvBuffer, BUFFER_SIZE, out DataSize, out error);
        switch (type) {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("sucsessfully connected to the server as user: " + connectionId);
                isConnected = true;
                SceneManager.LoadScene(1);

                break;

            case NetworkEventType.DisconnectEvent:
                string disconnectedMsg = "";
                if (!isConnected) {
                    disconnectedMsg = "can't connect to server.";
                    Debug.Log(disconnectedMsg);
                } else {
                    disconnectedMsg = "disconnected from the server.";
                    Debug.Log(disconnectedMsg);
                }
                // disconnect client and set disconnected message
                disconnect();

                break;

            case NetworkEventType.DataEvent:
                ClientNET_RecieaveFunction.OnDataRecieaved(recvBuffer);
                // Debug.Log("recieaved data type: " + recvBuffer[0]);

                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("unexpected network event type.");
                break;
        }
    }

    public void SendServer(byte[] buffer) {
        if (!isStarted)
            return;

        if (buffer.Length > BUFFER_SIZE) {
            Debug.Log("triying to send messege too big");
        } else {
            // send data
            NetworkTransport.Send(hostId, connectionId, reliableChanel, buffer, BUFFER_SIZE, out error);
            // Debug.Log("sending data type:" + buffer[0]);

        }
    }

    public void SendServerUnrealibale(byte[] buffer) {
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

    /*------------------------------------------------------------*/
    public void disconnect() {
        // shutDown Client
        shutdown();

        // looad main menu scean
        SceneManager.LoadScene(0);

        // destroy client
        Destroy(gameObject);
    }

    void OnGUI() {
        if (!isConnected) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                disconnect();
            }
        }
    }

    /*-------------------------- UTILS ---------------------------*/

    public GameObject getPlayerFromClientID(int ClientID) {
        GameObject players = GameObject.Find("Players");

        for (int i=0; i < players.transform.childCount; i++) {
            Player player = players.transform.GetChild(i).GetComponent<Player>();
            if (player.clientID == ClientID) {
                return players.transform.GetChild(i).gameObject;
            }

        }

        Debug.Log("getPlayerFromClientID null ClientID:" + ClientID);
        return null;
    }

    public void initNewPlayer(int clientId, bool playable, Vector3 position) {
        GameObject player = Instantiate(PlayerPrefab, GameObject.Find("Players").transform);
        player.GetComponent<Player>().Init(clientId, playable, position);
    }

    public void DestroyPlayer(int clientId) {
        getPlayerFromClientID(clientId).GetComponent<Player>().Destroy();
    }


}
