using Mirror;

namespace Network {
    public interface INetworkManager
    {
        public void OnServerAddPlayer(NetworkConnectionToClient conn);
    }
}