using Mirror;
using Network;
using Player.Network;

namespace UDP
{
    public class UDPNetworkManager : ANetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
        }
    }
    
}