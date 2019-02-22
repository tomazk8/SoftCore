using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public abstract class LifetimeAttribute : Attribute
    {
    }
}
