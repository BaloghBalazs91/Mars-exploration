using Codecool.MarsExploration.MapExplorer.Configuration;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Rover
{
    internal class RoverDeployer : IRoverDeployer
    {
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private readonly ExplorationConfiguration _configuration;

        private readonly Map _map;

        public RoverDeployer(ExplorationConfiguration configuration, Map map)
        {
            _configuration = configuration;
            _map = map;
        }

        /// <summary>
        /// This method responsible for rover creation.
        /// </summary>
        /// <param name="id">Id of rover.</param>
        /// <param name="sight">The sight, meaning how far away the rover can see.</param>
        /// <returns>Return MarsRover type rover.</returns>
        public MarsRover CreateRover(string id, int sight)
        {
            return new MarsRover(id, sight, GetStartingPoint(_configuration.landingSpot), _configuration.resources);
        }

        /// <summary>
        /// This method deploys the rover and the spaceship on the map by marking their spot.
        /// </summary>
        /// <param name="rover">MarsRover type rover.</param>
        public void DeployRoverOnMap(MarsRover rover)
        {
            // mark spaceship on map
            _map.Representation[_configuration.landingSpot.X, _configuration.landingSpot.Y] = "S";
            // mark rover on map
            _map.Representation[rover.GetPosition().X, rover.GetPosition().Y] = "R";
        }

        /// <summary>
        /// This method defines the startingpoint of rover for exploration in regards of landingspot.
        /// </summary>
        /// <param name="point">Coordinate type, landingspot of spaceship.</param>
        /// <returns>Returns a Coordinate type as startingpoint.</returns>
        private Coordinate GetStartingPoint(Coordinate point)
        {
            IEnumerable<Coordinate> adjacentAreas = new List<Coordinate>()
            {
                new Coordinate(point.X - 1, point.Y),
                new Coordinate(point.X + 1, point.Y),
                new Coordinate(point.X, point.Y + 1),
                new Coordinate(point.X, point.Y - 1),
                new Coordinate(point.X - 1, point.Y + 1),
                new Coordinate(point.X + 1, point.Y + 1),
                new Coordinate(point.X - 1, point.Y - 1),
                new Coordinate(point.X + 1, point.Y - 1)
            };

            List<Coordinate> emptySpots = adjacentAreas.Where(coordinate => _map.Representation[coordinate.X, coordinate.Y] == " ").ToList();

            int randomIndex = _random.Next(emptySpots.Count);
            Coordinate startingPoint = emptySpots[randomIndex];

            return startingPoint;
        }
    }
}
