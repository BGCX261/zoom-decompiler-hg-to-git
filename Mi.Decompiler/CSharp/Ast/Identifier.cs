﻿#region Copyright
// 
// Identifier.cs
//  
// Author:
//       Mike KrÃ¼ger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Mi.CSharp.Ast
{
	public class Identifier : AstNode
	{
		public static readonly Identifier Null = new NullIdentifier ();
		class NullIdentifier : Identifier
		{
			public override bool IsNull {
				get {
					return true;
				}
			}
			
			public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
			{
				return default (S);
			}
			
			protected internal override bool DoMatch(AstNode other, Mi.NRefactory.PatternMatching.Match match)
			{
				return other == null || other.IsNull;
			}
		}
		
		public override NodeType NodeType {
			get {
				return NodeType.Unknown;
			}
		}
		
		string name;
		public string Name {
			get { return this.name; }
			set { 
				if (value == null)
					throw new ArgumentNullException("value");
				this.name = value;
			}
		}
		
		AstLocation startLocation;
		public override AstLocation StartLocation {
			get {
				return startLocation;
			}
			
		}
		
		public virtual bool IsVerbatim {
			get {
				return false;
			}
		}
		
		public override AstLocation EndLocation {
			get {
				return new AstLocation (StartLocation.Line, StartLocation.Column + (Name ?? "").Length);
			}
		}
		
		Identifier ()
		{
			this.name = string.Empty;
		}
		
		protected Identifier (string name, AstLocation location)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			this.Name = name;
			this.startLocation = location;
		}

		public static Identifier Create (string name)
		{
			return Create (name, AstLocation.Empty);
		}

		public static Identifier Create (string name, AstLocation location)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length > 0 && name[0] == '@')
				return new VerbatimIdentifier(name.Substring (1), location);
			return new Identifier (name, location);
		}
		
		public static Identifier Create (string name, AstLocation location, bool isVerbatim)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			if (isVerbatim)
				return new VerbatimIdentifier(name, location);
			return new Identifier (name, location);
		}
		
		public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitIdentifier (this, data);
		}
		
		protected internal override bool DoMatch(AstNode other, Mi.NRefactory.PatternMatching.Match match)
		{
			Identifier o = other as Identifier;
			return o != null && !o.IsNull && MatchString(this.Name, o.Name);
		}

		class VerbatimIdentifier : Identifier
		{
			public override AstLocation EndLocation {
				get {
					return new AstLocation (StartLocation.Line, StartLocation.Column + (Name ?? "").Length + 1); // @"..."
				}
			}
			
			public override bool IsVerbatim {
				get {
					return true;
				}
			}
			
			public VerbatimIdentifier(string name, AstLocation location) : base (name, location)
			{
			}
		}
	}
}
