using UnityEngine;

using System.Collections.Generic;

using Balance.Specialized;
using Balance.Utils;

public class Other
{
    public string ClientId;
    public GameObject GObject;

    public Other(string ClientId, GameObject GObject)
    {
        this.ClientId = ClientId;
        this.GObject = GObject;
    }
};

public class SimplePlayer : MonoBehaviour {

    public GameObject PlayerPrefab;
    public GameObject OthersPrefab;
    public Transform StartingPoint;

    //public for editor visibility
    public GameObject Player;
    public List<Other> Others = new List<Other>();

    private RoomGroupClient rgc;

    private int lastUpdated;
    const int updateHertz = 20;
    const int updateMs = 1000 / updateHertz;

    void Start()
    {
        lastUpdated = (int)(Time.time * 1000);
    }

    // Update is called once per frame
    void Update () {

        if (rgc == null)
        {
            rgc = BalanceUnity.ACCESS().GetRGC();

            rgc.OnMatchDisband += packet => {
                MainThread.Call(() =>
                {
                    removeOthers();
                    removePlayer();
                });
            };

            rgc.OnMatchEnd += packet => {
                MainThread.Call(() =>
                {
                    removeOthers();
                    removePlayer();
                });
            };

            rgc.OnMatchExit += packet => {
                MainThread.Call(() =>
                {
                    removeOthers();
                    removePlayer();
                });
            };

            rgc.OnMatchStart += packet =>
            {
                Debug.Log("OnMatchStart.");

                MainThread.Call(() =>
                {
                    spawnPlayer();
                });
            };

            rgc.OnStatesUpdate += states =>
            {
                //Debug.Log("OnStatesUpdate.");

                MainThread.Call(() =>
                {
                    handleStates(states);
                });
            };
        }

        int now = (int)(Time.time * 1000);
        if(now - lastUpdated >= updateMs)
        {
            lastUpdated = now;
            if (rgc.IsInMatch())
            {
                if (Player != null && Player.transform != null)
                {
                    rgc.SendStateUpdate(getStateUpdate());
                }
            }
        }
    }

    StateUpdate getStateUpdate()
    {
        StateUpdate su = new StateUpdate();

        su.position = new Vector(
            Player.transform.position.x,
            Player.transform.position.y,
            Player.transform.position.z
        );

        Vector3 rotation = Player.transform.rotation.eulerAngles;
        su.rotation = new Vector(
            rotation.x,
            rotation.y,
            rotation.z
        );

        return su;
    }

    void spawnPlayer()
    {
        Debug.Log("spawn player.");

        Vector3 position = new Vector3(
            StartingPoint.position.x + Random.Range(-10.0f, 10.0f),
            StartingPoint.position.y,
            StartingPoint.position.z + Random.Range(-10.0f, 10.0f)
            );

        Transform transf = new GameObject().transform;
        transf.position = position;
        transf.rotation = StartingPoint.rotation;

        Player = GameObject.Instantiate(PlayerPrefab, transf) as GameObject;
    }

    void removePlayer()
    {
        GameObject.DestroyImmediate(Player);
    }

    void removeOthers()
    {
        for(int i = 0; i < Others.Count; i++)
        {
            GameObject.DestroyImmediate(Others[i].GObject);
        }

        Others.Clear();
    }

    void handleStates(List<RStateUpdate> states)
    {
        RStateUpdate su = null;
        Other ot = null;
        bool known = false;
        for(int s = 0; s < states.Count; s++)
        {
            known = false;
            su = states[s];
            for (int o = 0; o < Others.Count; o++)
            {
                ot = Others[o];
                if(su.ClientId == ot.ClientId)
                {
                    known = true;
                    applyStateToGameObject(su, ot.GObject);
                    break;
                }
            }

            if (!known)
            {
                Transform transf = new GameObject().transform;

                transf.position = new Vector3(
                    (float)su.Position.x, 
                    (float)su.Position.y, 
                    (float)su.Position.z);

                transf.rotation = Quaternion.Euler(
                    (float)su.Rotation.x,
                    (float)su.Rotation.y,
                    (float)su.Rotation.z);

                Debug.Log("spawning other.");
                Other newOther = new Other(
                    su.ClientId,
                    GameObject.Instantiate(OthersPrefab, transf));

                Others.Add(newOther);
            }
        }
    }

    void applyStateToGameObject(RStateUpdate state, GameObject go)
    {
       Vector3 position = new Vector3(
                    (float)state.Position.x,
                    (float)state.Position.y,
                    (float)state.Position.z);

        Quaternion rotation = Quaternion.Euler(
            (float)state.Rotation.x,
            (float)state.Rotation.y,
            (float)state.Rotation.z);

        go.transform.position = position;
        go.transform.rotation = rotation;
        //TODO move & rotate smooth
        //TODO play animations
    }

}
