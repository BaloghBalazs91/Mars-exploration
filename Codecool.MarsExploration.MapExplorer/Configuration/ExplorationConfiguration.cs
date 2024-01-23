using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Configuration
{
    public record ExplorationConfiguration(string mapFile, Coordinate landingSpot, IEnumerable<string> resources, int simulationSteps);
}
