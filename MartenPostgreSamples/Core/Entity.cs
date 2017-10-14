using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartenPostgreSamples.Core
{
    /// <summary>
    /// Represents a DDD Entity.
    /// </summary>
    public abstract class Entity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
    }
}
