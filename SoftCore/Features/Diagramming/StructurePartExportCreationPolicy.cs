using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features.Diagramming
{
    /// <summary>
    /// Specifies when and how a part will be instantiated.
    /// </summary>
    public enum StructurePartExportCreationPolicy
    {
        /// <summary>
        /// Specifies that a single shared instance of the associated ComposablePart will be created by the CompositionContainer and shared by all requestors.
        /// </summary>
        Shared = 0,
        /// <summary>
        /// Specifies that a new non-shared instance of the associated ComposablePart will be created by the CompositionContainer for every requestor.
        /// </summary>
        NonShared = 1
    }
}
