using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    /// <summary>
    /// When an exported class (which may contain imports) is not imported by any of the other classes,
    /// its instance will never be created. There may be cases though where we want the library
    /// to create an instance of a class without anyone importing it. Without this attribute,
    /// this is not possible. A class marked as "Floating" will be created by the library
    /// itself so importing it is not needed. Instance is said to be "floating" because nobody uses it.
    /// 
    /// It is usefull to attach the functionality onto the existing structure without modifying the
    /// existing structure.
    /// 
    /// An example is a test class that handles events, to try out some new functionality. But without
    /// this attribute, we would have to import this test class somewhere in our normal code to create
    /// an instance of it, which means mixing testing code with normal code. Using this attribute,
    /// library will construct the instance and satisfy the imports, so class can process
    /// the events for testing.
    /// 
    /// This attribute will only work if it is applied on a class that also contains an Export attribute.
    /// 
    /// Floating instances are always singletons.
    /// 
    /// TODO: The other idea was to automatically create an instance of an export if it uses some import
    /// even if export is nowhere referenced. This would make a new problem because it is not an explicitly
    /// defined behavior and developer may not know that something happens behind the scener which could
    /// lead to bugs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FloatingExportAttribute : Attribute
    {
    }
}
