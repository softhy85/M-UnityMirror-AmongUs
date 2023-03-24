using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Lobbying
{

    [System.Serializable]
    public class Room
    {
        public string roomName;
        public string password;
        public List<LocalClient> clientsInRoom = new List<LocalClient>();

        public Room(string roomName, string password, LocalClient hostClient)
        {
            this.roomName = roomName;
            this.password = password;
            this.clientsInRoom.Add(hostClient);
        }

        public bool isPublic() { return password.Length == 0; }

        public Room() { }
    }

    public class RoomManager : NetworkBehaviour
    {

        public static RoomManager instance;

        public readonly SyncList<Room> rooms = new SyncList<Room>();
        public readonly SyncList<string> roomNames = new SyncList<string>();

        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
                instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("RoomManager Instance created");
        }


        public bool HostNewRoom(string roomName, LocalClient hostClient, string password)
        {
            if (!roomNames.Contains(roomName))
            {
                roomNames.Add(roomName);
                Room room = new Room(roomName, password, hostClient);
                rooms.Add(room);
                return true;
            }
            else
            {
                Debug.Log($"This room name is already taken !");
                return false;
            }
        }

        public bool JoinRoom(string roomName, LocalClient localClient)
		{
            if (roomNames.Contains(roomName)) {
                for (int i = 0; i < rooms.Count; i++)
                    if (rooms[i].roomName == roomName) {

                        // TODO Check if client can join room (full, in-game, correct password)

                        rooms[i].clientsInRoom.Add(localClient);
                        localClient.currentRoom = rooms[i];

                        break;
                    }

                Debug.Log($"Room joined");
                return true;
            }
            else
            {
                Debug.Log($"No room with this name exists");
                return false;
            }
        }

        public void test()
		{
            Debug.Log("RoomManager Test");
		}

    }

}