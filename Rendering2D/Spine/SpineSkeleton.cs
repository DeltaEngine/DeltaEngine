using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using Spine;

namespace DeltaEngine.Rendering2D.Spine
{
	/// <summary>
	/// Allows the creation of animated moving 2D sprites with skeletons and bone animations. To use 
	/// this you will need a Spine license and Spine Creation tool from: http://esotericsoftware.com/
	/// For details of how to use Spine within the Delta Engine see: http://deltaengine.net/spine
	/// </summary>
	public class SpineSkeleton : Entity2D, RapidUpdateable
	{
		public SpineSkeleton(string atlasName, string skeletonName, Vector2D origin)
			: this(atlasName, skeletonName, new Rectangle(origin, Size.One)) {}

		public SpineSkeleton(string atlasName, string skeletonName, Rectangle originAndScale)
		{
			skeleton = CreateSkeleton(atlasName, skeletonName);
			skeleton.SetBonesToSetupPose();
			stateData = new AnimationStateData(skeleton.Data);
			state = new AnimationState(stateData);
			LastOriginAndScale = OriginAndScale = originAndScale;
			OnDraw<SpineRenderer>();
		}

		internal readonly Skeleton skeleton;
		internal readonly AnimationStateData stateData;
		internal readonly AnimationState state;

		private Skeleton CreateSkeleton(string atlasName, string skeletonName)
		{
			var atlasData = ContentLoader.Load<AtlasData>(atlasName);
			atlas = new Atlas(atlasData.TextReader, "Content", MaterialLoader);
			var skeletonJson = new SkeletonJson(atlas);
			var spineSkeletonData = ContentLoader.Load<SpineSkeletonData>(skeletonName);
			SkeletonData = skeletonJson.ReadSkeletonData(spineSkeletonData.TextReader);
			return new Skeleton(SkeletonData);
		}

		private Atlas atlas;
		internal SkeletonData SkeletonData { get; private set; }
		private static readonly MaterialLoader MaterialLoader = new MaterialLoader();

		public Vector2D Origin
		{
			get { return TopLeft; }
			set { TopLeft = value; }
		}

		public Size Scale
		{
			get { return Size; }
			set { DrawArea = new Rectangle(Origin, value); }
		}

		public Rectangle OriginAndScale
		{
			get { return DrawArea; }
			set { DrawArea = value; }
		}

		public Rectangle LastOriginAndScale
		{
			get { return LastDrawArea; }
			set { LastDrawArea = value; }
		}

		public void DefineAnimationMix(string fromName, string toName, float duration)
		{
			stateData.SetMix(fromName, toName, duration);
		}

		public void SetAnimation(string animationName, int trackIndex = 0)
		{
			SetAnimation(animationName, () => { }, trackIndex);
		}

		public void SetAnimation(string animationName, Action animationEnded, int trackIndex = 0)
		{
			TrackEntry entry = state.SetAnimation(trackIndex, animationName, false);
			if (animationEnded != null)
				entry.End += (sender, eventArgs) => animationEnded();
		}

		public void SetAnimationLooped(string animationName, int trackIndex = 0)
		{
			state.SetAnimation(trackIndex, animationName, true);
		}

		public void AddAnimation(string animationName, float delay = 0, int trackIndex = 0)
		{
			AddAnimation(animationName, () => { }, delay, trackIndex);
		}

		public void AddAnimation(string animationName, Action animationEnded, float delay = 0,
			int trackIndex = 0)
		{
			TrackEntry entry = state.AddAnimation(trackIndex, animationName, false, delay);
			if (animationEnded != null)
				entry.End += (sender, eventArgs) => animationEnded();
		}

		public void AddAnimationLooped(string animationName, float delay = 0, int trackIndex = 0)
		{
			state.AddAnimation(trackIndex, animationName, true, delay);
		}

		public FlipMode FlipMode
		{
			get
			{
				return (skeleton.FlipX ? FlipMode.Horizontal : FlipMode.None) |
							(skeleton.FlipY ? FlipMode.Vertical : FlipMode.None);
			}
			set
			{
				skeleton.FlipX = value.HasFlag(FlipMode.Horizontal);
				skeleton.FlipY = value.HasFlag(FlipMode.Vertical);
			}
		}

		public void RapidUpdate()
		{
			state.Update(Time.Delta);
			state.Apply(skeleton);
			skeleton.Update(Time.Delta);
			skeleton.UpdateWorldTransform();
		}

		public override void Dispose()
		{
			atlas.Dispose();
			base.Dispose();
		}
	}
}