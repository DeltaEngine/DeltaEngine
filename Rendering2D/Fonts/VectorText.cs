using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Draws vector style text in 2D space
	/// </summary>
	public class VectorText : Entity2D
	{
		public VectorText(string text, Vector2D position)
			: base(Rectangle.FromCenter(position, new Size(0.05f)))
		{
			Add(new Data(text));
			Start<ProcessText>();
			OnDraw<Render>();
		}

		internal class Data
		{
			public Data(string text)
			{
				Text = text;
			}

			public string Text
			{
				get { return text; }
				set
				{
					text = value;
					IsRefreshNeeded = true;
				}
			}

			private string text;
			public bool IsRefreshNeeded { get; set; }
			public List<Vector2D> LinePoints { get; set; }
		}

		public class ProcessText : UpdateBehavior, Filtered
		{
			public ProcessText()
				: base(Priority.Last) {}

			public Func<Entity, bool> Filter
			{
				get { return entity => entity.Get<Data>().IsRefreshNeeded; }
			}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Data data in entities.Select(entity => entity.Get<Data>()))
				{
					data.LinePoints = CalculateLinePoints(data.Text);
					data.IsRefreshNeeded = false;
				}
			}

			private List<Vector2D> CalculateLinePoints(string text)
			{
				var points = new List<Vector2D>();
				caretPosition = new Vector2D(-CharacterWidth * text.Length / 2, -CharacterWidth / 2);
				foreach (char c in text)
					CalculateCharLines(points, c);
				return points;
			}

			private Vector2D caretPosition = Vector2D.Zero;
			private const float CharacterWidth = 0.64f;

			private void CalculateCharLines(List<Vector2D> points, char c)
			{
				if (!char.IsWhiteSpace(c))
					points.AddRange(characterLines.GetPoints(c).Select(point => caretPosition + point));
				caretPosition.X += CharacterWidth;
			}

			private readonly VectorCharacterLines characterLines = new VectorCharacterLines();
		}

		public class Render : DrawBehavior
		{
			public Render(Drawing drawing)
			{
				this.drawing = drawing;
				material = new Material(ShaderFlags.Position2DColored, "");
			}

			private readonly Drawing drawing;
			private readonly Material material;
			private readonly List<VertexPosition2DColor> vertices = new List<VertexPosition2DColor>();

			public void Draw(List<DrawableEntity> visibleEntities)
			{
				foreach (Entity2D entity in visibleEntities)
					AddVertices(entity);
				if (vertices.Count > 0)
					drawing.AddLines(material, vertices.ToArray());
				vertices.Clear();
			}

			private void AddVertices(Entity2D entity)
			{
				var area = entity.DrawArea;
				foreach (Vector2D pos in entity.Get<Data>().LinePoints)
					vertices.Add(new VertexPosition2DColor(
						ScreenSpace.Current.ToPixelSpaceRounded(pos * area.Height + area.Center), entity.Color));
			}
		}

		public string Text
		{
			get { return Get<Data>().Text; }
			set { Get<Data>().Text = value; }
		}
	}
}