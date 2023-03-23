using JetBrains.Annotations;
using Mirror;
using Player.Behaviour;
using Player.Behaviour.Escapist;
using Player.Behaviour.Monster;
using Player.Information;
using Player.Information.Structure;
using Player.Room;
using UnityEngine;

namespace Network {
    public class RoomManager : NetworkRoomManager
    {
        private bool host = false;
        [Header("Game")]
        public PlayerPrefab[] playerPrefabs;


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

        public override GameObject OnRoomServerCreateGamePlayer(
            NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            Debug.Log("OnRoomServerCreateGamePlayer");
            PlayerRole role = roomPlayer.GetComponent<RoomPlayer>().GetRole();
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].role == role)
                {
                    Debug.Log("i - " + i);
                    Debug.Log("role - " + playerPrefabs[i].role);
                    Debug.Log("prefab - " + playerPrefabs[i].prefab);
                    if (playerPrefabs[i].prefab) {
                        Debug.Log("Test 1");
                        GameObject player = Instantiate(playerPrefabs[i].prefab);
                        return (player);
                    }
                    else
                    {
                        Debug.Log("Test 2");
                    }
                }
            }

            return (null);
        }
    }
}