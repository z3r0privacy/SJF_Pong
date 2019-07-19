using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class StateManager : NetworkBehaviour
{

    public event EventHandler GameStarted;

    public GameObject ObstaclePrefab;

    private NetManTest _networkManager;

    void Awake()
    {
        _networkManager = GameObject.FindGameObjectWithTag("Network").GetComponent<NetManTest>();
        _networkManager.PlayerChanged += NetworkManager_PlayerChanged;
    }
    private void OnDestroy()
    {
        _networkManager.PlayerChanged -= NetworkManager_PlayerChanged;
    }

    private void NetworkManager_PlayerChanged(object sender, PlayerChangedEventArgs e)
    {
        Debug.Log("Got playerchange event");

        if (!isServer)
            return;

        if (e.RegisteredChange == PlayerChangedEventArgs.Change.Connected)
        {
            if (e.CurrentNumberOfPlayers == 2)
            {
                //RpcDistributeField("####################             ##### ########       ####### #      ######");
                var fieldDef = @"
####    ##     ######
    ####   ##   ##
                     
                     
                     
####                 
                ####
#### ########    
####     ##    ######
    ####    ##   ##
                     
                     
                     
####                 
                ####
############     
";
                RpcDistributeField(fieldDef);

                GameStarted?.Invoke(this, new EventArgs());
            }
        }
    }

    [ClientRpc]
    void RpcDistributeField(string field)
    {
        if (field == null) return;

        //w = 20, h = 16
        var pos = 0;
        for (var i = 0; i < 21 * 16 && pos < field.Length; i++, pos++)
        {
            var ch = field[pos];

            if (ch == '\n' || ch == '\r')
            {
                i--;
                continue;
            }

            var l = i / 21;
            var c = i % 21;
            l -= 8;
            l *= -1;
            c -= 10;

            if (!AllInRange(-1, 1, l, c) && ch == '#')
            {
                Instantiate(ObstaclePrefab, new Vector3(c, 0, l), Quaternion.identity);
            }
        }
    }

    private bool AllInRange(int lower, int upper, params int[] xs)
    {
        foreach (var x in xs)
        {
            if (!(x >= lower && x <= upper))
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isServer)
            {
                Debug.Log("Calling RpcServerEnded");
                RpcServerEnded();
            } else
            {
                Debug.Log("Calling CmdClientEnded");
                CmdClientEnded();
            }
            //Thread.Sleep(1000);
            //_networkManager.End();
        }
    }

    [ClientRpc]
    void RpcServerEnded()
    {
        Debug.Log("RpcServerEnded called");
        _networkManager.End();
    }

    [Command]
    void CmdClientEnded()
    {
        Debug.Log("CmdClientEnded called");
        _networkManager.End();
    }
}
