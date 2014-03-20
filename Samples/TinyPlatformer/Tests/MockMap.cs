using DeltaEngine.Content.Json;

namespace TinyPlatformer.Tests
{
	public static class MockMap
	{
		public static readonly JsonNode JsonNode = new JsonNode(Text);

		private const string Text = @"{ ""height"":6,
 ""layers"":[
				{
				 ""data"":
		 [5,5,5,5,5,5,5,5,5,0,0,0,0,0,0,5,5,0,0,0,0,0,0,5,5,0,0,0,0,0,0,5,5,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5],
				 ""height"":6,
				 ""name"":""background"",
				 ""opacity"":1,
				 ""type"":""tilelayer"",
				 ""visible"":true,
				 ""width"":8,
				 ""x"":0,
				 ""y"":0
				}, 
				{
				 ""height"":6,
				 ""name"":""Object Layer 1"",
				 ""objects"":[
								{
								 ""height"":32,
								 ""name"":""player"",
								 ""properties"":
										{

										},
								 ""type"":""player"",
								 ""visible"":true,
								 ""width"":32,
								 ""x"":128,
								 ""y"":96
								}, 
								{
								 ""height"":32,
								 ""name"":""monster"",
								 ""properties"":
										{
										 ""maxdx"":""10"",
										 ""right"":""true""
										},
								 ""type"":""monster"",
								 ""visible"":true,
								 ""width"":32,
								 ""x"":160,
								 ""y"":64
								},  
								{
								 ""height"":32,
								 ""name"":""treasure"",
								 ""properties"":
										{

										},
								 ""type"":""treasure"",
								 ""visible"":true,
								 ""width"":32,
								 ""x"":64,
								 ""y"":128
								}],
				 ""opacity"":1,
				 ""type"":""objectgroup"",
				 ""visible"":true,
				 ""width"":8,
				 ""x"":0,
				 ""y"":0
				}],
 ""orientation"":""orthogonal"",
 ""properties"":
		{

		},
 ""tileheight"":32,
 ""tilesets"":[
				{
				 ""firstgid"":1,
				 ""image"":""tiles.png"",
				 ""imageheight"":32,
				 ""imagewidth"":160,
				 ""margin"":0,
				 ""name"":""tiles"",
				 ""properties"":
						{

						},
				 ""spacing"":0,
				 ""tileheight"":32,
				 ""tilewidth"":32
				}],
 ""tilewidth"":32,
 ""version"":1,
 ""width"":8
}";
	}
}