using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition.Catalogs
{
    // TODO: Sometimes we want to register a class which imports a whole tree of other classes.
    // Sometimes we don't want to care explicitly about those classes and simply want to
    // register them without actually knowing them.
    //
    // The idea with this catalog is that it takes an exported class, scans for improts and
    // registers all classes in the whole tree that this and any imported classes import.
    //
    // This can be a problem though. Some unwanted classes could get registered. The question
    // is also how will the dependant imports be found and where. Library cannot know
    // where assemblies with the classes are located.

    /*public class DependenciesCatalog : Catalog
    {
        
    }*/
}
