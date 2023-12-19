namespace ChessLib
{
    public class Player : IPlayer
    {
        public int Id { get; }
        public string Name { get; }
        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public Player(string name) 
        {
            GenerateID.Player++;
            Id = GenerateID.Player;
            Name = name;
        }
    }
}
