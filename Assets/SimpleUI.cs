using UnityEngine;

using Balance.Specialized;

public class SimpleUI : MonoBehaviour {

    private RoomGroupClient rgc;

    // Called when Unity3D updates its UI
    void OnGUI()
    {
        if (rgc == null)
        {
            rgc = BalanceUnity.ACCESS().GetRGC();
        }

        //MM-Queue
        if (!rgc.IsInQueue() && !rgc.IsInMatch())
        {
            if (GUI.Button(new Rect(10, 10, 126, 30), "Join Match-Making"))
            {
                rgc.JoinMatchMakingQueue();
            }
        } else if(!rgc.IsInMatch() && !rgc.IsConfirmationOpen())
        {
            if (GUI.Button(new Rect(10, 10, 146, 30), "Leave Match-Making"))
            {
                rgc.LeaveMatchMakingQueue();
            }
        }

        //Match-Confirmation
        if (rgc.IsConfirmationOpen())
        {
            if (GUI.Button(new Rect(10, 50, 126, 30), "Confirm Match"))
            {
                rgc.ConfirmMatchRequest();
            }
        }

        //Match
        if (rgc.IsInMatch())
        {
            if (GUI.Button(new Rect(10, 90, 126, 30), "Leave Match"))
            {
                rgc.ExitMatch();
            }
        }

    }
}
