using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MenuLogic : MonoBehaviour
{
    public NetManTest nm;

    private void CheckRef()
    {
        if (nm == null)
        {
            nm = GameObject.FindGameObjectWithTag("Network").GetComponent<NetManTest>();
        }
    }

    public void StartClient()
    {
        CheckRef();
        nm.NMStartClient();
    }

    public void StartHost()
    {
        CheckRef();
       nm.StartHost();
    }

    public void StartServer()
    {
        CheckRef();
        nm.NMStartServer();
    }
}
