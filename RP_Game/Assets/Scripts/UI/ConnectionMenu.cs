using UnityEngine;
﻿using UnityEngine.UI;

public class ConnectionMenu : MonoBehaviour
{
    public Client client;

    // to active/disactive menus
    public GameObject menu;
    public GameObject connectStatus_txt;

    // get inputs values
    public InputField host_inpt;
    public InputField port_inpt;

    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void connect_btn() {

        // set host & port info on client
        client.ServerHost = (string)host_inpt.text;
        client.PORT = int.Parse((string)port_inpt.text);

        // handle menu
        menu.SetActive(false); // desactive connection menu
        connectStatus_txt.SetActive(true); // active connection status txt

        // start client
        client.init();
    }

    public void setConnectionStatusMessage(string msg) {
        connectStatus_txt.GetComponent<Text>().text = msg;
    }

    public string getConnectionStatusMessage() {
        return connectStatus_txt.GetComponent<Text>().text;
    }

    public void setConnectionStatusMessageActive(bool activate) {
        connectStatus_txt.SetActive(activate);
    }

}
