// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

//[module: CLSCompliantAttribute(false)]
namespace NamedInitializerFieldEnum
{
    [AttributeUsage(AttributeTargets.All)]
    public class MyAttributeAttribute : Attribute
    {
        public AttributeTargets Field;
    }
    [MyAttribute(Field = (AttributeTargets.Class | AttributeTargets.Method))]
    public class MyClass
    {
    }
}
