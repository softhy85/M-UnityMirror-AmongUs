using Mirror;
using Player.Network.Escapist;
using UnityEngine;

namespace Network {
    public abstract class ANetworkManager : NetworkManager, INetworkManager
    {
        // // Client
        // public override void OnClientConnect(NetworkConnection conn)
        // {
        //     // A custom identifier we want to transmit from client to server on connection
        //     int id = GetCustomValue();
        //
        //     // Create message which stores our custom identifier
        //     IntegerMessage msg = newIntegerMessage(id);
        //
        //     // Call Add player and pass the message
        //     ClientScene.AddPlayer(conn,playerControllerId,msg);
        // }
        //
        // // Server
        // public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader )
        // {
        //     int id = 0;
        //     if (extraMessageReader != null)
        //     {
        //         var i = extraMessageReader.ReadMessage<IntegerMessage> ();
        //         id = i.value;
        //     }
        //     GameObject playerPrefab = GetPlayerPrefabById(id);
        //     GameObject player = (GameObject)Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        //     NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        // }
    }
}