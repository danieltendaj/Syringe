using System.Diagnostics;

namespace Syringe.Core.Environment
{
    [DebuggerDisplay("{Name}")]
    public class Environment
    {
        /// <summary>
        /// The name of the environment, this must be unique.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The order of the environment in the list.
        /// </summary>
        public int Order { get; set; }
    }
}
