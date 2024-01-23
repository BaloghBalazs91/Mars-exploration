using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.MapLoader
{
    public class MapReader : IMapLoader
    {
        /// <summary>
        /// This method loads and creates a Map type from a .map file.
        /// </summary>
        /// <param name="mapFile">The filepath of mapfile.</param>
        /// <returns>Return a Map type.</returns>
        /// <exception cref="ArgumentException">All lines in the map file must have the same length.</exception>
        public Map Load(string mapFile)
        {
            string[] lines = File.ReadAllLines(mapFile);

            int mapWidth = lines[0].Length;
            int mapHeight = lines.Length;
            string[,] mapRepresentation = new string[mapWidth, mapHeight];

            for (int i = 0; i < mapHeight; i++)
            {
                string line = lines[i];

                if (line.Length != mapWidth)
                {
                    // Handle the case where lines have different lengths, if necessary
                    throw new ArgumentException("All lines in the map file must have the same length.");
                }

                for (int j = 0; j < mapWidth; j++)
                {
                    mapRepresentation[i, j] = line[j].ToString(); // Convert the character to a string
                }
            }

            return new Map(mapRepresentation, true);
        }
    }
}
