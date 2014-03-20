using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering3D.Particles
{
	public class ParticleSystemData : ContentData
	{
		public ParticleSystemData(string contentName)
			: base(contentName) { }

		public ParticleSystemData()
			: base("<GeneratedParticleSystemData>")
		{
			emitterNames = new List<string>();
		}

		public List<string> emitterNames;

		protected override void DisposeData() { }
		protected override void LoadData(Stream fileData)
		{
			var namesFromMetaData = MetaData.Get("EmitterNames", " ");
			if (string.IsNullOrEmpty(namesFromMetaData))
				throw new InvalidParticleSystemDataNoEmitterNames(); //ncrunch: no coverage
			var names = namesFromMetaData.SplitAndTrim(new[] { ' ', ',' });
			emitterNames = new List<string>();
			foreach (var emitterName in names)
				emitterNames.Add(emitterName);
		}

		public class InvalidParticleSystemDataNoEmitterNames : Exception {}
	}
}
