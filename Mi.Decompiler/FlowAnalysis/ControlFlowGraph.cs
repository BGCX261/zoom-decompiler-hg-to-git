﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Mi.NRefactory.Utils;

namespace Mi.Decompiler.FlowAnalysis
{
	/// <summary>
	/// Contains the control flow graph.
	/// </summary>
	/// <remarks>Use ControlFlowGraph builder to create instances of the ControlFlowGraph.</remarks>
	public sealed class ControlFlowGraph
	{
		readonly ReadOnlyCollection<ControlFlowNode> nodes;
		
		public ControlFlowNode EntryPoint {
			get { return nodes[0]; }
		}
		
		public ControlFlowNode RegularExit {
			get { return nodes[1]; }
		}
		
		public ControlFlowNode ExceptionalExit {
			get { return nodes[2]; }
		}
		
		public ReadOnlyCollection<ControlFlowNode> Nodes {
			get { return nodes; }
		}
		
		internal ControlFlowGraph(ControlFlowNode[] nodes)
		{
			this.nodes = new ReadOnlyCollection<ControlFlowNode>(nodes);
			Debug.Assert(EntryPoint.NodeType == ControlFlowNodeType.EntryPoint);
			Debug.Assert(RegularExit.NodeType == ControlFlowNodeType.RegularExit);
			Debug.Assert(ExceptionalExit.NodeType == ControlFlowNodeType.ExceptionalExit);
		}
		
		/// <summary>
		/// Resets "Visited" to false for all nodes in this graph.
		/// </summary>
		public void ResetVisited()
		{
			foreach (ControlFlowNode node in nodes) {
				node.Visited = false;
			}
		}
		
		/// <summary>
		/// Computes the dominator tree.
		/// </summary>
		public void ComputeDominance(Action verifyProgress = default(Action))
		{
			// A Simple, Fast Dominance Algorithm
			// Keith D. Cooper, Timothy J. Harvey and Ken Kennedy
			
			EntryPoint.ImmediateDominator = EntryPoint;
			bool changed = true;
			while (changed) {
				changed = false;
				ResetVisited();
				
				verifyProgress();
				
				// for all nodes b except the entry point
				EntryPoint.TraversePreOrder(
					b => b.Successors,
					b => {
						if (b != EntryPoint) {
							ControlFlowNode newIdom = b.Predecessors.First(block => block.Visited && block != b);
							// for all other predecessors p of b
							foreach (ControlFlowNode p in b.Predecessors) {
								if (p != b && p.ImmediateDominator != null) {
									newIdom = FindCommonDominator(p, newIdom);
								}
							}
							if (b.ImmediateDominator != newIdom) {
								b.ImmediateDominator = newIdom;
								changed = true;
							}
						}
					});
			}
			EntryPoint.ImmediateDominator = null;
			foreach (ControlFlowNode node in nodes) {
				if (node.ImmediateDominator != null)
					node.ImmediateDominator.DominatorTreeChildren.Add(node);
			}
		}
		
		static ControlFlowNode FindCommonDominator(ControlFlowNode b1, ControlFlowNode b2)
		{
			// Here we could use the postorder numbers to get rid of the hashset, see "A Simple, Fast Dominance Algorithm"
			HashSet<ControlFlowNode> path1 = new HashSet<ControlFlowNode>();
			while (b1 != null && path1.Add(b1))
				b1 = b1.ImmediateDominator;
			while (b2 != null) {
				if (path1.Contains(b2))
					return b2;
				else
					b2 = b2.ImmediateDominator;
			}
			throw new Exception("No common dominator found!");
		}
		
		/// <summary>
		/// Computes dominance frontiers.
		/// This method requires that the dominator tree is already computed!
		/// </summary>
		public void ComputeDominanceFrontier()
		{
			ResetVisited();
			
			EntryPoint.TraversePostOrder(
				b => b.DominatorTreeChildren,
				n => {
					//logger.WriteLine("Calculating dominance frontier for " + n.Name);
					n.DominanceFrontier = new HashSet<ControlFlowNode>();
					// DF_local computation
					foreach (ControlFlowNode succ in n.Successors) {
						if (succ.ImmediateDominator != n) {
							//logger.WriteLine("  local: " + succ.Name);
							n.DominanceFrontier.Add(succ);
						}
					}
					// DF_up computation
					foreach (ControlFlowNode child in n.DominatorTreeChildren) {
						foreach (ControlFlowNode p in child.DominanceFrontier) {
							if (p.ImmediateDominator != n) {
								//logger.WriteLine("  DF_up: " + p.Name + " (child=" + child.Name);
								n.DominanceFrontier.Add(p);
							}
						}
					}
				});
		}
	}
}
