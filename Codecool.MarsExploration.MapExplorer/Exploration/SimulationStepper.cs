using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.Rover;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Exploration
{
    internal class SimulationStepper : ISimulationStepper
    {
        private readonly Map _map;
        private readonly IOutComeAnalyzer _analyzer;
        private readonly IOutOfMapChecker _outOfMapChecker;
        private readonly ILogger _fileLogger;
        private readonly ILogger _consoleLogger;
        private readonly List<Coordinate> _visitedCoordinates = new List<Coordinate>();
        private readonly Random _random;
        public Dictionary<Coordinate, string> CoordinatesByFoundResources { get; }
        public SimulationStepper(Map map, ILogger fileLogger, ILogger logger)
        {
            _map = map;
            _outOfMapChecker = new OutOfMapChecker(_map);
            _analyzer = new OutcomeAnalyzer(_map, _outOfMapChecker);
            _fileLogger = fileLogger;
            _consoleLogger = logger;
            CoordinatesByFoundResources = new Dictionary<Coordinate, string>();
            _random = new Random();
        }

        public ExplorationOutcome Analize(MarsRover rover, int currentStep, int maximumSteps)
        {
            if (_analyzer.TimeOut(currentStep, maximumSteps) && !_analyzer.Success(CoordinatesByFoundResources))
            {
                ReturnToSpaceShip(rover);
                return ExplorationOutcome.Timeout;

            }
            if (_analyzer.Success(CoordinatesByFoundResources))
            {
                ReturnToSpaceShip(rover);
                return ExplorationOutcome.Colonizable;
            }
            
            return ExplorationOutcome.Continue;
        }

        public int IncrementSimulationStep(int currentStep)
        {
            ++currentStep;
            _fileLogger.LogInfo($"Simulation step incremented, current step count is: {currentStep}");
            _consoleLogger.LogInfo($"Simulation step incremented, current step count is: {currentStep}");
            return currentStep;
        }

        public void Move(MarsRover rover)
        {
            List<Coordinate> emptySpots = GetEmptyAdjacentAreas(rover).ToList();

            // Prioritize unexplored areas
            Coordinate nextDestination = FindNextUnexploredCoordinate(rover, emptySpots);

            if (nextDestination != null)
            {
                MoveRoverToCoordinate(nextDestination, rover);
            }
            else
            {
                // If no unexplored areas nearby, backtrack to a previous coordinate
                Coordinate previousCoordinate = BacktrackToPreviousCoordinate();

                if (previousCoordinate != null)
                {
                    MoveRoverToCoordinate(previousCoordinate, rover);
                }
                else
                {
                    // Handle the case where there are no previous coordinates to backtrack to
                    // This can occur if the rover is stuck and has explored the entire map
                    _fileLogger.LogError($"{rover.Id} is stuck and has explored the entire map.");
                    _consoleLogger.LogError($"{rover.Id} is stuck and has explored the entire map.");

                    // Add a return statement to exit the method
                    return;
                }
            }
        }

        private void ReturnToSpaceShip(MarsRover rover) 
        {
            _consoleLogger.LogInfo($"{rover.Id} is returning to spaceship!");
            _fileLogger.LogInfo($"{rover.Id} is returning to spaceship!");
            List<Coordinate> uniqueVisitedCoordinates = _visitedCoordinates.Distinct().ToList();
            for (int i = uniqueVisitedCoordinates.Count - 1; i >= 0; i--)
            {
                MoveRoverToCoordinate(_visitedCoordinates[i], rover);
            }
            _consoleLogger.LogInfo($"{rover.Id} has returned to spaceship!");
            _fileLogger.LogInfo($"{rover.Id} has returned to spaceship!");
        }

        private Coordinate BacktrackToPreviousCoordinate()
        {
            // Get the current position of the rover
            List<Coordinate> uniqueVisitedCoordinates = _visitedCoordinates.Distinct().ToList();
            // Find the last visited coordinate in the movement history
            for (int i = uniqueVisitedCoordinates.Count - 1; i >= 0; i--)
            {
                Coordinate visitedCoordinate = uniqueVisitedCoordinates[i];

                // Check if the visited coordinate has adjacent unexplored areas
                List<Coordinate> adjacentEmptySpots = GetEmptyAdjacentAreas(visitedCoordinate);

                if (adjacentEmptySpots.Count() > 0)
                {
                    // Return the last visited coordinate with adjacent unexplored areas
                    Coordinate nextDestination = adjacentEmptySpots[_random.Next(adjacentEmptySpots.Count())];
                    return nextDestination;
                }
            }

            // If no suitable previous coordinate is found, return null
            return null;
        }


        private void MoveRoverToCoordinate(Coordinate coordinate, MarsRover rover)
        {
            Coordinate roverPosition = rover.GetPosition();
            //Make previous position empty
            _map.Representation[roverPosition.X, roverPosition.Y] = "-";
            //Record the previous position as visited location
            _visitedCoordinates.Add(roverPosition);
            //Change rover's position to new coordinate
            rover.ChangeRoverPosition(coordinate);
            //Mark rover's position on the map
            _map.Representation[rover.GetPosition().X, rover.GetPosition().Y] = "R";

            _fileLogger.LogInfo($"{rover.Id} moved from [X:{roverPosition.X}, Y:{roverPosition.Y}] to [X:{coordinate.X}, Y:{coordinate.Y}]");
            _consoleLogger.LogInfo($"{rover.Id} moved from [X:{roverPosition.X}, Y:{roverPosition.Y}] to [X:{coordinate.X}, Y:{coordinate.Y}]");
        }

        private Coordinate FindNextUnexploredCoordinate(MarsRover rover, List<Coordinate> emptySpots)
        {
            Coordinate nextDestination = null;
            List<Coordinate> unexploredEmptyCoordinates = new List<Coordinate>();

            foreach (Coordinate emptyCoordinate in emptySpots)
            {
                if (!_visitedCoordinates.Contains(emptyCoordinate))
                {
                    unexploredEmptyCoordinates.Add(emptyCoordinate);
                    nextDestination = unexploredEmptyCoordinates[_random.Next(unexploredEmptyCoordinates.Count)];
                }
            }

            return nextDestination;
        }


        public void Scan(MarsRover rover)
        {
            IEnumerable<Coordinate> scanableArea = GetAdjacentAreasBySight(rover);

            foreach (Coordinate coordinate in scanableArea)
            {
                _fileLogger.LogInfo($"Scanning area at [X:{coordinate.X}, Y:{coordinate.Y}]");
                _consoleLogger.LogInfo($"Scanning area at [X:{coordinate.X}, Y:{coordinate.Y}]");
                foreach (string resource in rover.Resources)
                {
                    if (_map.Representation[coordinate.X, coordinate.Y] == resource && !CoordinatesByFoundResources.ContainsKey(coordinate))
                    {
                        CoordinatesByFoundResources.Add(coordinate, resource);
                        _fileLogger.LogInfo($"{resource} found at [X:{coordinate.X}, Y:{coordinate.Y}]!");
                        _consoleLogger.LogInfo($"{resource} found at [X:{coordinate.X}, Y:{coordinate.Y}]!");
                    }
                }
            }
        }

        private IEnumerable<Coordinate> GetAdjacentAreasBySight(MarsRover rover)
        {
            Coordinate roverPosition = rover.GetPosition();

            Coordinate scanStartPosition = new Coordinate(roverPosition.X - rover.Sight, roverPosition.Y - rover.Sight);

            List<Coordinate> adjacentAreaBySight = new List<Coordinate>();

            int iterationLength = (2 * rover.Sight + 1);

            for (int i = scanStartPosition.X; i < scanStartPosition.X + iterationLength; i++)
            {
                for (int j = scanStartPosition.Y; j < scanStartPosition.Y + iterationLength; j++)
                {
                    Coordinate sightCoordinate = new Coordinate(i, j);
                    if (!adjacentAreaBySight.Contains(sightCoordinate) && _outOfMapChecker.IsWithinMap(sightCoordinate))
                    {
                        adjacentAreaBySight.Add(sightCoordinate);
                    }
                }
            }

            return adjacentAreaBySight;
        }
        
        private List<Coordinate> GetEmptyAdjacentAreas(MarsRover rover)
        {
            Coordinate roverPosition = rover.GetPosition();

            IEnumerable<Coordinate> adjacentArea = new List<Coordinate>()
            {
                new Coordinate(roverPosition.X - 1, roverPosition.Y),
                new Coordinate(roverPosition.X + 1, roverPosition.Y),
                new Coordinate(roverPosition.X, roverPosition.Y + 1),
                new Coordinate(roverPosition.X, roverPosition.Y - 1),
                new Coordinate(roverPosition.X - 1, roverPosition.Y + 1),
                new Coordinate(roverPosition.X + 1, roverPosition.Y + 1),
                new Coordinate(roverPosition.X - 1, roverPosition.Y - 1),
                new Coordinate(roverPosition.X + 1, roverPosition.Y - 1)
            };

            IEnumerable<Coordinate> filterAdjacentArea = adjacentArea.Where(coordinate => _outOfMapChecker.IsWithinMap(coordinate));

            List<Coordinate> emptySpots = filterAdjacentArea.Where(coordinate => _map.Representation[coordinate.X, coordinate.Y] == " ").ToList();
            return emptySpots;
        }

        private List<Coordinate> GetEmptyAdjacentAreas(Coordinate coordinate)
        {
            IEnumerable<Coordinate> adjacentArea = new List<Coordinate>()
            {
                new Coordinate(coordinate.X - 1, coordinate.Y),
                new Coordinate(coordinate.X + 1, coordinate.Y),
                new Coordinate(coordinate.X, coordinate.Y + 1),
                new Coordinate(coordinate.X, coordinate.Y - 1),
                new Coordinate(coordinate.X - 1, coordinate.Y + 1),
                new Coordinate(coordinate.X + 1, coordinate.Y + 1),
                new Coordinate(coordinate.X - 1, coordinate.Y - 1),
                new Coordinate(coordinate.X + 1, coordinate.Y - 1)
            };

            IEnumerable<Coordinate> filterAdjacentArea = adjacentArea.Where(coordinate => _outOfMapChecker.IsWithinMap(coordinate));

            List<Coordinate> emptySpots = filterAdjacentArea.Where(coordinate => _map.Representation[coordinate.X, coordinate.Y] == " ").ToList();
            return emptySpots;
        }
    }
}
