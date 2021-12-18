using System;
using System.Collections.Generic;
using System.Text;

namespace Axis.Libra.Query
{
    /// <summary>
    /// Provides the service of discovering and registering querys and dispatchers with a supplied <see cref="Proteus.IoC.ServiceRegistrar"/>.
    /// 
    /// Note that the query registrar ensures that the correct <see cref="Utils.BindResultAttribute"/> bindings are respected during registration
    /// </summary>
    class QueryRegistrar
    {
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of querys. It encapsulates a list of all <see cref="IQuery"/>s and corresponding <see cref="IQueryResult"/>s in the system.
    /// </summary>
    class QueryManifest
    {

    }
}
