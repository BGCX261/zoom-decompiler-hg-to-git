﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;

namespace ICSharpCode.TreeView
{
	public class SharpTreeViewItem : ListViewItem
	{
		static SharpTreeViewItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SharpTreeViewItem),
			                                         new FrameworkPropertyMetadata(typeof(SharpTreeViewItem)));
		}

		public SharpTreeNode Node
		{
			get { return DataContext as SharpTreeNode; }
		}

		public SharpTreeNodeView NodeView { get; internal set; }
		public SharpTreeView ParentTreeView { get; internal set; }

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.Key) {
				case Key.F2:
//					if (SharpTreeNode.ActiveNodes.Count == 1 && Node.IsEditable) {
//						Node.IsEditing = true;
//						e.Handled = true;
//					}
					break;
				case Key.Escape:
					Node.IsEditing = false;
					break;
			}
		}

		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			ContextMenu = Node.GetContextMenu();
		}

		protected override void OnContextMenuClosing(ContextMenuEventArgs e)
		{
			ClearValue(ContextMenuProperty);
		}

		#region Mouse

		Point startPoint;
		bool wasSelected;
		bool wasDoubleClick;

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			wasSelected = IsSelected;
			if (!IsSelected) {
				base.OnMouseLeftButtonDown(e);
			}

			if (Mouse.LeftButton == MouseButtonState.Pressed) {
				startPoint = e.GetPosition(null);
				CaptureMouse();

				if (e.ClickCount == 2) {
					wasDoubleClick = true;
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (IsMouseCaptured) {
				var currentPoint = e.GetPosition(null);
				if (Math.Abs(currentPoint.X - startPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
				    Math.Abs(currentPoint.Y - startPoint.Y) >= SystemParameters.MinimumVerticalDragDistance) {

//					if (Node.InternalCanDrag()) {
//						Node.InternalDrag(this);
//					}
				}
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (wasDoubleClick) {
				wasDoubleClick = false;
				Node.ActivateItem(e);
				if (!e.Handled) {
					if (!Node.IsRoot || ParentTreeView.ShowRootExpander) {
						Node.IsExpanded = !Node.IsExpanded;
					}
				}
			}
			
			ReleaseMouseCapture();
			if (wasSelected) {
				base.OnMouseLeftButtonDown(e);
			}
		}

		#endregion
	}
}
