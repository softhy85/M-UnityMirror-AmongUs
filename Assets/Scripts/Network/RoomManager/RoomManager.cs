using System;
using System.Collections.Generic;
using Mirror;
using Player.Room;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Network {
    public class RoomManager : NetworkRoomManager, IRoomManager
    {
        private bool host = false;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            if (conn.identity.TryGetComponent<RoomPlayer>(out var player))
            {
                if (!host)
                {
                    host = true;
                    player.SetHosting(true);
                }
            }
        }


        public override void OnClientConnect()
        {
            base.OnClientConnect();
            GameObject[] roomPlayers = GameObject.FindGameObjectsWithTag("RoomPlayer");
            
            for (int i = 0; i < roomPlayers.Length; i++)
            {
                RoomPlayer roomPlayer = roomPlayers[i].GetComponent<RoomPlayer>();
            }
        }

    }
}