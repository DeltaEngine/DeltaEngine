using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Particles;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class ParticleEditorViewModel : ViewModelBase
	{
		public ParticleEditorViewModel(Service service)
		{
			this.service = service;
			CreateInstances();
			UpdateDataForLoad();
			SetInitialDefaults();
			Messenger.Default.Register<ContentIconAndName>(this, "SelectContentInContentManager",
				SelectContentInContentManager);
		}

		private void CreateInstances()
		{
			EffectsInProject = new ObservableCollection<string>();
			SpawnerTypeList = new ObservableCollection<ParticleEmitterPositionType>();
			MaterialList = new ObservableCollection<string>();
			AvailableTemplates = new ObservableCollection<string>();
			metaDataCreator = new ContentMetaDataCreator();
			EmittersInProject = new List<string>();
			BillboardModeList = new List<string>();
		}

		private void SetInitialDefaults()
		{
			TemplateListVisibility = Visibility.Hidden;
			SavedEmitterSelectionVisibility = Visibility.Hidden;
			currentEmitterInEffect = 0;
			ParticleSystemName = "MyParticleSystem";
			OverwriteOnSave = true;
			if (LoadEffect())
				return; //ncrunch: no coverage
			currentEffect = new ParticleSystem();
			AddEmitterToSystem();
			CenterViewOnCurrentEffect();
		}
		
		internal void UpdateDataForLoad()
		{
			GetListOfSpawnerTypes();
			GetListOfBillboardModes();
			GetMaterials();
			GetSavedEffects();
		}

		private void GetListOfSpawnerTypes()
		{
			SpawnerTypeList.Clear();
			foreach (ParticleEmitterPositionType spawnerType in
				Enum.GetValues(typeof(ParticleEmitterPositionType)))
				SpawnerTypeList.Add(spawnerType);
		}

		private void GetListOfBillboardModes()
		{
			BillboardModeList.Clear();
			var billboardModes = Enum.GetValues(typeof(BillboardMode));
			foreach (var mode in billboardModes)
				BillboardModeList.Add(mode.ToString());
		}

		private void GetMaterials()
		{
			MaterialList.Clear();
			MaterialList.Add("Default2D");
			var materialList = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in materialList.Where(material => MaterialHasColor(material)))
				MaterialList.Add(material); //ncrunch: no coverage
		}

		private static bool MaterialHasColor(string material)
		{
			try
			{
				return TryMaterialHasColor(material);
			}
			catch (Exception) //ncrunch: no coverage start
			{
				return false;
			} //ncrunch: no coverage end
		}

		private static bool TryMaterialHasColor(string material)
		{
			return (ContentLoader.Load<Material>(material).Shader as ShaderWithFormat).Format.HasColor;
		}

		private void GetSavedEffects()
		{
			EffectsInProject.Clear();
			var foundParticles = service.GetAllContentNamesByType(ContentType.ParticleSystem);
			foreach (var particle in foundParticles)
				EffectsInProject.Add(particle);
		}

		private void SelectContentInContentManager(ContentIconAndName content)
		{
			var type = service.GetTypeOfContent(content.Name);
			if (type == ContentType.ParticleSystem)
			{
				particleSystemName = content.Name;
				LoadEffect();
			}
		}

		public ObservableCollection<string> EffectsInProject { get; set; }
		public ObservableCollection<ParticleEmitterPositionType> SpawnerTypeList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		private readonly Service service;
		private ContentMetaDataCreator metaDataCreator;

		public ParticleSystem currentEffect;
		private int currentEmitterInEffect;

		public void AddEmitterToSystem(ParticleEmitter existingEmitter = null)
		{
			currentEffect.AttachEmitter(existingEmitter ??
				new ParticleEmitter(CreateDefaultEmitterData(), Vector3D.Zero));
			currentEmitterInEffect = currentEffect.AttachedEmitters.Count - 1;
			RefreshAllEffectProperties();
			RaisePropertyChanged("AvailableEmitterIndices");
		}

		private static ParticleEmitterData CreateDefaultEmitterData()
		{
			return new ParticleEmitterData
			{
				ParticleMaterial = ContentExtensions.CreateDefaultMaterial2D(),
				LifeTime = 1,
				Size = new RangeGraph<Size>(new Size(0.01f, 0.01f), new Size(0.1f, 0.1f)),
				SpawnInterval = 0.1f,
				MaximumNumberOfParticles = 128,
				StartVelocity =
					new RangeGraph<Vector3D>(new Vector3D(-0.1f, -0.2f, 0), new Vector3D(0.4f, -0.2f, 0)),
				Acceleration =
					new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.1f, 0.0f), new Vector3D(0.0f, 0.3f, 0.0f))
			};
		}

		public void RemoveCurrentEmitterFromSystem()
		{
			if (currentEffect.AttachedEmitters.Count == 1)
			{
				ResetDefaultEffect();
				return;
			}
			currentEffect.DisposeEmitter(currentEmitterInEffect);
			currentEmitterInEffect = 0;
			RefreshAllEffectProperties();
			RaisePropertyChanged("AvailableEmitterIndices");
		}

		public string ParticleSystemName
		{
			get { return particleSystemName; }
			set
			{
				particleSystemName = value;
				CanLoadEffect = ContentLoader.Exists(particleSystemName, ContentType.ParticleSystem);
				RaisePropertyChanged("ParticleSystemName");
				RaisePropertyChanged("CanLoadEffect");
			}
		}

		private string particleSystemName;

		public bool CanLoadEffect { get; private set; }

		public bool LoadEffect()
		{
			try
			{
				return TryLoadParticleEffect(particleSystemName);
			}
			//ncrunch: no coverage start
			catch
			{
				Logger.Warning("Failed to load particle effect " + particleSystemName);
				ResetDefaultEffect();
				return false;
			} //ncrunch: no coverage end
		}

		public bool TryLoadParticleEffect(string effectName)
		{
			if (!ContentLoader.Exists(effectName, ContentType.ParticleSystem))
				return false;
			var effectData = ContentLoader.Load<ParticleSystemData>(effectName);
			DestroyAllEmitters();
			currentEffect = new ParticleSystem(effectData);
			currentEmitterInEffect = 0;
			RefreshAllEffectProperties();
			RaisePropertyChanged("AvailableEmitterIndices");
			CenterViewOnCurrentEffect();
			return true;
		}

		private void DestroyAllEmitters()
		{
			if (currentEffect != null)
				currentEffect.DisposeSystem();
		}

		public void Save()
		{
			TrySavingEffectData(ParticleSystemName, OverwriteOnSave);
		}

		public bool OverwriteOnSave { get; set; }

		public bool TrySavingEffectData(string effectName, bool overwrite = false)
		{
			if (string.IsNullOrEmpty(effectName))
			{
				Logger.Info("Saving of content data requires a name, cannot save with an empty name.");
				return false;
			}
			if (!MaySaveRegardingExistingAndOverwrite(effectName, ContentType.ParticleSystem, overwrite))
			{
				WarnExistantContentNoOverwrite(effectName, ContentType.ParticleSystem);
				return false;
			}
			var emitterNames = new List<string>();
			for (int i = 0; i < currentEffect.AttachedEmitters.Count; i++)
			{
				var emitterName = effectName + "Emitter" + i.ToString(CultureInfo.InvariantCulture);
				if (
					!TrySavingEmitterData(currentEffect.AttachedEmitters[i].EmitterData, emitterName,
						overwrite))
					return false; //ncrunch: no coverage
				emitterNames.Add(emitterName);
			}
			var metaData = metaDataCreator.CreateParticleSystemData(effectName, emitterNames);
			service.UploadContent(metaData);
			service.ContentUpdated += LogSuccessMessage;
			return true;
		}

		private bool TrySavingEmitterData(ParticleEmitterData emitterData, string name,
			bool overwrite)
		{
			if (!MaySaveRegardingExistingAndOverwrite(name, ContentType.ParticleEmitter, overwrite))
			{
				//ncrunch: no coverage start
				WarnExistantContentNoOverwrite(name, ContentType.ParticleEmitter);
				return false;
			} //ncrunch: no coverage end
			var bytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(emitterData);
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(name + ".deltaparticle", bytes);
			ContentMetaData metaData = metaDataCreator.CreateMetaDataFromParticle(name, bytes);
			service.UploadContent(metaData, fileNameAndBytes);
			return true;
		}

		private static bool MaySaveRegardingExistingAndOverwrite(string name, ContentType type,
			bool overwrite)
		{
			if (ContentLoader.Exists(name, type))
				return overwrite;
			return true;
		}

		private ParticleEmitter EmitterModified
		{
			get { return currentEffect.AttachedEmitters[currentEmitterInEffect]; }
		}

		private void RecreateEmitter(int index)
		{
			var emitterData = currentEffect.AttachedEmitters[index].EmitterData;
			currentEffect.AttachedEmitters[index].IsActive = false;
			currentEffect.AttachedEmitters[index] = new ParticleEmitter(emitterData,
				currentEffect.Position);
		}

		public int MaxNumberOfParticles
		{
			get { return EmitterModified.EmitterData.MaximumNumberOfParticles; }
			set
			{
				var emitterData = EmitterModified.EmitterData;
				emitterData.MaximumNumberOfParticles = value.Clamp(0, 1024);
				RecreateEmitter(currentEmitterInEffect);
				RaisePropertyChanged("MaxNumberOfParticles");
			}
		}

		public string SelectedMaterialName
		{
			get
			{
				var retrievedName = EmitterModified.EmitterData.ParticleMaterial.Name;
				if (!retrievedName.StartsWith("<Generated"))
					return retrievedName;
				if ((EmitterModified.EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format.Is3D)
					return "Default3D;"; //ncrunch: no coverage
				return "Default2D";
			}
			set
			{
				if (String.IsNullOrEmpty(value))
					return;
				try
				{
					selectedMaterialName = value;
					var emitterData = EmitterModified.EmitterData;
					emitterData.ParticleMaterial = selectedMaterialName == "Default2D"
						? ContentExtensions.CreateDefaultMaterial2D()
						: ContentLoader.Load<Material>(selectedMaterialName);
					RecreateEmitter(currentEmitterInEffect);
					RaisePropertyChanged("SelectedMaterial");
				}
				catch
				{
					Logger.Warning("Material " + value + "failed to load!");
				}
			}
		}

		private string selectedMaterialName;

		public ParticleEmitterPositionType SelectedSpawnerType
		{
			get { return EmitterModified.EmitterData.PositionType; }
			set
			{
				EmitterModified.EmitterData.PositionType = value;
				RaisePropertyChanged("SelectedSpawnerType");
			}
		}

		public RangeGraph<Size> Size
		{
			get { return EmitterModified.EmitterData.Size; }
			set
			{
				if (value != null)
					EmitterModified.EmitterData.Size = value;
				RaisePropertyChanged("Size");
			}
		}

		public RangeGraph<Color> Color
		{
			get { return EmitterModified.EmitterData.Color; }
			set
			{
				if (value != null)
					EmitterModified.EmitterData.Color = value;
				RaisePropertyChanged("Color");
			}
		}

		public RangeGraph<Vector3D> StartPosition
		{
			get { return EmitterModified.EmitterData.StartPosition; }
			set
			{
				if (value != null)
					EmitterModified.EmitterData.StartPosition = value;
				RaisePropertyChanged("StartPosition");
			}
		}

		public RangeGraph<ValueRange> StartRotation
		{
			get { return EmitterModified.EmitterData.StartRotation; }
			set
			{
				if (value != null)
					EmitterModified.EmitterData.StartRotation = value;
				RaisePropertyChanged("StartRotation");
			}
		}

		public RangeGraph<Vector3D> StartVelocity
		{
			get { return EmitterModified.EmitterData.StartVelocity; }
			set
			{
				if (value != null)
					EmitterModified.EmitterData.StartVelocity = value;
				RaisePropertyChanged("StartVelocity");
			}
		}

		public RangeGraph<Vector3D> Acceleration
		{
			get { return EmitterModified.EmitterData.Acceleration; }
			set
			{
				if (value != null)
					EmitterModified.EmitterData.Acceleration = value;
				RaisePropertyChanged("Acceleration");
			}
		}

		public RangeGraph<ValueRange> RotationSpeed
		{
			get { return EmitterModified.EmitterData.RotationSpeed; }
			set
			{
				if (value != null)
					EmitterModified.EmitterData.RotationSpeed = value;
				RaisePropertyChanged("RotationSpeed");
			}
		}

		public float SpawnInterval
		{
			get { return EmitterModified.EmitterData.SpawnInterval; }
			set
			{
				EmitterModified.EmitterData.SpawnInterval = value;
				RaisePropertyChanged("SpawnInterval");
			}
		}

		public float LifeTime
		{
			get { return EmitterModified.EmitterData.LifeTime; }
			set
			{
				if (value >= 0)
					EmitterModified.EmitterData.LifeTime = value;
				RaisePropertyChanged("LifeTime");
			}
		}

		public int ParticlesPerSpawn
		{
			get { return (int)EmitterModified.EmitterData.ParticlesPerSpawn.Start.Start; }
			set
			{
				EmitterModified.EmitterData.ParticlesPerSpawn =
					new RangeGraph<ValueRange>(new ValueRange(value, value));
				RaisePropertyChanged("ParticlesPerSpawn");
			}
		}

		public string SelectedBillboardMode
		{
			get { return EmitterModified.EmitterData.BillboardMode.ToString(); }
			set
			{
				EmitterModified.EmitterData.BillboardMode =
					(BillboardMode)Enum.Parse(typeof(BillboardMode), value);
				RaisePropertyChanged("SelectedBillboardMode");
			}
		}

		public bool DoParticlesTrackEmitter
		{
			get { return EmitterModified.EmitterData.DoParticlesTrackEmitter; }
			set
			{
				EmitterModified.EmitterData.DoParticlesTrackEmitter = value;
				RaisePropertyChanged("DoParticlesTrackEmitter");
			}
		}

		public List<string> BillboardModeList { get; set; }

		private static void WarnExistantContentNoOverwrite(string name, ContentType type)
		{
			var message = "Tried to save " + type + name;
			message += ", which already is existant. Set overwrite if that's what you want!";
			Logger.Warning(message);
		}

		private void LogSuccessMessage(ContentType type, string name)
		{
			Logger.Info("Successfully saved: " + name + " of type " + type.ToString());
			service.ContentUpdated -= LogSuccessMessage;
		}

		private void RefreshAllEffectProperties()
		{
			RaisePropertyChanged("ParticleSystemName");
			RaisePropertyChanged("SelectedMaterialName");
			RaisePropertyChanged("SelectedSpawnerType");
			RaisePropertyChanged("Size");
			RaisePropertyChanged("Color");
			RaisePropertyChanged("StartPosition");
			RaisePropertyChanged("StartRotation");
			RaisePropertyChanged("StartVelocity");
			RaisePropertyChanged("Acceleration");
			RaisePropertyChanged("RotationSpeed");
			RaisePropertyChanged("SpawnInterval");
			RaisePropertyChanged("LifeTime");
			RaisePropertyChanged("ParticlesPerSpawn");
			RaisePropertyChanged("SelectedBillboardMode");
			RaisePropertyChanged("MaxNumberOfParticles");
			RaisePropertyChanged("DoParticlesTrackEmitter");
			RaisePropertyChanged("CurrentEmitterInEffect");
		}

		public bool CanModifyEmitters
		{
			get { return currentEffect != null; }
		}

		public int CurrentEmitterInEffect
		{
			get { return currentEmitterInEffect; }
			set
			{
				currentEmitterInEffect = value.Clamp(0, currentEffect.AttachedEmitters.Count);
				RefreshAllEffectProperties();
			}
		}

		public int[] AvailableEmitterIndices
		{
			get
			{
				var indices = new int[currentEffect.AttachedEmitters.Count];
				for (int i = 0; i < indices.Length; i++)
					indices[i] = i;
				return indices;
			}
		}

		public void ResetDefaultEffect()
		{
			ParticleSystemName = "";
			DestroyAllEmitters();
			currentEffect = new ParticleSystem();
			currentEmitterInEffect = 0;
			CenterViewOnCurrentEffect();
			AddEmitterToSystem();
			RefreshAllEffectProperties();
			RaisePropertyChanged("AvailableEmitterIndices");
			SavedEmitterSelectionVisibility = Visibility.Hidden;
			RaisePropertyChanged("SavedEmitterSelectionVisibility");
			TemplateListVisibility = Visibility.Hidden;
			RaisePropertyChanged("TemplateListVisibility");
		}

		public void ToggleLookingForExistingEmitters()
		{
			if (SavedEmitterSelectionVisibility == Visibility.Visible)
			{
				SavedEmitterSelectionVisibility = Visibility.Hidden;
				RaisePropertyChanged("SavedEmitterSelectionVisibility");
				return;
			}
			EmittersInProject.Clear();
			var foundEmitters = service.GetAllContentNamesByType(ContentType.ParticleEmitter);
			foreach (var emitterName in foundEmitters)
				EmittersInProject.Add(emitterName);
			SavedEmitterSelectionVisibility = Visibility.Visible;
			RaisePropertyChanged("EmittersInProject");
			RaisePropertyChanged("SavedEmitterSelectionVisibility");
		}

		public List<string> EmittersInProject { get; set; }

		public string ParticleEmitterNameToAdd
		{
			get { return ""; }
			set
			{
				if (!ContentLoader.Exists(value))
					return;
				AddEmitterToSystem(new ParticleEmitter(ContentLoader.Load<ParticleEmitterData>(value),
					Vector3D.Zero));
				SavedEmitterSelectionVisibility = Visibility.Hidden;
				RaisePropertyChanged("SavedEmitterSelectionVisibility");
			}
		}

		public Visibility SavedEmitterSelectionVisibility { get; private set; }

		public void ToggleLookingForTemplateEffect()
		{
			if (TemplateListVisibility == Visibility.Visible)
			{
				TemplateListVisibility = Visibility.Hidden;
				RaisePropertyChanged("TemplateListVisibility");
				return;
			}
			AvailableTemplates.Clear();
			foreach (var availableTemplateName in availableTemplateNames)
				AvailableTemplates.Add(availableTemplateName);
			TemplateListVisibility = Visibility.Visible;
			RaisePropertyChanged("AvailableTemplates");
			RaisePropertyChanged("TemplateListVisibility");
		}

		public Visibility TemplateListVisibility { get; set; }

		private readonly string[] availableTemplateNames = { "Point Fountain" };

		public string TemplateNameToLoad
		{
			get { return " "; }
			set
			{
				if (string.IsNullOrEmpty(value) || !AvailableTemplates.Contains(value))
					return;
				DestroyAllEmitters();
				currentEffect = new ParticleSystem();
				if (value == availableTemplateNames[0])
				{
					var emitterData = new ParticleEmitterData
					{
						ParticleMaterial = ContentExtensions.CreateDefaultMaterial2D(),
						LifeTime = 1,
						MaximumNumberOfParticles = 128,
						Size = new RangeGraph<Size>(new Size(0.05f), new Size(0.0f)),
						SpawnInterval = 0.1f,
						StartVelocity =
							new RangeGraph<Vector3D>(new Vector3D(-0.1f, -0.2f, 0.0f),
								new Vector3D(0.2f, -0.2f, 0.0f)),
						Acceleration =
							new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.4f, 0.0f), new Vector3D(0.0f, 0.4f, 0.0f)),
						Color = new RangeGraph<Color>(Datatypes.Color.White, Datatypes.Color.TransparentWhite)
					};
					AddEmitterToSystem(new ParticleEmitter(emitterData, Vector3D.Zero));
				}
				currentEmitterInEffect = 0;
				RefreshAllEffectProperties();
				RaisePropertyChanged("AvailableEmitterIndices");
				CenterViewOnCurrentEffect();
				TemplateListVisibility = Visibility.Hidden;
				RaisePropertyChanged("TemplateListVisibility");
				SavedEmitterSelectionVisibility = Visibility.Hidden;
				RaisePropertyChanged("SavedEmitterSelectionVisibility");
			}
		}

		public ObservableCollection<string> AvailableTemplates { get; private set; }

		private void CenterViewOnCurrentEffect()
		{
			if (service.Viewport == null)
				return;
			//ncrunch: no coverage start
			service.Viewport.CenterViewOn(currentEffect.Position.GetVector2D());
			service.Viewport.ZoomViewTo(1.0f);
		}

		//ncrunch: no coverage end

		public void UpdateOnContentChange(ContentType type, string addedName)
		{
			if (type == ContentType.ParticleSystem && !EffectsInProject.Contains(addedName))
			{
				EffectsInProject.Add(addedName);
				RaisePropertyChanged("EffectsInProject");
			}
			if (type == ContentType.ParticleEmitter && !EmittersInProject.Contains(addedName))
			{
				EmittersInProject.Add(addedName);
				RaisePropertyChanged("EmittersInProject");
			}
			if (type == ContentType.Material && !MaterialList.Contains(addedName))
			{
				MaterialList.Add(addedName);
				RaisePropertyChanged("MaterialList");
			}
		}

		public void UpdateOnContentDeletion(string removedName)
		{
			if (EffectsInProject.Contains(removedName))
				EffectsInProject.Remove(removedName);
			else if (EmittersInProject.Contains(removedName))
				EmittersInProject.Remove(removedName);
			else if (MaterialList.Contains(removedName))
			{
				MaterialList.Remove(removedName);
				foreach (var emitter in currentEffect.AttachedEmitters)
					if (emitter.EmitterData.ParticleMaterial.Name.Equals(removedName))
						emitter.EmitterData.ParticleMaterial = ContentExtensions.CreateDefaultMaterial2D(); //ncrunch: no coverage
			}
			RaisePropertyChanged("EffectsInProject");
		}

		public void Reset()
		{
			UpdateDataForLoad();
			ResetDefaultEffect();
		}

		public void Activate()
		{
			foreach (var emitter in currentEffect.AttachedEmitters)
				emitter.IsActive = true;
			if (service.Viewport == null)
				return;
			//ncrunch: no coverage start
			service.Viewport.CenterViewOn(currentEffect.Position.GetVector2D());
			service.Viewport.ZoomViewTo(1.0f);
			Messenger.Default.Send("ParticleSystem", "OpenEditorPlugin");
		}

		//ncrunch: no coverage end
		public bool CanSaveParticleSystem
		{
			get { return !string.IsNullOrEmpty(ParticleSystemName); }
		}
	}
}