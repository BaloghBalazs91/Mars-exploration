using Codecool.MarsExploration.MapExplorer.Rover;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Exploration;

public record SimulationContext(
    int CurrentStep,
    int MaximumSteps, 
    MarsRover Rover, 
    Coordinate SpaceshipLocation, 
    Map Map, 
    IEnumerable<string> Resources, 
    ExplorationOutcome ExplorationOutcome);