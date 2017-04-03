using UnityEngine;

using Balance.Utils;
using Balance.Client;
using Balance.Specialized;

public class BUStats {
	public long WsPing = -1;
	public long UdpPing = -1;
}

public class BalanceUnity : MonoBehaviour {

	public string ServerAddress = "192.168.192.52";
	public int ServerPort = 8443;
    public int UdpServerPort = 9443;
	public bool ClientDebugLog = false;

    private RoomGroupClient rgc;

	// Use this for initialization
	void Start () {
        rgc = this.initBalanceEngine();
    }

	public BUStats GetStats(){
		
		BUStats stats = new BUStats ();

		if (rgc != null) {
			stats.WsPing = rgc.GetPing ();
		}

		if (rgc.GetUdpSubClient() != null) {
			stats.UdpPing = rgc.GetUdpSubClient().GetPing ();
		}

		return stats;
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

    public void Reconnect()
    {
        if (rgc != null && rgc.IsInMatch())
        {
            rgc.ExitMatch();
        }

        endRgc();
        rgc = initBalanceEngine();
    }

    RoomGroupClient initBalanceEngine()
    {
        Config config = new Config();
		config.hostname = ServerAddress;
        config.port = ServerPort;
		config.debugLog = ClientDebugLog;

        Config udpConfig = new Config();
        udpConfig.hostname = ServerAddress;
        udpConfig.port = UdpServerPort;
        config.debugLog = ClientDebugLog;

        WSClient ws = new WSClient();
        RoomGroupClient rgc = new RoomGroupClient(config, ws, udpConfig, this.debugLog);

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
