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
    public class MarsRover 
    {
        public string Id { get; }
        public int Sight { get; }
        private Coordinate Position { get; set; }

        public IEnumerable<string> Resources { get; }
        public MarsRover(string id, int sight, Coordinate position, IEnumerable<string> resources) 
        { 
            Id = $"Rover-{id}";
            Sight = sight;
            Position = position;
            Resources = resources;
        }

        public Coordinate GetPosition()
        {
            return Position;
        }

        public void ChangeRoverPosition(Coordinate coordinate)
        {
            Position = coordinate;
        }
    } 

}
