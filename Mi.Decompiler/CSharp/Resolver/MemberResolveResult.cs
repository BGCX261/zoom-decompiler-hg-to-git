﻿// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using Mi.NRefactory.TypeSystem;

namespace Mi.CSharp.Resolver
{
	/// <summary>
	/// Represents the result of a member invocation.
	/// </summary>
	public class MemberResolveResult : ResolveResult
	{
		readonly IMember member;
		readonly bool isConstant;
		readonly object constantValue;
		
		public MemberResolveResult(IMember member, IType returnType) : base(returnType)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			this.member = member;
		}
		
		public MemberResolveResult(IMember member, IType returnType, object constantValue) : base(returnType)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			this.member = member;
			this.isConstant = true;
			this.constantValue = constantValue;
		}
		
		public MemberResolveResult(IMember member, ITypeResolveContext context) : base(member.ReturnType.Resolve(context))
		{
			this.member = member;
		}
		
		public IMember Member {
			get { return member; }
		}
		
		public override bool IsCompileTimeConstant {
			get { return isConstant; }
		}
		
		public override object ConstantValue {
			get { return constantValue; }
		}
		
		public override string ToString()
		{
			return string.Format("[{0} {1}]", GetType().Name, member);
		}
	}
}
