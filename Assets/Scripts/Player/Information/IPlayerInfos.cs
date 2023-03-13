namespace Player.Information
{
    public interface IPlayerInfos
    {
        public string Uuid { get; set; }
        public string Pseudo { get; set; }
        public PlayerRole Role { get; set; }
    }
}