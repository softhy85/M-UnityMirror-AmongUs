using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Security.Cryptography;
using System.Text;

namespace Lobbying
{

    [RequireComponent(typeof(NetworkMatch))]
    public class LocalClient : NetworkBehaviour
    {

        public static LocalClient localClient;
        NetworkMatch networkMatch;
        [SyncVar] public int playerIndex;
        [SyncVar] public Room currentRoom;
        Guid netIDGuid;


        void Awake()
        {
            networkMatch = GetComponent<NetworkMatch>();
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStartServer()
        {
            netIDGuid = new Guid(netId.ToString());
            networkMatch.matchId = netIDGuid;
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                if (localClient == null)
                    localClient = this;
            }
            else
            {
                Debug.Log($"Spawning other player UI Prefab");
                //playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this);
            }
        }


		#region HOST

		public void HostGame(string roomName, string password)
        {
            CmdHostGame(roomName, password);
        }

        [Command]
        void CmdHostGame(string roomName, string password)
        {
            if (RoomManager.instance.HostNewRoom(roomName, this, password))
            {
                Debug.Log($"<color=green>Game hosted successfully</color>");
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(roomName));
                    Guid roomGuid = new Guid(hash);
                    networkMatch.matchId = roomGuid;
                }
                TargetHostGame(true, roomName, password.Length > 0);
            }
            else
            {
                Debug.Log($"<color=red>Game hosting failed</color>");
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string roomName, bool isPrivate)
        {
            MainMenuActions.instance.HostSuccess(success, roomName, isPrivate);
        }


        #endregion


        #region Join

        public void JoinRoom(string roomName)
        {
            CmdJoinGame(roomName);
        }

        [Command]
        void CmdJoinGame(string roomName)
        {
            if (RoomManager.instance.JoinRoom(roomName, this))
            {
                Debug.Log($"<color=green>Game joined successfully</color>");
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(roomName));
                    Guid roomGuid = new Guid(hash);
                    networkMatch.matchId = roomGuid;
                }
                TargetJoinRoom(true, roomName);
            }
            else
            {
                Debug.Log($"<color=red>Game joined failed</color>");
                TargetJoinRoom(false, roomName);
            }
        }

        [TargetRpc]
        void TargetJoinRoom(bool success, string roomName)
        {
            MainMenuActions.instance.JoinSuccess(success, roomName);
        }


        #endregion

        public void test()
		{
            Debug.Log("LocalClient test");
		}

    }

}