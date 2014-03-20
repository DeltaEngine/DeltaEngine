using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;

namespace $safeprojectname$
{
	public class Program : App
	{
		private Program()
		{
			new Game(new Track(simpleTrackPoints), Resolve<Window>());
		}

		private readonly Vector2D[] simpleTrackPoints =
		{
			new Vector2D(0.5138889f, 0.7425926f), new Vector2D(0.5453703f, 0.7425926f),
			new Vector2D(0.5759259f, 0.7407407f), new Vector2D(0.6074074f, 0.7314814f),
			new Vector2D(0.6324074f, 0.712037f), new Vector2D(0.6444444f, 0.6824074f),
			new Vector2D(0.6537037f, 0.6527777f), new Vector2D(0.6694444f, 0.6259259f),
			new Vector2D(0.6925926f, 0.6055555f), new Vector2D(0.7166666f, 0.587037f),
			new Vector2D(0.7416667f, 0.5675926f), new Vector2D(0.7666667f, 0.5462963f),
			new Vector2D(0.7888889f, 0.5240741f), new Vector2D(0.8037037f, 0.4962963f),
			new Vector2D(0.8120371f, 0.4666666f), new Vector2D(0.8175926f, 0.4351852f),
			new Vector2D(0.8185185f, 0.4046296f), new Vector2D(0.8157407f, 0.374074f),
			new Vector2D(0.8027778f, 0.3444445f), new Vector2D(0.7814814f, 0.3222222f),
			new Vector2D(0.7537037f, 0.3092592f), new Vector2D(0.7231481f, 0.3074074f),
			new Vector2D(0.6916667f, 0.3083333f), new Vector2D(0.6601852f, 0.3157407f),
			new Vector2D(0.6305556f, 0.3287037f), new Vector2D(0.6037037f, 0.3444445f),
			new Vector2D(0.5768518f, 0.3592592f), new Vector2D(0.5490741f, 0.3712963f),
			new Vector2D(0.5185185f, 0.3731481f), new Vector2D(0.4888889f, 0.3675926f),
			new Vector2D(0.4592592f, 0.3555555f), new Vector2D(0.4296296f, 0.3435185f),
			new Vector2D(0.4f, 0.3314815f), new Vector2D(0.3703704f, 0.3203704f),
			new Vector2D(0.337963f, 0.3083333f), new Vector2D(0.3074074f, 0.3018519f),
			new Vector2D(0.2768518f, 0.3009259f), new Vector2D(0.2472222f, 0.3092592f),
			new Vector2D(0.2212963f, 0.3259259f), new Vector2D(0.1981481f, 0.3481481f),
			new Vector2D(0.1814815f, 0.3731481f), new Vector2D(0.1648148f, 0.3990741f),
			new Vector2D(0.1481481f, 0.4240741f), new Vector2D(0.1324074f, 0.4518518f),
			new Vector2D(0.1222222f, 0.4805555f), new Vector2D(0.1175926f, 0.512037f),
			new Vector2D(0.1175926f, 0.5444444f), new Vector2D(0.1175926f, 0.575f),
			new Vector2D(0.1268519f, 0.6046296f), new Vector2D(0.1425926f, 0.6305555f),
			new Vector2D(0.162963f, 0.6555555f), new Vector2D(0.1898148f, 0.6731481f),
			new Vector2D(0.2194444f, 0.6833333f), new Vector2D(0.25f, 0.6898148f),
			new Vector2D(0.2805555f, 0.6972222f), new Vector2D(0.3101852f, 0.7055556f),
			new Vector2D(0.3407407f, 0.7138889f), new Vector2D(0.3722222f, 0.7175925f),
			new Vector2D(0.4037037f, 0.7185185f), new Vector2D(0.4342593f, 0.7231481f),
			new Vector2D(0.4648148f, 0.7277778f), new Vector2D(0.489351869f, 0.735185146f)
		};

		static void Main()
		{
			new Program().Run();
		}
	}
}