using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    public interface IPartSatisfiedCallback
    {
        void OnImportsSatisfied();
    }
}
