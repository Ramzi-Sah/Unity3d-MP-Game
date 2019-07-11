using UnityEngine;
// ﻿using UnityEngine.UI;

public class GUI : MonoBehaviour {
    public static TMPro.TextMeshProUGUI latencyGUI;

    void Start() {
        latencyGUI = GameObject.Find("GUI_latency_txt").GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void disconnect_btn() {
        Client client = GameObject.Find("Client").GetComponent<Client>();

        Debug.Log("disconnect btn pressed !");
        client.disconnect();
    }

    // resets latency text gui
    public static void setLatencyText(float latency) {
        latencyGUI.text = "ping: " + Mathf.Round(latency * 100) + " ms";
    }
}
