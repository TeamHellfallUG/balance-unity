﻿using UnityEngine;

using Balance.Utils;
using Balance.Client;
using Balance.Specialized;

public class BalanceUnity : MonoBehaviour {

	public string ServerAddress = "192.168.192.52";
	public int ServerPort = 8443;
	public bool ClientDebugLog = false;

    private RoomGroupClient rgc;

	// Use this for initialization
	void Start () {
        rgc = this.initBalanceEngine();
    }

    void debugLog(string message)
    {
        Debug.Log(message);
    }

    public RoomGroupClient GetRGC()
    {
        return rgc;
    }

    public static BalanceUnity ACCESS()
    {
        if(Camera.main == null)
        {
            Debug.LogWarning("There is not Main-Camera in the scene, cannot access balance unity.");
            return null;
        }

        BalanceUnity bu = Camera.main.GetComponent<BalanceUnity>();
        if(bu == null)
        {
            Debug.LogWarning("There is not BalanceUnity Instance present on the Main-Camera.");
        }

        return bu;
    }

    RoomGroupClient initBalanceEngine()
    {
        Config config = new Config();
		config.hostname = ServerAddress;
        config.port = ServerPort;
		config.debugLog = ClientDebugLog;

        WSClient ws = new WSClient();
        RoomGroupClient rgc = new RoomGroupClient(config, ws, this.debugLog);

        ws.OnConnect += () => {
            Debug.Log("connected.");
        };

		rgc.OnReady += () => {
			Debug.Log("rgc is ready: " + rgc.GetIdentification());
		};

        rgc.OnJoinedQueue += packet => {
            Debug.Log("queue joined.");
        };

        rgc.Run();
        return rgc;
    }

    void endRgc()
    {
        if (rgc != null)
        {
            Debug.Log("closed");
            rgc.Close();
            rgc = null;
        }
    }

    // OnDisable is called when Scene is taken down
    void OnDisable()
    {
        this.endRgc();
    }

    ~BalanceUnity()
    {
        this.endRgc();
    }
}
