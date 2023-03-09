using Mirror;
using UnityEngine;

namespace Network {
    public abstract class ANetworkManager : NetworkManager, INetworkManager
    {
        public void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
        }
    }
}