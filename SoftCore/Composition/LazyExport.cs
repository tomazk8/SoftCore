using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="T">Generic type of the return value</typeparam>
    internal class LazyExport<T> : Lazy<T>
    {
        public LazyExport(Export export)
            : base(() => (T)export.ToInstance())
        {
        }
    }
}
