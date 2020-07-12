using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Aga.Controls.Tree
{
	internal class InputWithControl: NormalInputState
	{
		private bool _mouseDownFlag = false;

		public InputWithControl(TreeViewAdv tree): base(tree)
		{
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if (args.Node != null)
			{
				if (args.Button == MouseButtons.Left || args.Button == MouseButtons.Right)
				{
					if (args.Node.IsSelected)
					{
						_mouseDownFlag = true;
						return;
					}
					else
						_mouseDownFlag = false;
				}
			}

			base.MouseDown(args);
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			Tree.ItemDragMode = false;
			if (_mouseDownFlag && args.Node != null)
			{
				if (_mouseDownFlag && args.Button == MouseButtons.Left)
					DoMouseOperation(args, false);

				_mouseDownFlag = false;
			}
			else
			{
				base.MouseUp(args);
			}
		}


		protected override void DoMouseOperation(TreeNodeAdvMouseEventArgs args, bool focusOnly)
		{
			if (Tree.SelectionMode == TreeSelectionMode.Single)
			{
				base.DoMouseOperation(args, focusOnly);
			}
			else if (CanSelect(args.Node))
			{
				args.Node.IsSelected = !args.Node.IsSelected;
				Tree.SelectionStart = args.Node;
			}
		}

		protected override void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
		}
	}
}
