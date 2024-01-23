using Codecool.MarsExploration.MapExplorer.Configuration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Validator
{
    internal class ConfigurationValidator : IConfigurationValidator
    {
        private readonly Map map;
        private readonly ILogger ConsoleLogger = new ConsoleLogger();
        private readonly ILogger FileLogger = new FileLogger();


        public ConfigurationValidator(Map map)
        {
            this.map = map;
        }

        /// <summary>
        /// This method checks if all the criteria for a valid configuration has been met.
        /// </summary>
        /// <param name="configuration">Configuration of exploration.</param>
        /// <returns>Returns a boolean.</returns>
        public bool IsConfigurationValid(ExplorationConfiguration configuration)
        {
            bool allCriteriaFulfilled = LandingSpotEmpty(configuration)
                && LandingAreaTraversable(configuration)
                && MapFileExisting(configuration)
                && ResourcesDefined(configuration)
                && TimeOutGreaterThanZero(configuration);
            
            if (allCriteriaFulfilled)
            {
                ConsoleLogger.LogInfo("The configuration of exploration is valid!");
                FileLogger.LogInfo("The configuration of exploration is valid!");
            }

            return allCriteriaFulfilled;
        }

        /// <summary>
        /// This method checks if the specified landing spot for the spaceship is empty or not.
        /// </summary>
        /// <param name="configuration">Configuration of exploration.</param>
        /// <returns>Returns a boolean.</returns>
        private bool LandingSpotEmpty(ExplorationConfiguration configuration)
        {
            if (map.Representation[configuration.landingSpot.X, configuration.landingSpot.Y] != " ")
            {
                ConsoleLogger.LogError("Landing spot is not empty!");
                FileLogger.LogError("Landing spot is not empty!");
            }

            return map.Representation[configuration.landingSpot.X, configuration.landingSpot.Y] == " ";
        }

        /// <summary>
        /// This method checks if the area surrounding the landingspot has any empty coordinate.
        /// </summary>
        /// <param name="configuration">Configuration of exploration.</param>
        /// <returns>Returns a boolean.</returns>
        private bool LandingAreaTraversable(ExplorationConfiguration configuration)
        {
            IEnumerable<Coordinate> adjacentAreas = new List<Coordinate>()
            {
                new Coordinate(configuration.landingSpot.X - 1, configuration.landingSpot.Y),
                new Coordinate(configuration.landingSpot.X + 1, configuration.landingSpot.Y),
                new Coordinate(configuration.landingSpot.X, configuration.landingSpot.Y + 1),
                new Coordinate(configuration.landingSpot.X, configuration.landingSpot.Y - 1),
                new Coordinate(configuration.landingSpot.X - 1, configuration.landingSpot.Y + 1),
                new Coordinate(configuration.landingSpot.X + 1, configuration.landingSpot.Y + 1),
                new Coordinate(configuration.landingSpot.X - 1, configuration.landingSpot.Y - 1),
                new Coordinate(configuration.landingSpot.X + 1, configuration.landingSpot.Y - 1)
            };

            if (!adjacentAreas.Any(coordinate => map.Representation[coordinate.X, coordinate.Y] == " "))
            {
                ConsoleLogger.LogError($"Surrounding area of landing spot is not traversable!");
                FileLogger.LogError("Surrounding area of landing spot is not traversable!");
            }

            return adjacentAreas.Any(coordinate => map.Representation[coordinate.X, coordinate.Y] == " ");
        }

        /// <summary>
        /// This method checks if the mapfile specified in the configuration exists.
        /// </summary>
        /// <param name="configuration">Configuration of exploration.</param>
        /// <returns>Returns a boolean.</returns>
        private bool MapFileExisting(ExplorationConfiguration configuration)
        {
            if (!File.Exists(configuration.mapFile))
            {
                ConsoleLogger.LogError($"The file path does not exist: {configuration.mapFile}!");
                FileLogger.LogError($"The file path does not exist: {configuration.mapFile}!");
            }
            return File.Exists(configuration.mapFile);
        }

        /// <summary>
        /// This method checks if the defined resources collection is not empty in the configuration.
        /// </summary>
        /// <param name="configuration">Configuration of exploration.</param>
        /// <returns>Returns a boolean.</returns>
        private bool ResourcesDefined(ExplorationConfiguration configuration)
        {
            if (configuration.resources.Count() <= 0)
            {
                ConsoleLogger.LogError("Simulation steps can not be equal or less than 0!");
                FileLogger.LogError("Simulation steps can not be equal or less than 0!");
            }
            return configuration.resources.Count() > 0 ? true : false;
        }

        /// <summary>
        /// This method checks whether the number of simulation steps is greater than zero.
        /// </summary>
        /// <param name="configuration">Configuration of exploration.</param>
        /// <returns>Returns a boolean.</returns>
        private bool TimeOutGreaterThanZero(ExplorationConfiguration configuration)
        {
            if (configuration.simulationSteps <= 0)
            {
                ConsoleLogger.LogError("Timeout can not be equal or less than 0!");
                FileLogger.LogError("Timeout can not be equal or less than 0!");
            }
            return configuration.simulationSteps > 0 ? true : false;
        }
    }
}
