using Codecool.MarsExploration.MapExplorer.Configuration;
using Codecool.MarsExploration.MapExplorer.Exploration.DBRepository;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.Rover;
using Codecool.MarsExploration.MapExplorer.Validator;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using System.Runtime.CompilerServices;

namespace Codecool.MarsExploration.MapExplorer.Exploration;

public class ExplorationSimulator
{
    private readonly Random _random = new Random();
    private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;

    // Navigate to the parent directory of the current project
    private static readonly string ParentDir = Path.Combine(WorkDir, "..", "..", "..", "..", "Codecool.MarsExploration.MapGenerator");
    private readonly Map _map;
    private readonly int _maxSteps = 500;
    private int _currentStep = 0;
    private readonly int _sight = 4;
    private int _roverCount = 1;
    private readonly IEnumerable<string> _resources = new List<string>() { "*", "%" };
    private readonly Coordinate _landingSpot;
    private readonly string _mapFile;
    private readonly ExplorationConfiguration _configuration;
    private readonly IConfigurationValidator _configurationValidator;
    private readonly IRoverDeployer _roverDeployer;
    private readonly MarsRover _rover;
    private readonly ISimulationStepper _simulationStepper;
    private readonly ILogger _logger;
    private readonly ILogger _fileLogger;
    private readonly IExplorationLogRepository _logRepository;
    private readonly string _dbFilePath;

    public ExplorationSimulator()
    {
        _dbFilePath = Path.Combine(WorkDir, "Resources", "explorationlog.db");
        _mapFile = Path.Combine(ParentDir, "bin", "Debug", "net6.0", $"exploration-{_random.Next(3)}.map");
        _map = ReadFile(_mapFile);
        _landingSpot = RandomLandingSpotGenerator(_map);
        _configuration = new ExplorationConfiguration(_mapFile, _landingSpot, _resources, _maxSteps);
        _roverDeployer = new RoverDeployer(_configuration, _map);
        _configurationValidator = new ConfigurationValidator(_map);
        string roverId = $"{_roverCount}";
        _rover = Deploy(_configuration, _map, _sight, roverId);
        _logger = new ConsoleLogger();
        _fileLogger = new FileLogger();
        _simulationStepper = new SimulationStepper(_map, _fileLogger, _logger);

        Dictionary<Coordinate, string> _foundResourcesByCoordinates = new Dictionary<Coordinate, string>();
        _logRepository = new ExplorationLogRepository(_dbFilePath);
    }
    /// <summary>
    /// Runs a loop of the simulation, based on the given context which contains the specific data.
    /// Logs the outcome of events.
    /// </summary>
    public void Simulate()
    {
        ExplorationOutcome outcome = ExplorationOutcome.Continue;

        while (outcome == ExplorationOutcome.Continue)
        {
            _simulationStepper.Move(_rover);
            _simulationStepper.Scan(_rover);
            outcome = _simulationStepper.Analize(_rover, _currentStep, _maxSteps);
            _currentStep = _simulationStepper.IncrementSimulationStep(_currentStep);
            _logger.LogInfo(outcome.ToString());
        } 
        SimulationLogger(_currentStep, _rover, outcome, _simulationStepper.CoordinatesByFoundResources);
        _logger.LogInfo($"\n {_map}");
    }

    private SimulationContext GetContext()
    {
        ExplorationOutcome outcome = ExplorationOutcome.Colonizable;
        _roverCount++;

        return new SimulationContext(_currentStep, _maxSteps, _rover, _configuration.landingSpot, _map, _resources, outcome);
    }

    private Map ReadFile(string mapFile)
    {
        IMapLoader maploader = new MapReader();
        Map map = maploader.Load(mapFile);

        return map;
    }

    private MarsRover Deploy(ExplorationConfiguration config, Map map, int sight, string id)
    {
        do
        {
            config = new ExplorationConfiguration(_mapFile, RandomLandingSpotGenerator(map), _resources, _maxSteps);

        } while (!_configurationValidator.IsConfigurationValid(config));

        MarsRover rover = _roverDeployer.CreateRover(id, sight);
        _roverDeployer.DeployRoverOnMap(rover);

        return rover;
    }

    private Coordinate RandomLandingSpotGenerator(Map map)
    {
        int mapLength = map.Representation.GetLength(0);
        Coordinate coordinate = new Coordinate(_random.Next(2, mapLength - 2), _random.Next(2, mapLength - 2));
        return coordinate;
    }

    private void SimulationLogger(int currentStep, MarsRover rover, ExplorationOutcome outcome, Dictionary<Coordinate, string> foundResources)
    {
        _logRepository.Add(currentStep, outcome.ToString(), DateTime.Now.ToString(), foundResources.Count);
    }
}
/*
Everything is in place to start working on the simulation engine.
The simulator must receive a configuration object, contact the necessary services,
and use the necessary methods to run the simulation.
You need to make some decisions about the implementation details of the simulator class. We recommend the following:

Generate the context first, then simulate the rover exploration runfter. (SLAP)
Generating the context implies loading the chart (reading the file),
validating the landing coordinates for the spaceship and deploying the rover in an empty coordinate adjacent to the spaceship.
Each one of these responsibilities can be in different classes (SLAP & SRP)

The simulation of the rover exploration run can be arranged as a loop that repeats until an outcome is found.
Each iteration of the loop can run a series of ordered simulation steps. (SRP & OCP)
 */