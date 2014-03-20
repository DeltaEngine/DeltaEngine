namespace DeltaEngine.Core
{
	/// <summary>
	/// Blending mode to be used for drawing. Usually in Opaque mode or Normal transparency.
	/// </summary>
	public enum BlendMode
	{
		/// <summary>
		/// Opaque is the fastest mode as it does not use any blending. Used for non alpha images.
		/// </summary>
		Opaque,
		/// <summary>
		/// Default blending mode is source alpha with inverted source alpha for the destination.
		/// Usually used for images with alpha, otherwise Opaque is choosen.
		/// </summary>
		Normal,
		/// <summary>
		/// Same as opaque, but use AlphaTest to discard any pixel below 0.66f in the alpha channel.
		/// Rendering this is faster than Normal because we need no sorting and we can skip pixels.
		/// </summary>
		AlphaTest,
		/// <summary>
		/// Used to accumulate the color we already got, all pixels are added to the destination.
		/// Has the advantage that we do not need to sort, can be rendered in any order after Opaque.
		/// </summary>
		Additive,
		/// <summary>
		/// Opposite of additive blending. Instead of always adding more brightness to the target screen
		/// pixels, this one subtracts brightness. Often used for fake shadowing effects (blob shadows).
		/// </summary>
		Subtractive,
		/// <summary>
		/// Special blend mode of DestColor+One used for light effects (similar to lightmap shaders, but
		/// just using a lightmap, no diffuse texture). Used for glow, light and flare effects.
		/// </summary>
		LightEffect
	}
}