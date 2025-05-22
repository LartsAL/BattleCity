namespace Interfaces
{
    public interface IMapGenerator
    {
        public enum TileType
        {
            Empty = 0, // Reserved
            Floor = 1,
            Water = 2, // Reserved
            Wall = 3, // Random one: wood, brick or steel, but NOT map wall
            MapBorder = 4
        }

        public TileType[,] GenerateMap(int width, int height);
    }
}