using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public interface IConsoleHandler
    {
        bool HandleCommand(string command,List<string> args);
    }
}
