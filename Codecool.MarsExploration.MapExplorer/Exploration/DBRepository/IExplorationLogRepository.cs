using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Exploration.DBRepository
{
    internal interface IExplorationLogRepository
    {
        void Add(int stepCount, string outcome, string date, int resourcesFound);
        void Update(int id, int stepCount, string outcome, string date, int resourcesFound);
        void Delete(int id);
        void DeleteAll();
    }
}
