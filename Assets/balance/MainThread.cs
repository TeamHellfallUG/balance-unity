using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.spacepuppy.Async;

public class MainThread : MonoBehaviour
{
    #region Fields
    private static InvokePump _pump;
    private static MainThread _singleton;
    #endregion

    #region CONSTRUCTOR
    void Awake()
    {
        if (_singleton != null)
        {
            Debug.LogWarning("Multiple MainThread InvokePumps were added to the scene.");
            Object.Destroy(this);
            return;
        }

        _pump = new InvokePump(); //it was created in the main thread, so the owner thread is main thread
        _singleton = this;
    }

    #endregion

    public static InvokePump Pump { get { return _pump; } }

    public static void Call(System.Action<object> func, object obj)
    {
        _pump.BeginInvoke(() => { func(obj); });
    }
    public static void Call(System.Action func)
    {
        _pump.BeginInvoke(func);
    }

    private void Update()
    {
        _pump.Update();
    }

}