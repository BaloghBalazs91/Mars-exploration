using Codecool.MarsExploration.MapExplorer.Rover;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Exploration
{
    internal interface ISimulationStepper
    {
        Dictionary<Coordinate, string> CoordinatesByFoundResources { get; }
        void Move(MarsRover rover);

        void Scan(MarsRover rover);

        ExplorationOutcome Analize(MarsRover rover, int currentStep, int maximumSteps);

        int IncrementSimulationStep(int currentStep);
    }
}
