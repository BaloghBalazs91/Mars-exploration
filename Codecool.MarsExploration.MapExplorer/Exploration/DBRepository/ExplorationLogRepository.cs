using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Codecool.MarsExploration.MapExplorer.Exploration.DBRepository
{
    internal class ExplorationLogRepository : IExplorationLogRepository
    {
        private readonly string _dbFilePath;

        public ExplorationLogRepository(string dbFilePath)
        {
            _dbFilePath = dbFilePath;
        }

        private SqliteConnection GetPhysicalDbConnection()
        {
            var dbConnection = new SqliteConnection($"Data Source ={_dbFilePath};Mode=ReadWrite");
            dbConnection.Open();
            return dbConnection;
        }

        private void ExecuteNonQuery(string query)
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private static SqliteCommand GetCommand(string query, SqliteConnection connection)
        {
            return new SqliteCommand
            {
                CommandText = query,
                Connection = connection,
            };
        }

        public void Add(int stepCount, string outcome, string date, int resourcesFound)
        {
            var query = $"INSERT INTO explorationlog (stepCount, outcome, timeStamp, foundResources) " +
                        $"VALUES ('{stepCount}', '{outcome}', '{date}', '{resourcesFound}')";
            ExecuteNonQuery(query);
        }


        public void Delete(int id)
        {
            var query = $"DELETE FROM explorationlog WHERE id = '{id}'";
            ExecuteNonQuery(query);
        }

        public void DeleteAll()
        {
            var query = $"DELETE FROM explorationlog";
            ExecuteNonQuery(query);
        }

        public void Update(int id, int stepCount, string outcome, string date, int resourcesFound)
        {
            var query = $"UPDATE explorationlog " +
                $"SET stepCount = {stepCount}" +
                $"outcome = {outcome}" +
                $"timeStamp = {date}" +
                $"resourcesFound = {resourcesFound}" +
                $"WHERE id = {id}";
        }
    }
}
