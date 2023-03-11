using Player.Behaviour;

namespace Player.Network
{
    public interface IPlayerNetwork
    {
        public APlayerBehaviour PlayerBehaviour { get; set; }
    }
}