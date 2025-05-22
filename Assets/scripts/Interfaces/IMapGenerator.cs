namespace Interfaces
{
    public interface IMapGenerator
    {
        public enum TileType
        {
            Empty = 0, // Reserved
            Placeholder = 1,
            Floor = 2,
            Water = 3, // Reserved
            Wall = 4, // Random one: wood, brick or steel, but NOT map wall
            MapBorder = 5
        }

        public TileType[,] GenerateMap(int width, int height);
    }
}