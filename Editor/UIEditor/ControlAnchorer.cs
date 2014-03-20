using System.Collections.Generic;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlAnchorer
	{
		public static void AnchorSelectedControls(Entity2D anchoringControl,
			List<Entity2D> selectedControls)
		{
			foreach (Entity2D attachingControl in selectedControls)
			{
				if (attachingControl == anchoringControl)
					continue;
				AnchorVertical((Control)anchoringControl, (Control)attachingControl);
				AnchorHorizontal((Control)anchoringControl, (Control)attachingControl);
			}
		}

		private static void AnchorVertical(Control anchoringControl, Control attachingControl)
		{
			if (anchoringControl.DrawArea.Left < attachingControl.DrawArea.Left)
				CheckIfAnchoringLeft(anchoringControl, attachingControl);
			else if (anchoringControl.DrawArea.Left > attachingControl.DrawArea.Left)
				CHeckIfAnchoringRight(anchoringControl, attachingControl);
			else
				attachingControl.LeftMargin = new Margin(anchoringControl, Edge.Left, 0);
		}

		private static void CheckIfAnchoringLeft(Control anchoringControl, Control attachingControl)
		{
			if (anchoringControl.DrawArea.Left > attachingControl.DrawArea.Right)
				return; //ncrunch: no coverage 
			float verticalDistance = attachingControl.DrawArea.Left - anchoringControl.DrawArea.Left;
			attachingControl.LeftMargin = new Margin(anchoringControl, Edge.Left, verticalDistance);
		}

		private static void CHeckIfAnchoringRight(Control anchoringControl, Control attachingControl)
		{
			if (anchoringControl.DrawArea.Left > attachingControl.DrawArea.Right)
			{
				float verticalDistance = anchoringControl.DrawArea.Left - attachingControl.DrawArea.Right;
				attachingControl.RightMargin = new Margin(anchoringControl, Edge.Left, verticalDistance);
			}
			else
			{
				float verticalDistance = anchoringControl.DrawArea.Left - attachingControl.DrawArea.Left;
				attachingControl.RightMargin = new Margin(anchoringControl, Edge.Right, verticalDistance);
			}
		}

		private static void AnchorHorizontal(Control anchoringControl, Control attachingControl)
		{
			if (anchoringControl.DrawArea.Top < attachingControl.DrawArea.Top)
				CheckIfAnchoringTop(anchoringControl, attachingControl);
			else if (anchoringControl.DrawArea.Top > attachingControl.DrawArea.Top)
				CheckIfAnchoringBottom(anchoringControl, attachingControl);
			else
				attachingControl.TopMargin = new Margin(anchoringControl, Edge.Top, 0);
		}

		private static void CheckIfAnchoringTop(Control anchoringControl, Control attachingControl)
		{
			if (anchoringControl.DrawArea.Top > attachingControl.DrawArea.Bottom)
				return; //ncrunch: no coverage 
			float horizontalDistance = attachingControl.DrawArea.Top - anchoringControl.DrawArea.Top;
			attachingControl.TopMargin = new Margin(anchoringControl, Edge.Top, horizontalDistance);
		}

		private static void CheckIfAnchoringBottom(Control anchoringControl, Control attachingControl)
		{
			if (anchoringControl.DrawArea.Top > attachingControl.DrawArea.Bottom)
			{
				float horizontalDistance = anchoringControl.DrawArea.Top - attachingControl.DrawArea.Bottom;
				attachingControl.BottomMargin = new Margin(anchoringControl, Edge.Top, horizontalDistance);
			}
			else
			{
				float horizontalDistance = anchoringControl.DrawArea.Top - attachingControl.DrawArea.Top;
				attachingControl.BottomMargin = new Margin(anchoringControl, Edge.Bottom,
					horizontalDistance);
			}
		}

		public static void UnAnchorSelectedControls(List<Entity2D> controlList)
		{
			foreach (var control in controlList)
				control.Set(new AnchoringState());
		}
	}
}