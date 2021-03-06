// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Mi.CSharp.Ast.Expressions
{
    using Mi.NRefactory.PatternMatching;
    
    /// <summary>
	/// Base class for expressions.
	/// </summary>
	/// <remarks>
	/// This class is useful even though it doesn't provide any additional functionality:
	/// It can be used to communicate more information in APIs, e.g. "this subnode will always be an expression"
	/// </remarks>
	public abstract class Expression : AstNode
	{
		#region Null
		public new static readonly Expression Null = new NullExpression ();
		
		sealed class NullExpression : Expression
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
			
			protected internal override bool DoMatch(AstNode other, Match match)
			{
				return other == null || other.IsNull;
			}
		}
		#endregion
		
		#region PatternPlaceholder
		public static implicit operator Expression(Pattern pattern)
		{
			return pattern != null ? new PatternPlaceholder(pattern) : null;
		}
		
		sealed class PatternPlaceholder : Expression, INode
		{
			readonly Pattern child;
			
			public PatternPlaceholder(Pattern child)
			{
				this.child = child;
			}
			
			public override NodeType NodeType {
				get { return NodeType.Pattern; }
			}
			
			public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
			{
				return visitor.VisitPatternPlaceholder(this, child, data);
			}
			
			protected internal override bool DoMatch(AstNode other, Match match)
			{
				return child.DoMatch(other, match);
			}
			
			bool INode.DoMatchCollection(Role role, INode pos, Match match, BacktrackingInfo backtrackingInfo)
			{
				return child.DoMatchCollection(role, pos, match, backtrackingInfo);
			}
		}
		#endregion
		
		public override NodeType NodeType {
			get {
				return NodeType.Expression;
			}
		}
		
		public new Expression Clone()
		{
			return (Expression)base.Clone();
		}
		
		public Expression ReplaceWith(Func<Expression, Expression> replaceFunction)
		{
			if (replaceFunction == null)
				throw new ArgumentNullException("replaceFunction");
			return (Expression)base.ReplaceWith(node => replaceFunction((Expression)node));
		}
		
		#region Builder methods
		/// <summary>
		/// Builds an member reference expression using this expression as target.
		/// </summary>
		public MemberReferenceExpression Member(string memberName)
		{
			return new MemberReferenceExpression { Target = this, MemberName = memberName };
		}
		
		/// <summary>
		/// Builds an indexer expression using this expression as target.
		/// </summary>
		public IndexerExpression Indexer(IEnumerable<Expression> arguments)
		{
			IndexerExpression expr = new IndexerExpression();
			expr.Target = this;
			expr.Arguments.AddRange(arguments);
			return expr;
		}
		
		/// <summary>
		/// Builds an indexer expression using this expression as target.
		/// </summary>
		public IndexerExpression Indexer(params Expression[] arguments)
		{
			IndexerExpression expr = new IndexerExpression();
			expr.Target = this;
			expr.Arguments.AddRange(arguments);
			return expr;
		}
		
		/// <summary>
		/// Builds an invocation expression using this expression as target.
		/// </summary>
		public InvocationExpression Invoke(string methodName, IEnumerable<Expression> arguments)
		{
			return Invoke(methodName, null, arguments);
		}
		
		/// <summary>
		/// Builds an invocation expression using this expression as target.
		/// </summary>
		public InvocationExpression Invoke(string methodName, params Expression[] arguments)
		{
			return Invoke(methodName, null, arguments);
		}
		
		/// <summary>
		/// Builds an invocation expression using this expression as target.
		/// </summary>
		public InvocationExpression Invoke(string methodName, IEnumerable<AstType> typeArguments, IEnumerable<Expression> arguments)
		{
			InvocationExpression ie = new InvocationExpression();
			MemberReferenceExpression mre = new MemberReferenceExpression();
			mre.Target = this;
			mre.MemberName = methodName;
			mre.TypeArguments.AddRange(typeArguments);
			ie.Target = mre;
			ie.Arguments.AddRange(arguments);
			return ie;
		}
		
		/// <summary>
		/// Builds an invocation expression using this expression as target.
		/// </summary>
		public InvocationExpression Invoke(IEnumerable<Expression> arguments)
		{
			InvocationExpression ie = new InvocationExpression();
			ie.Target = this;
			ie.Arguments.AddRange(arguments);
			return ie;
		}
		
		/// <summary>
		/// Builds an invocation expression using this expression as target.
		/// </summary>
		public InvocationExpression Invoke(params Expression[] arguments)
		{
			InvocationExpression ie = new InvocationExpression();
			ie.Target = this;
			ie.Arguments.AddRange(arguments);
			return ie;
		}
		
		public CastExpression CastTo(AstType type)
		{
			return new CastExpression { Type = type,  Expression = this };
		}
		
		public CastExpression CastTo(Type type)
		{
			return new CastExpression { Type = AstType.Create(type),  Expression = this };
		}
		
		public AsExpression CastAs(AstType type)
		{
			return new AsExpression { Type = type,  Expression = this };
		}
		
		public AsExpression CastAs(Type type)
		{
			return new AsExpression { Type = AstType.Create(type),  Expression = this };
		}
		
		public IsExpression IsType(AstType type)
		{
			return new IsExpression { Type = type,  Expression = this };
		}
		
		public IsExpression IsType(Type type)
		{
			return new IsExpression { Type = AstType.Create(type),  Expression = this };
		}
		#endregion
	}
}
