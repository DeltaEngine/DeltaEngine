namespace DeltaEngine.Content
{
	/// <summary>
	/// Content type for ContentData, for details see http://deltaengine.net/Features/ContentFormats
	/// </summary>
	public enum ContentType
	{
		/// <summary>
		/// .DeltaScene binary file, created by the SceneEditor, loaded when opening scenes.
		/// </summary>
		Scene = 0,
		/// <summary>
		/// .DeltaLevel is a collection of used meshes their world matrix and maybe some other content
		/// like camera paths, animations, etc. (different level settings for each game).
		/// </summary>
		Level = 3,
		/// <summary>
		/// Image content type, imported as .png bitmaps. Typically stored as compressed textures.
		/// </summary>
		Image = 4,
		/// <summary>
		/// Parent type for images in a sequence, e.g. "Explosion" uses "Explosion01", "Explosion02",
		/// etc. and some extra meta data for displaying this animation at certain speeds.
		/// </summary>
		ImageAnimation = 5,
		/// <summary>
		/// Uses Secondary Image Size to use one spritesheet for an animation.
		/// </summary>
		SpriteSheetAnimation = 6,
		/// <summary>
		/// .DeltaShader, which contains platform specific shader data.
		/// </summary>
		Shader = 7,
		/// <summary>
		/// Material, usually used for meshes and advanced rendering classes like Particle Effects.
		/// </summary>
		Material = 8,
		/// <summary>
		/// .DeltaMesh 3D content, either imported from an FBX file (or 3ds, obj, dxf, collada). The
		/// mesh itself is stored just as binary data, use the MeshData class to access it.
		/// </summary>
		Mesh = 9,
		/// <summary>
		/// Animation data for a mesh, e.g. an "Idle", a "Run" or an "Attack" animation of a character. 
		/// </summary>
		MeshAnimation = 10,
		/// <summary>
		/// 3D Models contain no own content, but just links up a bunch of meshes with their used
		/// materials and optionally their MeshAnimations into an easy to use form.
		/// </summary>
		Model = 11,
		/// <summary>
		/// .DeltaParticleEmitter represent the content of a particle effect system. Use the Particle
		/// Effect Editor to edit particle effects.
		/// </summary>
		ParticleEmitter = 12,
		/// <summary>
		/// Delta Spritefont used in the eninge! TTF font when used in sending from editor to server.
		/// </summary>
		Font = 13,
		/// <summary>
		/// Camera content, stores the initial camera position, rotation and values plus optionally a
		/// camera path this camera should follow.
		/// </summary>
		Camera = 15,
		/// <summary>
		/// Sound file for sound playback, just a .wav file on most platforms.
		/// </summary>
		Sound = 16,
		/// <summary>
		/// Music file for playing in the background or even use streaming. .mp3 or .ogg file.
		/// </summary>
		Music = 17,
		/// <summary>
		/// Video file for multimedia. Supports .mp4 and other video formats.
		/// </summary>
		Video = 18,
		/// <summary>
		/// Xml files for game specific content or whatever else you need.
		/// </summary>
		Xml = 20,
		/// <summary>
		/// Json files for game specific content, if you are a JavaScript freak.
		/// </summary>
		Json = 21,
		/// <summary>
		/// Input commands for a game or whatever else you need
		/// </summary>
		InputCommand = 24,
		/// <summary>
		/// Contains geometry data with its own VertexFormat and an identifier name.
		/// </summary>
		Geometry = 25,
		/// <summary>
		/// The data for a ParticleSystem contain names of saved emitter data to be used.
		/// Upon loading, those are loaded as well and the emitters are created and attached to the system.
		/// </summary>
		ParticleSystem = 26,
		/// <summary>
		/// Just store the file entry, this can be used for any file (binary makes most sense). All the
		/// logic to load and use this file has to be done by the application.
		/// </summary>
		JustStore = 29
	}
}