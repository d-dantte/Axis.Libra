using System;
using System.Collections.Generic;
using System.Text;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Provides the service of discovering and registering commands and dispatchers and command result queries with a supplied <see cref="Proteus.IoC.ServiceRegistrar"/>.
    /// 
    /// Note that the command registrar ensures that the correct <see cref="Utils.BindResultAttribute"/> bindings are respected during registration
    /// </summary>
    class CommandRegistrar
    {
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of commands. It encapsulates a list of all <see cref="ICommand"/>s and corresponding <see cref="ICommandResult"/>s in the system.
    /// </summary>
    class CommandManifest
    {

    }
}
