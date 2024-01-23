using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Exploration
{
    public interface IOutOfMapChecker
    {
        bool IsWithinMap(Coordinate coordinate);
    }
}
