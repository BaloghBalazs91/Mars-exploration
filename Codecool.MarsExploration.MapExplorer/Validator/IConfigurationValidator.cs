using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codecool.MarsExploration.MapExplorer.Configuration;

namespace Codecool.MarsExploration.MapExplorer.Validator
{
    internal interface IConfigurationValidator
    {
        bool IsConfigurationValid(ExplorationConfiguration configuration);
    }
}
