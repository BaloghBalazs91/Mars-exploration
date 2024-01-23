using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Exploration
{
    internal class OutOfMapChecker : IOutOfMapChecker
    {
        private readonly Map _map;

        public OutOfMapChecker(Map map)
        {
            _map = map;
        }

        public bool IsWithinMap(Coordinate coordinate)
        {
            int mapSize = _map.Representation.GetLength(0);
            bool coordinateIsWithinMap = coordinate.X < mapSize && coordinate.X >= 0 && coordinate.Y < mapSize && coordinate.Y >= 0;
            
            return coordinateIsWithinMap;
        }
    }
}
