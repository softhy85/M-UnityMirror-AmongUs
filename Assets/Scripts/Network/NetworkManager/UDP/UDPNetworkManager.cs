using Mirror;
using Network;

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