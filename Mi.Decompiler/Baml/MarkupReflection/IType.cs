﻿// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Decompiler.Baml.MarkupReflection
{
	/// <summary>
	/// Interface rappresenting a DotNet type
	/// </summary>
	public interface IType
	{
		string AssemblyQualifiedName { get; }
		bool IsSubclassOf(IType type);
		bool Equals(IType type);
	}
}