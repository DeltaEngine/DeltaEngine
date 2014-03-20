namespace DeltaEngine.Content.Mocks
{
	public static class MockSpineSkeleton
	{
		public const string Text = @"
{
""bones"": [
	{ ""name"": ""root"" },
	{ ""name"": ""hip"", ""parent"": ""root"", ""x"": 0.64, ""y"": 114.41 },
	{ ""name"": ""left upper leg"", ""parent"": ""hip"", ""length"": 50.39, ""x"": 14.45, ""y"": 2.81, ""rotation"": -89.09 },
	{ ""name"": ""pelvis"", ""parent"": ""hip"", ""x"": 1.41, ""y"": -6.57 },
	{ ""name"": ""right upper leg"", ""parent"": ""hip"", ""length"": 45.76, ""x"": -18.27, ""rotation"": -101.13 },
	{ ""name"": ""torso"", ""parent"": ""hip"", ""length"": 85.82, ""x"": -6.42, ""y"": 1.97, ""rotation"": 94.95 },
	{ ""name"": ""left lower leg"", ""parent"": ""left upper leg"", ""length"": 56.45, ""x"": 51.78, ""y"": 3.46, ""rotation"": -16.65 },
	{ ""name"": ""left shoulder"", ""parent"": ""torso"", ""length"": 44.19, ""x"": 78.96, ""y"": -15.75, ""rotation"": -156.96 },
	{ ""name"": ""neck"", ""parent"": ""torso"", ""length"": 18.38, ""x"": 83.64, ""y"": -1.78, ""rotation"": 0.9 },
	{ ""name"": ""right lower leg"", ""parent"": ""right upper leg"", ""length"": 58.52, ""x"": 50.21, ""y"": 0.6, ""rotation"": -10.7 },
	{ ""name"": ""right shoulder"", ""parent"": ""torso"", ""length"": 49.95, ""x"": 81.9, ""y"": 6.79, ""rotation"": 130.6 },
	{ ""name"": ""head"", ""parent"": ""neck"", ""length"": 68.28, ""x"": 19.09, ""y"": 6.97, ""rotation"": -8.94 },
	{ ""name"": ""left arm"", ""parent"": ""left shoulder"", ""length"": 35.62, ""x"": 44.19, ""y"": -0.01, ""rotation"": 28.16 },
	{ ""name"": ""left foot"", ""parent"": ""left lower leg"", ""length"": 46.5, ""x"": 64.02, ""y"": -8.67, ""rotation"": 102.43 },
	{ ""name"": ""right arm"", ""parent"": ""right shoulder"", ""length"": 36.74, ""x"": 49.95, ""y"": -0.12, ""rotation"": 40.12 },
	{ ""name"": ""right foot"", ""parent"": ""right lower leg"", ""length"": 45.45, ""x"": 64.88, ""y"": 0.04, ""rotation"": 110.3 },
	{ ""name"": ""left hand"", ""parent"": ""left arm"", ""length"": 11.52, ""x"": 35.62, ""y"": 0.07, ""rotation"": 2.7 },
	{ ""name"": ""right hand"", ""parent"": ""right arm"", ""length"": 15.32, ""x"": 36.9, ""y"": 0.34, ""rotation"": 2.35 }
],
""slots"": [
	{ ""name"": ""left shoulder"", ""bone"": ""left shoulder"", ""attachment"": ""left-shoulder"" },
	{ ""name"": ""left arm"", ""bone"": ""left arm"", ""attachment"": ""left-arm"" },
	{ ""name"": ""left hand"", ""bone"": ""left hand"", ""attachment"": ""left-hand"" },
	{ ""name"": ""left foot"", ""bone"": ""left foot"", ""attachment"": ""left-foot"" },
	{ ""name"": ""left lower leg"", ""bone"": ""left lower leg"", ""attachment"": ""left-lower-leg"" },
	{ ""name"": ""left upper leg"", ""bone"": ""left upper leg"", ""attachment"": ""left-upper-leg"" },
	{ ""name"": ""pelvis"", ""bone"": ""pelvis"", ""attachment"": ""pelvis"" },
	{ ""name"": ""right foot"", ""bone"": ""right foot"", ""attachment"": ""right-foot"" },
	{ ""name"": ""right lower leg"", ""bone"": ""right lower leg"", ""attachment"": ""right-lower-leg"" },
	{ ""name"": ""right upper leg"", ""bone"": ""right upper leg"", ""attachment"": ""right-upper-leg"" },
	{ ""name"": ""torso"", ""bone"": ""torso"", ""attachment"": ""torso"" },
	{ ""name"": ""neck"", ""bone"": ""neck"", ""attachment"": ""neck"" },
	{ ""name"": ""head"", ""bone"": ""head"", ""attachment"": ""head"" },
	{ ""name"": ""eyes"", ""bone"": ""head"", ""attachment"": ""eyes"" },
	{ ""name"": ""right shoulder"", ""bone"": ""right shoulder"", ""attachment"": ""right-shoulder"", ""additive"": true },
	{ ""name"": ""right arm"", ""bone"": ""right arm"", ""attachment"": ""right-arm"" },
	{ ""name"": ""right hand"", ""bone"": ""right hand"", ""attachment"": ""right-hand"" },
	{ ""name"": ""bb-head"", ""bone"": ""head"", ""attachment"": ""bb-head"" }
],
""skins"": {
	""default"": {
		""bb-head"": {
			""bb-head"": {
				""type"": ""boundingbox"",
				""vertices"": [
					55.69696,
					-44.60648,
					8.2226715,
					-47.609646,
					-11.244263,
					-32.942703,
					-0.05206299,
					35.835804,
					61.018433,
					43.227512,
					90.35846,
					-16.054127,
					115.41275,
					-32.817406,
					78.29431,
					-56.05409
				]
			}
		},
		""eyes"": {
			""eyes"": { ""x"": 28.94, ""y"": -32.92, ""rotation"": -86.9, ""width"": 34, ""height"": 27 },
			""eyes-closed"": { ""x"": 28.77, ""y"": -32.86, ""rotation"": -86.9, ""width"": 34, ""height"": 27 }
		},
		""head"": {
			""head"": { ""x"": 53.94, ""y"": -5.75, ""rotation"": -86.9, ""width"": 121, ""height"": 132 }
		},
		""left arm"": {
			""left-arm"": { ""x"": 15.11, ""y"": -0.44, ""rotation"": 33.84, ""width"": 35, ""height"": 29 }
		},
		""left foot"": {
			""left-foot"": { ""x"": 24.35, ""y"": 8.88, ""rotation"": 3.32, ""width"": 65, ""height"": 30 }
		},
		""left hand"": {
			""left-hand"": { ""x"": 0.75, ""y"": 1.86, ""rotation"": 31.14, ""width"": 35, ""height"": 38 }
		},
		""left lower leg"": {
			""left-lower-leg"": { ""x"": 24.55, ""y"": -1.92, ""rotation"": 105.75, ""width"": 49, ""height"": 64 }
		},
		""left shoulder"": {
			""left-shoulder"": { ""x"": 23.74, ""y"": 0.11, ""rotation"": 62.01, ""width"": 34, ""height"": 53 }
		},
		""left upper leg"": {
			""left-upper-leg"": { ""x"": 26.12, ""y"": -1.85, ""rotation"": 89.09, ""width"": 33, ""height"": 67 }
		},
		""neck"": {
			""neck"": { ""x"": 9.42, ""y"": -3.66, ""rotation"": -100.15, ""width"": 34, ""height"": 28 }
		},
		""pelvis"": {
			""pelvis"": { ""x"": -4.83, ""y"": 10.62, ""width"": 63, ""height"": 47 }
		},
		""right arm"": {
			""right-arm"": { ""x"": 18.34, ""y"": -2.64, ""rotation"": 94.32, ""width"": 21, ""height"": 45 }
		},
		""right foot"": {
			""right-foot"": { ""x"": 19.02, ""y"": 8.47, ""rotation"": 1.52, ""width"": 67, ""height"": 30 }
		},
		""right hand"": {
			""right-hand"": { ""x"": 6.82, ""y"": 1.25, ""rotation"": 91.96, ""width"": 32, ""height"": 32 }
		},
		""right lower leg"": {
			""right-lower-leg"": { ""x"": 23.28, ""y"": -2.59, ""rotation"": 111.83, ""width"": 51, ""height"": 64 }
		},
		""right shoulder"": {
			""right-shoulder"": { ""x"": 25.86, ""y"": 0.03, ""rotation"": 134.44, ""width"": 52, ""height"": 51 }
		},
		""right upper leg"": {
			""right-upper-leg"": { ""x"": 23.03, ""y"": 0.25, ""rotation"": 101.13, ""width"": 44, ""height"": 70 }
		},
		""torso"": {
			""torso"": { ""x"": 44.57, ""y"": -7.08, ""rotation"": -94.95, ""width"": 68, ""height"": 92 }
		}
	}
},
""events"": {
	""behind"": { ""int"": 4 },
	""headAttach"": {},
	""headPop"": {}
},
""animations"": {
	""drawOrder"": {
		""bones"": {
			""head"": {
				""rotate"": [
					{ ""time"": 0, ""angle"": 0 },
					{ ""time"": 0.4827, ""angle"": -23.11 },
				],
				""translate"": [
					{
						""time"": 0,
						""x"": 0,
						""y"": 0,
						""curve"": [ 0.19, 0.4, 0.586, 0.75 ]
					},
					{ ""time"": 4, ""x"": 0, ""y"": 0 }
				],
				""scale"": [
					{ ""time"": 0.8275, ""x"": 1, ""y"": 1 },
					{ ""time"": 1.3103, ""x"": 0.742, ""y"": 0.742 },
				]
			}
		},
		""events"": [
			{ ""time"": 0, ""name"": ""headPop"", ""string"": ""pop.wav"" },
			{ ""time"": 1.3103, ""name"": ""behind"" },
			{ ""time"": 2.9655, ""name"": ""behind"" },
			{ ""time"": 4, ""name"": ""headAttach"", ""string"": ""attach.wav"" }
		],
		""draworder"": [
			{
				""time"": 0.6206,
				""offsets"": [
					{ ""slot"": ""head"", ""offset"": -12 },
					{ ""slot"": ""eyes"", ""offset"": -12 }
				]
			},
			{
				""time"": 1.7931,
				""offsets"": [
					{ ""slot"": ""head"", ""offset"": 3 },
					{ ""slot"": ""eyes"", ""offset"": 3 }
				]
			},
			{
				""time"": 2.6206,
				""offsets"": [
					{ ""slot"": ""head"", ""offset"": -12 },
					{ ""slot"": ""eyes"", ""offset"": -12 }
				]
			},
			{ ""time"": 3.5862 }
		]
	},
	""jump"": {
		""bones"": {
			""hip"": {
				""rotate"": [
					{ ""time"": 0, ""angle"": 0, ""curve"": ""stepped"" },
					{ ""time"": 0.9333, ""angle"": 0, ""curve"": ""stepped"" },
					{ ""time"": 1.3666, ""angle"": 0 }
				],
				""translate"": [
					{ ""time"": 0, ""x"": -11.57, ""y"": -3 },
					{ ""time"": 0.2333, ""x"": -16.2, ""y"": -19.43 },
					{
						""time"": 0.3333,
						""x"": 7.66,
						""y"": -8.48,
						""curve"": [ 0.057, 0.06, 0.712, 1 ]
					},
					{ ""time"": 0.3666, ""x"": 15.38, ""y"": 5.01 },
					{ ""time"": 0.4666, ""x"": -7.84, ""y"": 57.22 },
					{
						""time"": 0.6,
						""x"": -10.81,
						""y"": 96.34,
						""curve"": [ 0.241, 0, 1, 1 ]
					},
					{ ""time"": 0.7333, ""x"": -7.01, ""y"": 54.7 },
					{ ""time"": 0.8, ""x"": -10.58, ""y"": 32.2 },
				],
				""scale"": [
					{ ""time"": 0, ""x"": 1, ""y"": 1, ""curve"": ""stepped"" },
					{ ""time"": 0.9333, ""x"": 1, ""y"": 1, ""curve"": ""stepped"" },
					{ ""time"": 1.3666, ""x"": 1, ""y"": 1 }
				]
			}
		}
	},
	""walk"": {
		""bones"": {
			""left upper leg"": {
				""rotate"": [
					{ ""time"": 0, ""angle"": -26.55 },
					{ ""time"": 0.1333, ""angle"": -8.78 },
				],
				""translate"": [
					{ ""time"": 0, ""x"": -3, ""y"": -2.25 },
					{ ""time"": 0.4, ""x"": -2.18, ""y"": -2.25 },
				]
			},
			""right upper leg"": {
				""rotate"": [
					{ ""time"": 0, ""angle"": 42.45 },
					{ ""time"": 0.1333, ""angle"": 52.1 },
				],
				""translate"": [
					{ ""time"": 0, ""x"": 8.11, ""y"": -2.36 },
					{ ""time"": 0.1333, ""x"": 10.03, ""y"": -2.56 },
				]
			},
			""head"": {
				""rotate"": [
					{
						""time"": 0,
						""angle"": 3.6,
						""curve"": [ 0, 0, 0.704, 1.61 ]
					},
					{ ""time"": 0.1666, ""angle"": -0.2 },
				]
			}
		}
	}
}
}";
	}
}
