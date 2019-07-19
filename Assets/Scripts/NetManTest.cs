using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManTest : NetworkManager
{
    private enum NetworkRole
    {
        None, Client, Host, Server
    }

    private NetworkClient _clientObj;
    private NetworkRole _myRole;

    public event EventHandler<PlayerChangedEventArgs> PlayerChanged;

    private int fstPlayerPos = -1;
    private int numPlayer = 0;

    public NetManTest() : base()
    {
        Debug.Log("I got created");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var playerSpawnPos = GetMyStartPosition();
        var player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        numPlayer++;

        Debug.Log("Got connection, sending event");

        PlayerChanged?.Invoke(this, new PlayerChangedEventArgs(PlayerChangedEventArgs.Change.Connected, numPlayer));
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        PlayerChanged?.Invoke(this,
            new PlayerChangedEventArgs(PlayerChangedEventArgs.Change.Disconnected, 0));

        End();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        End();
    }

    private Vector3 GetMyStartPosition()
    {
        if (numPlayer == 0)
        {
            fstPlayerPos = UnityEngine.Random.Range(0, 2);
            return startPositions[fstPlayerPos].position;
        }
        return startPositions[1 - fstPlayerPos].position;
    }

    public override NetworkClient StartHost()
    {
        _myRole = NetworkRole.Host;
        return _clientObj = base.StartHost(); 
    }

    public NetworkClient NMStartClient()
    {
        _myRole = NetworkRole.Client;
        return _clientObj = StartClient();
    }

    public bool NMStartServer()
    {
        _myRole = NetworkRole.Server;
        return StartServer();
    }

    public void End()
    {
        switch (_myRole)
        {
            case NetworkRole.None:
                return;
            case NetworkRole.Client:
                StopClient();
                break;
            case NetworkRole.Host:
                StopHost();
                break;
            case NetworkRole.Server:
                StopServer();
                break;
        }

        _clientObj = null;
        _myRole = NetworkRole.None;
        numPlayer = 0;
        fstPlayerPos = -1;
    }
}

public class PlayerChangedEventArgs : EventArgs
{
    public enum Change
    {
        Connected, Disconnected
    }

    public Change RegisteredChange { get; set; }
    public int CurrentNumberOfPlayers { get; set; }

    public PlayerChangedEventArgs(Change registeredChange, int currentNumberOfPlayers)
    {
        RegisteredChange = registeredChange;
        CurrentNumberOfPlayers = currentNumberOfPlayers;
    }
}