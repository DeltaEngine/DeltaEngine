using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Drawable components like Entity2D or Entity3D will interpolate between update ticks.
	/// NextUpdateStarted marks the beginning of an update tick to copy interpolatable data, it can
	/// also be used to check if any data has changed since last time and if something needs updating.
	/// </summary>
	public class DrawableEntity : Entity
	{
		/// <summary>
		/// By default all drawable entities are visible, but you can easily turn off drawing here.
		/// </summary>
		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				if (isVisible != value)
					ToggleVisibility();
			}
		}

		private bool isVisible = true;

		public virtual void ToggleVisibility()
		{
			isVisible = !isVisible;
			if (isVisible)
				EntitiesRunner.Current.AddVisible(this);
			else
				EntitiesRunner.Current.RemoveVisible(this);
		}

		/// <summary>
		/// Each Entity2D uses a RenderLayer, which will determine the sorting for rendering.
		/// </summary>
		public int RenderLayer
		{
			get { return base.Contains<int>() ? base.Get<int>() : DefaultRenderLayer; }
			set
			{
				if (value == RenderLayer)
					return;
				Set(value);
				if (IsVisible)
					EntitiesRunner.Current.ChangeRenderLayer(this, drawBehaviors);
			}
		}

		public const int DefaultRenderLayer = 0;

		internal void InvokeNextUpdateStarted()
		{
			NextUpdateStarted();
		}

		protected virtual void NextUpdateStarted()
		{
			for (int index = 0; index < lastTickLerpComponents.Count; index++)
				foreach (var current in components)
					if (current.GetType() == lastTickLerpComponents[index].GetType())
					{
						if (current is IList)
						{
							var genericListTypes = current.GetType().GetGenericArguments();
							if (genericListTypes.Length > 0)
							{
								var elementType = genericListTypes[0];
								var currentList = current as IList;
								var lastList = lastTickLerpComponents[index] as IList;
								if (lastList == null || Equals(lastList, currentList) ||
									lastList.Count != currentList.Count)
									lastTickLerpComponents[index] =
										Activator.CreateInstance((typeof(List<>).MakeGenericType(elementType)), current);
								else
								{
									lastList.Clear();
									foreach (var item in currentList)
										lastList.Add(item);
								}
							}
							else if (current is Array)
							{
								var currentArray = current as Array;
								var elementType = current.GetType().GetElementType();
								var copiedArray = lastTickLerpComponents[index] as Array;
								if (copiedArray == null || copiedArray == currentArray ||
									copiedArray.Length != currentArray.Length)
									copiedArray = Array.CreateInstance(elementType, currentArray.Length);
								if (currentArray.Length > 0)
									Array.Copy(currentArray, copiedArray, currentArray.Length);
								lastTickLerpComponents[index] = copiedArray;
							}
						}
						else
							lastTickLerpComponents[index] = current;
						break;
					}
		}

		/// <summary>
		/// Each element can either be a Lerp, a Lerp List or an array of Lerp objects.
		/// </summary>
		protected readonly List<object> lastTickLerpComponents = new List<object>();

		public override T Get<T>()
		{
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw &&
				typeof(Lerp).IsAssignableFrom(typeof(T)))
				if (typeof(Lerp<T>).IsAssignableFrom(typeof(T)))
					foreach (var previous in lastTickLerpComponents)
						if (previous is T)
							return ((Lerp<T>)previous).Lerp(base.Get<T>(), EntitiesRunner.CurrentDrawInterpolation);
			return base.Get<T>();
		}

		public List<T> GetInterpolatedList<T>() where T : Lerp
		{
			EntitiesRunner.Current.CheckIfInDrawState();
			foreach (var previous in lastTickLerpComponents)
			{
				var previousList = previous as IList<T>;
				if (previousList != null && previousList.GetType().IsGenericType &&
					previousList.GetType().GetGenericArguments()[0] == typeof(T))
					foreach (var component in components)
					{
						var currentList = component as IList<T>;
						if (currentList == null)
							continue;
						var length = Math.Min(previousList.Count, currentList.Count);
						var returnValue = new List<T>(previousList);
						for (int index = 0; index < length; index++)
							returnValue[index] = ((Lerp<T>)returnValue[index]).Lerp(currentList[index],
								EntitiesRunner.CurrentDrawInterpolation);
						return returnValue;
					}
			}
			throw new ListWithLerpElementsForInterpolationWasNotFound(typeof(T));
		}

		public class ListWithLerpElementsForInterpolationWasNotFound : Exception
		{
			public ListWithLerpElementsForInterpolationWasNotFound(Type type)
				: base(type.ToString()) {}
		}

		public T[] GetInterpolatedArray<T>(int arrayCopyLimit = -1) where T : Lerp
		{
			EntitiesRunner.Current.CheckIfInDrawState();
			foreach (var previous in lastTickLerpComponents)
			{
				var previousArray = previous as T[];
				if (previousArray != null && previousArray.GetType().GetElementType() == typeof(T))
					foreach (var component in components)
					{
						var currentArray = component as T[];
						if (currentArray == null)
							continue; //ncrunch: no coverage
						var length = Math.Min(previousArray.Length, currentArray.Length);
						if (arrayCopyLimit > 0 && length > arrayCopyLimit)
							length = arrayCopyLimit;
						var returnValue = new T[length];
						for (int index = 0; index < length; index++)
							returnValue[index] = ((Lerp<T>)previousArray[index]).Lerp(currentArray[index],
								EntitiesRunner.CurrentDrawInterpolation);
						return returnValue;
					}
			} //ncrunch: no coverage
			throw new ArrayWithLerpElementsForInterpolationWasNotFound(typeof(T));
		}

		public class ArrayWithLerpElementsForInterpolationWasNotFound : Exception
		{
			public ArrayWithLerpElementsForInterpolationWasNotFound(Type type)
				: base(type.ToString()) { }
		}

		public override Entity Add<T>(T component)
		{
			base.Add(component);
			if (IsLerpableType(component))
				lastTickLerpComponents.Add(component);
			return this;
		}

		private static bool IsLerpableType<T>(T component)
		{
			if (component is Lerp || component is float)
				return true;
			var list = component as IList;
			if (list != null)
			{
				if (list.Count > 0 && IsLerpableType(list[0]))
					return true;
				var arguments = list.GetType().GetGenericArguments();
				if (arguments.Length > 0 && typeof(Lerp).IsAssignableFrom(arguments[0]))
					return true; // ncrunch: no coverage
			}
			return false;
		}

		public virtual void SetWithoutInterpolation<T>(T component)
		{
			Set(component);
			if (!IsLerpableType(component))
				return;
			for (int index = 0; index < lastTickLerpComponents.Count; index++)
				if (lastTickLerpComponents[index] is T)
				{
					lastTickLerpComponents[index] = component;
					return;
				}
		}

		public override void Set(object component)
		{
			if (component is bool)
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				isVisible = (bool)component;
				return;
			}
			base.Set(component);
			if (!IsLerpableType(component))
				return;
			if (lastTickLerpComponents.All(c => c.GetType() != component.GetType()))
				lastTickLerpComponents.Add(component);
		}

		public void OnDraw<T>() where T : class, DrawBehavior
		{
			OnDraw(typeof(T));
		}

		internal void OnDraw(Type drawBehaviorType)
		{
			var behavior = EntitiesRunner.Current.GetDrawBehavior(drawBehaviorType) as DrawBehavior;
			if (drawBehaviors.Contains(behavior))
				return; // ncrunch: no coverage
			drawBehaviors.Add(behavior);
			EntitiesRunner.Current.AddToDrawBehaviorList(this, behavior);
		}

		internal readonly List<DrawBehavior> drawBehaviors = new List<DrawBehavior>();

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				if (base.IsActive == value)
					return;
				base.IsActive = value;
				if (value)
					foreach (DrawBehavior behavior in drawBehaviors)
						EntitiesRunner.Current.AddToDrawBehaviorList(this, behavior);
			}
		}

		protected internal List<DrawBehavior> GetDrawBehaviors()
		{
			return drawBehaviors;
		}
	}
}