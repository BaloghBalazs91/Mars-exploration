using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Exploration;

public class OutcomeAnalyzer : IOutComeAnalyzer
{
    private readonly Map _map;
    private readonly IOutOfMapChecker _outsideMapChecker;
    private readonly ILogger _logger;
    private readonly ILogger _fileLogger;
    public OutcomeAnalyzer(Map map, IOutOfMapChecker outsideMapChecker)
    {
        _map = map;
        _outsideMapChecker = outsideMapChecker;
        _logger = new ConsoleLogger();
        _fileLogger = new FileLogger();
    }

    public bool TimeOut(int currentStep, int maximumSteps)
    {
        if(currentStep >= maximumSteps)
        {
            _logger.LogInfo("Exploration has timed out! No more steps allowed!");
            _fileLogger.LogInfo("Exploration has timed out! No more steps allowed!");
            return true;
        }
        return false;
    }

    public bool Success(Dictionary<Coordinate, string> foundResources)
    {
        bool success =
            FoundMineralWithinFiveEmptyCoordinatesOfWater(foundResources)
            || FourMineralsAndThreeWaterInTotal(foundResources);

        return success;
    }

    private bool FourMineralsAndThreeWaterInTotal(Dictionary<Coordinate, string> foundResources)
    {
        int waterFindings = 0;
        int mineralFindings = 0;

        foreach (var keyValuePair in foundResources)
        {
            if (keyValuePair.Value == "*")
            {
                waterFindings++;
            }
            else if (keyValuePair.Value == "%")
            {
                mineralFindings++;
            }
        }

        if (waterFindings >= 3 && mineralFindings >= 4)
        {
            _logger.LogInfo("There are at least 3 waters and 4 minerals in total!");
            _fileLogger.LogInfo("There are at least 3 waters and 4 minerals in total!");
            return true;
        }

        return false;
    }

    private bool FoundMineralWithinFiveEmptyCoordinatesOfWater(Dictionary<Coordinate, string> foundResources)
    {
        List<Coordinate> waterSpots = new List<Coordinate>();
        List<Coordinate> mineralSpots = new List<Coordinate>();

        foreach (var keyValuePair in foundResources)
        {
            if (keyValuePair.Value == "*")
            {
                waterSpots.Add(keyValuePair.Key);
            }
            if (keyValuePair.Value == "%")
            {
                waterSpots.Add(keyValuePair.Key);
            }
        }

        foreach(Coordinate waterCoordinate in waterSpots)
        {
            foreach(Coordinate mineralCoordinate in mineralSpots)
            {
                if(WithinManhattanDistance(waterCoordinate, mineralCoordinate, 5) && PathIsClear(waterCoordinate, mineralCoordinate))
                {
                    _logger.LogInfo("There are at least 1 mineral within 5 empty spots from water!");
                    _fileLogger.LogInfo("There are at least 1 mineral within 5 empty spots from water!");
                    return true;
                }
            }
        }

        return false;
    }

    private bool WithinManhattanDistance(Coordinate A, Coordinate B, int distance)
    {
        int distX = Math.Abs(A.X - B.X);
        int distY = Math.Abs(A.Y - B.Y);

        int manhattanDistance = distX + distY;

        return manhattanDistance <= distance;
    }

    private bool PathIsClear(Coordinate A, Coordinate B)
    {
        int minX = Math.Min(A.X, B.X);
        int minY = Math.Min(A.Y, B.Y);
        int maxX = Math.Max(A.X, B.X);
        int maxY = Math.Max(A.Y, B.Y);

        for(int i = minX; i <= maxX; i++)
        {
            for(int j = minY; j <= maxY; j++)
            {
                if (_map.Representation[i, j] != " ")
                {
                    return false;
                }
            }
        }
        return true;
    }
}