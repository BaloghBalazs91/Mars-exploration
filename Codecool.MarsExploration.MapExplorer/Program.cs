using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using Codecool.MarsExploration.MapExplorer.Configuration;
using Codecool.MarsExploration.MapExplorer.Validator;
using Codecool.MarsExploration.MapExplorer.Rover;
using Codecool.MarsExploration.MapExplorer.Exploration;

namespace Codecool.MarsExploration.MapExplorer;

class Program
{
    private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;

    public static void Main(string[] args)
    {

        ExplorationSimulator simulator = new ExplorationSimulator();
        simulator.Simulate();
    }
}
