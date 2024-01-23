using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Rover
{
    internal interface IRoverDeployer
    {
        MarsRover CreateRover(string id, int sight);

        void DeployRoverOnMap(MarsRover rover);
        
    }
}
