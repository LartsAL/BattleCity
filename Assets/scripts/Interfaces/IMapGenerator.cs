using Common;

namespace Interfaces
{
    public interface IMapGenerator
    {
        public TileType[,] GenerateMap(int width, int height);
    }
}