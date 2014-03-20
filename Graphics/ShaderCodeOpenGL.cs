namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Default source code for the OpenGL shaders
	/// Note: When the next ShaderFlag feature will be added the shader code needs to be generated on
	/// the Server.
	/// </summary>
	public static class ShaderCodeOpenGL
	{
		public const string PositionUVOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
attribute vec4 aPosition;
attribute vec2 aTextureUV;
varying vec2 vTexcoord;
void main()
{
	vTexcoord = aTextureUV;
	gl_Position = ModelViewProjection * aPosition;
}";

		public const string PositionUVOpenGLFragmentCode = @"
precision mediump float;
uniform sampler2D Texture;
varying vec2 vTexcoord;
void main()
{
	gl_FragColor = texture2D(Texture, vTexcoord);
}";

		public const string PositionColorOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
attribute vec4 aPosition;
attribute vec4 aColor;
varying vec4 diffuseColor;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	diffuseColor = aColor;
}";

		public const string PositionColorOpenGLFragmentCode = @"
precision mediump float;
varying vec4 diffuseColor;
void main()
{
	gl_FragColor = diffuseColor;
}";

		public const string PositionColorUVOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
attribute vec4 aPosition;
attribute vec4 aColor;
attribute vec2 aTextureUV;
varying vec4 diffuseColor;
varying vec2 diffuseTexCoord;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	diffuseColor = aColor;
	diffuseTexCoord = aTextureUV;
}";

		public const string PositionColorUVOpenGLFragmentCode = @"
precision mediump float;
uniform sampler2D Texture;
varying vec4 diffuseColor;
varying vec2 diffuseTexCoord;
void main()
{
	gl_FragColor = texture2D(Texture, diffuseTexCoord) * diffuseColor;
}";

		public const string PositionUVLightmapVertexCode = @"
uniform mat4 ModelViewProjection;
attribute vec4 aPosition;
attribute vec2 aTextureUV;
attribute vec2 aLightMapUV;
varying vec2 vDiffuseTexCoord;
varying vec2 vLightMapTexCoord;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	vDiffuseTexCoord = aTextureUV;
	vLightMapTexCoord = aLightMapUV;
}";

		public const string PositionUVLightmapFragmentCode = @"
precision mediump float;
uniform sampler2D Texture;
uniform sampler2D Lightmap;
varying vec2 vDiffuseTexCoord;
varying vec2 vLightMapTexCoord;
void main()
{
	gl_FragColor = texture2D(Texture, vDiffuseTexCoord) * texture2D(Lightmap, vLightMapTexCoord);
}";

		public const string PositionUVSkinnedOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
uniform mat4 JointTransforms[32];
attribute vec4 aPosition;
attribute vec2 aTextureUV;
attribute vec2 aSkinIndices;
attribute vec2 aSkinWeights;
varying vec2 vTexcoord;
void main()
{
	vec4 skinnedPosition = vec4(0.0, 0.0, 0.0, 0.0);
	for (int jointIndex = 0; jointIndex < 2; ++jointIndex)
	{
		int index = int(aSkinIndices[jointIndex]);
		float weight = aSkinWeights[jointIndex];
		skinnedPosition += (JointTransforms[index] * aPosition) * weight;
	} 
	gl_Position = ModelViewProjection * skinnedPosition;
	vTexcoord = aTextureUV;
}";

		public const string PositionUVSkinnedOpenGLFragmentCode = PositionUVOpenGLFragmentCode;

		public const string PositionColorFogOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
uniform mat4 ModelView;
uniform vec3 fogColor;
uniform float fogStart;
uniform float fogEnd;
attribute vec4 aPosition;
attribute vec4 aColor;
varying vec4 diffuseColor;
varying vec4 fogMixColor;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	diffuseColor = aColor;

	vec4 vertexPositionInView = ModelView * aPosition;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - fogStart) / (fogEnd - fogStart), 0.0, 1.0);
	fogMixColor = vec4(fogColor.rgb, fogFactor);
}";

		public const string PositionColorFogOpenGLFragmentCode = @"
precision mediump float;
varying vec4 diffuseColor;
varying vec4 fogMixColor;
void main()
{
	gl_FragColor = mix(diffuseColor, vec4(fogMixColor.rgb, 1.0), fogMixColor.a);
}";

		public const string PositionUVFogOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
uniform mat4 ModelView;
uniform vec3 fogColor;
uniform float fogStart;
uniform float fogEnd;
attribute vec4 aPosition;
attribute vec2 aTextureUV;
varying vec2 vTexcoord;
varying vec4 fogMixColor;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	vTexcoord = aTextureUV;
	vec4 vertexPositionInView = ModelView * aPosition;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - fogStart) / (fogEnd - fogStart), 0.0, 1.0);
	fogMixColor = vec4(fogColor.rgb, fogFactor);
}";

		public const string PositionUVFogOpenGLFragmentCode = @"
precision mediump float;
uniform sampler2D Texture;
varying vec2 vTexcoord;
varying vec4 fogMixColor;
void main()
{
	vec4 diffuseColor = texture2D(Texture, vTexcoord);
	gl_FragColor = mix(diffuseColor, vec4(fogMixColor.rgb, diffuseColor.a), fogMixColor.a);
}";

		public const string PositionColorUVFogOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
uniform mat4 ModelView;
uniform vec3 fogColor;
uniform float fogStart;
uniform float fogEnd;
attribute vec4 aPosition;
attribute vec4 aColor;
attribute vec2 aTextureUV;
varying vec4 vertexColor;
varying vec2 vTexcoord;
varying vec4 fogMixColor;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	vertexColor = aColor;
	vTexcoord = aTextureUV;

	vec4 vertexPositionInView = ModelView * aPosition;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - fogStart) / (fogEnd - fogStart), 0.0, 1.0);
	fogMixColor = vec4(fogColor.rgb, fogFactor);
}";

		public const string PositionColorUVFogOpenGLFragmentCode = @"
precision mediump float;
uniform sampler2D Texture;
varying vec2 vTexcoord;
varying vec4 vertexColor;
varying vec4 fogMixColor;
void main()
{
	vec4 diffuseColor = texture2D(Texture, vTexcoord) * vertexColor;
	gl_FragColor = mix(diffuseColor, vec4(fogMixColor.rgb, diffuseColor.a), fogMixColor.a);
}";

		public const string PositionUVLightmapFogVertexCode = @"
uniform mat4 ModelViewProjection;
uniform mat4 ModelView;
uniform vec3 fogColor;
uniform float fogStart;
uniform float fogEnd;
attribute vec4 aPosition;
attribute vec2 aTextureUV;
attribute vec2 aLightMapUV;
varying vec2 vDiffuseTexCoord;
varying vec2 vLightMapTexCoord;
varying vec4 fogMixColor;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	vDiffuseTexCoord = aTextureUV;
	vLightMapTexCoord = aLightMapUV;

	vec4 vertexPositionInView = ModelView * aPosition;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - fogStart) / (fogEnd - fogStart), 0.0, 1.0);
	fogMixColor = vec4(fogColor.rgb, fogFactor);
}";

		public const string PositionUVLightmapFogFragmentCode = @"
precision mediump float;
uniform sampler2D Texture;
uniform sampler2D Lightmap;
varying vec2 vDiffuseTexCoord;
varying vec2 vLightMapTexCoord;
varying vec4 fogMixColor;
void main()
{
	vec4 diffuseColor = texture2D(Texture, vDiffuseTexCoord) * texture2D(Lightmap, vLightMapTexCoord);
	gl_FragColor = mix(diffuseColor, vec4(fogMixColor.rgb, diffuseColor.a), fogMixColor.a);
}";

		public const string PositionUVSkinnedFogOpenGLVertexCode = @"
uniform mat4 ModelViewProjection;
uniform mat4 JointTransforms[32];
uniform mat4 ModelView;
uniform vec3 fogColor;
uniform float fogStart;
uniform float fogEnd;
attribute vec4 aPosition;
attribute vec2 aTextureUV;
attribute vec2 aSkinIndices;
attribute vec2 aSkinWeights;
varying vec2 vTexcoord;
varying vec4 fogMixColor;
void main()
{
	vec4 skinnedPosition = vec4(0.0, 0.0, 0.0, 0.0);
	for (int jointIndex = 0; jointIndex < 2; ++jointIndex)
	{
		int index = int(aSkinIndices[jointIndex]);
		float weight = aSkinWeights[jointIndex];
		skinnedPosition += (JointTransforms[index] * aPosition) * weight;
	}
	gl_Position = ModelViewProjection * skinnedPosition;
	vTexcoord = aTextureUV;

	vec4 vertexPositionInView = ModelView * skinnedPosition;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - fogStart) / (fogEnd - fogStart), 0.0, 1.0);
	fogMixColor = vec4(fogColor.rgb, fogFactor);
}";

		public const string PositionUVSkinnedFogOpenGLFragmentCode = PositionUVFogOpenGLFragmentCode;
		
		public const string PositionUVNormalOpenGLVertexCode = @"
attribute vec4 aPosition;
attribute vec4 aNormal;
attribute vec2 aTextureUV;
uniform mat4 ModelViewProjection;
uniform mat4 View;
uniform mat4 Normal;
uniform vec3 lightDir;
uniform vec4 lightColor;
varying vec2 vTexCoord;
varying	vec3 vEyeSpaceVertexNormal;
varying	vec4 vLightingColor;

void main()
{
	vTexCoord = aTextureUV;
	vec3 eyeSpaceVertexNormal = (Normal * aNormal).xyz;
	vec3 lightDirection = (View * vec4(lightDir, 0.0)).xyz;
	float lightingFactor = max(0.0, dot(eyeSpaceVertexNormal, lightDirection));
  vLightingColor = vec4(0.2, 0.2, 0.2, 1.0) + lightingFactor * lightColor;
	gl_Position = ModelViewProjection * aPosition;
}";

		public const string PositionUVNormalOpenGLFragmentCode = @"
precision mediump float;
uniform sampler2D Texture;
varying vec2 vTexCoord;
varying	vec4 vLightingColor;

void main()
{
	vec4 diffuseColor = texture2D(Texture, vTexCoord);
	gl_FragColor = diffuseColor * vLightingColor;
}";
	}
}