using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Paddle : NetworkBehaviour
{
    // how fast the paddle can move
    public float MoveSpeed = 10f;

    // how far up and down the paddle can move
    public float MoveRange = 10f;

    void Update()
    {
        // does not accept input, abort
        if (!isLocalPlayer)
            return;

        //get user input
        float input = Input.GetAxis("Vertical");

        // move paddle
        Vector3 pos = transform.position;
        pos.z += input * MoveSpeed * Time.deltaTime;

        // clamp paddle position
        pos.z = Mathf.Clamp(pos.z, -MoveRange, MoveRange);

        // set position
        CmdUpdatePlayerPos(pos, GetComponent<NetworkIdentity>());
        transform.position = pos;
    }

    [Command]
    void CmdUpdatePlayerPos(Vector3 position, NetworkIdentity identity)
    {
        RpcDistributePlayerPos(position, identity);
    }

    [ClientRpc(channel =1)]
    void RpcDistributePlayerPos(Vector3 position, NetworkIdentity identity)
    {
        if (!isLocalPlayer && GetComponent<NetworkIdentity>().netId.Equals(identity.netId))
        {
            transform.position = position;
        }
    }
}
