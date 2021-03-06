// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace MethodFromInterfaceAbstract
{
	public interface IMyInterface
	{
		void MyMethod();
	}
	public abstract class MyClass : IMyInterface
	{
		public abstract void MyMethod();
	}
}
