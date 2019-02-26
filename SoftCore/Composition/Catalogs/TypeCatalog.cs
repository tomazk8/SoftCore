using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Composition
{
    public class TypeCatalog : Catalog
    {
        public Type[] types;
        public ComposablePart[] parts;

        public TypeCatalog(params Type[] types)
        {
            this.types = types;
            this.parts = types
                .Select(x => new ComposablePart(x))
                .ToArray();
        }

        public IEnumerable<Type> Types
        {
            get { return types; }
        }

        public override IEnumerable<ComposablePart> Parts
        {
            get { return parts; }
        }
    }
}
