using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeltaEngine.Core;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	class ParticleSystemPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			particleSystemPreviewer = new ParticleSystemPreviewer();
			particleSystemPreviewer.PreviewContent("TestParticle");
			new Camera2DScreenSpace(Resolve<Window>());
			AdvanceTimeAndUpdateEntities();
		}

		private ParticleSystemPreviewer particleSystemPreviewer;

		[Test]
		public void ShowParticleSystem()
		{
			particleSystemPreviewer.PreviewContent("TestParticleSystem");
		}
	}
}
