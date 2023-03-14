using Mirror;
using Network;

namespace TCP
{
    public class TCPNetworkManager : ANetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
        }
    }
    
}