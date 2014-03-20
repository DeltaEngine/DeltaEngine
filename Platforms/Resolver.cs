using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Basic resolver functionality via Autofac, each configuration registers concrete types. For
	/// example GLFW uses GLFWGraphics, GLFWSound, GLFWKeyboard, etc. and makes them available.
	/// </summary>
	public abstract class Resolver : IDisposable
	{
		protected Resolver()
		{
			assemblyLoader = new AssemblyTypeLoader(this);
			manuallyCreatedDisposableObjects = new List<IDisposable>();
		}

		private readonly AssemblyTypeLoader assemblyLoader;
		private readonly List<IDisposable> manuallyCreatedDisposableObjects;

		internal IEnumerable<Type> RegisteredTypes
		{
			get { return alreadyRegisteredTypes; }
		}

		public void Register<T>()
		{
			Register(typeof(T));
		}

		public void Register(Type typeToRegister)
		{
			if (alreadyRegisteredTypes.Contains(typeToRegister))
				return;
			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();
			RegisterNonConcreteBaseTypes(typeToRegister, RegisterType(typeToRegister));
		}

		protected readonly List<Type> alreadyRegisteredTypes = new List<Type>();

		protected internal class UnableToRegisterMoreTypesAppAlreadyStarted : Exception {}

		public void RegisterSingleton<T>()
		{
			RegisterSingleton(typeof(T));
		}

		public void RegisterSingleton(Type typeToRegister)
		{
			if (alreadyRegisteredTypes.Contains(typeToRegister))
				return;
			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();
			RegisterNonConcreteBaseTypes(typeToRegister,
				RegisterType(typeToRegister).InstancePerLifetimeScope());
		}

		private
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
			RegisterType(Type t)
		{
			AddRegisteredType(t);
			if (typeof(ContentData).IsAssignableFrom(t))
				return builder.RegisterType(t).AsSelf().
					FindConstructorsWith(publicAndNonPublicConstructorFinder).
					UsingConstructor(contentConstructorSelector);
			return builder.RegisterType(t).AsSelf();
		}

		private readonly PublicAndNonPublicConstructorFinder publicAndNonPublicConstructorFinder =
			new PublicAndNonPublicConstructorFinder();

		private class PublicAndNonPublicConstructorFinder : IConstructorFinder
		{
			public ConstructorInfo[] FindConstructors(Type targetType)
			{
				return
					targetType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.Instance);
			}
		}

		private readonly ContentConstructorSelector contentConstructorSelector =
			new ContentConstructorSelector();

		private class ContentConstructorSelector : IConstructorSelector
		{
			public ConstructorParameterBinding SelectConstructorBinding(
				ConstructorParameterBinding[] constructorBindings)
			{
				foreach (var constructor in constructorBindings)
				{
					var parameters = constructor.TargetConstructor.GetParameters();
					if (parameters.Length > 0 && parameters[0].ParameterType == typeof(string))
						return constructor;
				}
				return constructorBindings.First();
			}
		}

		private bool AddRegisteredType(Type type)
		{
			if (type == typeof(IDisposable) || type.Module.ScopeName == "CommonLanguageRuntimeLibrary")
				return false; //ncrunch: no coverage
			if (!alreadyRegisteredTypes.Contains(type))
			{
				alreadyRegisteredTypes.Add(type);
				return true;
			}
			if (ExceptionExtensions.IsDebugMode && !type.IsInterface && !type.IsAbstract)
				Console.WriteLine("Warning: Type " + type + " already exists in alreadyRegisteredTypes");
			return false;
		}

		private ContainerBuilder builder = new ContainerBuilder();

		/// <summary>
		/// Registers an already created instance and overwrite all base classes and interfaces. For
		/// example if registering WpfHostedFormsWindow will force all Resolve Window calls to use it.
		/// </summary>
		protected internal void RegisterInstance(object instance)
		{
			var registration = builder.RegisterInstance(instance).SingleInstance().AsSelf();
			var type = instance.GetType();
			AddRegisteredType(type);
			registration.AsImplementedInterfaces();
			if (type.BaseType != typeof(object))
				RegisterAllBaseTypes(type.BaseType, registration);
		}

		private void RegisterAllBaseTypes(Type baseType,
			IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registration)
		{
			while (baseType != null && baseType != typeof(object))
			{
				if (AddRegisteredType(baseType))
					registration.As(baseType);
				baseType = baseType.BaseType;
			}
		}

		private void RegisterNonConcreteBaseTypes(Type typeToRegister,
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
				registration)
		{
			var baseType = typeToRegister.BaseType;
			if (baseType == typeof(object))
			{
				foreach (var type in typeToRegister.GetInterfaces())
					AddRegisteredType(type);
				registration.AsImplementedInterfaces();
			}
			while (baseType != null && baseType != typeof(object))
			{
				if (baseType.IsAbstract)
				{
					if (AddRegisteredType(baseType))
						registration.As(baseType);
				}
				baseType = baseType.BaseType;
			}
		}

		public virtual BaseType Resolve<BaseType>() where BaseType : class
		{
			if (typeof(BaseType) == typeof(EntitiesRunner))
				return EntitiesRunner.Current as BaseType;
			if (typeof(BaseType) == typeof(GlobalTime))
				return GlobalTime.Current as BaseType; //ncrunch: no coverage, already registered in MockResolver
			if (typeof(BaseType) == typeof(ScreenSpace))
				return ScreenSpace.Current as BaseType; //ncrunch: no coverage, already registered in MockResolver
			if (typeof(BaseType) == typeof(Randomizer))
				return Randomizer.Current as BaseType;
			MakeSureContainerIsInitialized();
			return (BaseType)container.Resolve(typeof(BaseType));
		}

		private void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return; //ncrunch: no coverage
			assemblyLoader.RegisterAllTypesFromAllAssemblies<ContentData, UpdateBehavior, DrawBehavior>();
			container = builder.Build();
		}

		protected bool IsAlreadyInitialized
		{
			get { return container != null; }
		}
		private IContainer container;

		internal void RegisterAllTypesInAssembly(Type[] assemblyTypes)
		{
			foreach (Type type in assemblyTypes)
				if (AssemblyTypeLoader.IsTypeResolveable(type) && !alreadyRegisteredTypes.Contains(type))
				{
					builder.RegisterType(type).AsSelf().InstancePerLifetimeScope();
					AddRegisteredType(type);
				}
		}

		internal virtual object Resolve(Type baseType, object customParameter = null)
		{
			MakeSureContainerIsInitialized();
			return ResolveAndShowErrorBoxIfNoDebuggerIsAttached(baseType, customParameter);
		}

		private object ResolveAndShowErrorBoxIfNoDebuggerIsAttached(Type baseType, object parameter)
		{
			try
			{
				return TryResolve(baseType, parameter);
			}
			catch (Exception ex)
			{
				Exception innerException = ex.InnerException ?? ex;
				if (innerException.GetType().FullName.Contains("Shader"))
					throw innerException;
				Logger.Error(ex);
				if (Debugger.IsAttached || baseType == typeof(Window) ||
					StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
					throw;
				//ncrunch: no coverage start
				ShowInitializationErrorBox(baseType, innerException);
				return null;
				//ncrunch: no coverage end
			}
		}

		private object TryResolve(Type baseType, object parameter)
		{
			if (parameter == null)
				return container.Resolve(baseType);
			if (parameter is ContentCreationData)
			{
				var resolvedInstance = CreateTypeManually(FindConcreteType(baseType), parameter);
				if (resolvedInstance is IDisposable)
					manuallyCreatedDisposableObjects.Add((IDisposable)resolvedInstance);
				if (resolvedInstance != null)
					return resolvedInstance; //ncrunch: no coverage
			}
			return container.Resolve(baseType,
				new Parameter[] { new TypedParameter(parameter.GetType(), parameter) });
		}

		private object CreateTypeManually(Type concreteTypeToCreate, object parameter)
		{
			const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var constructors = concreteTypeToCreate.GetConstructors(Flags);
			//ncrunch: no coverage start
			foreach (var constructor in constructors)
			{
				var parameterInfos = constructor.GetParameters();
				if (parameterInfos.Length <= 0 || !parameterInfos[0].ParameterType.IsInstanceOfType(parameter))
					continue; //ncrunch: no coverage
				var parameters = new object[parameterInfos.Length];
				parameters[0] = parameter;
				for (int num = 1; num < parameters.Length; num++)
					parameters[num] = Resolve(parameterInfos[num].ParameterType);
				return Activator.CreateInstance(concreteTypeToCreate, Flags, default(Binder), parameters,
					default(CultureInfo));
			} //ncrunch: no coverage end
			return null;
		}

		private Type FindConcreteType(Type baseType)
		{
			return alreadyRegisteredTypes.FirstOrDefault(
				type => !type.IsAbstract && baseType.IsAssignableFrom(type));
		}

		/// <summary>
		/// When resolving loading the first ContentData instance we need to make sure all Content is
		/// available and can be loaded. Otherwise we have to wait to avoid content crashes.
		/// </summary>
		internal abstract void MakeSureContentManagerIsReady();

		//ncrunch: no coverage start
		private void ShowInitializationErrorBox(Type baseType, Exception ex)
		{
			var exceptionText = StackTraceExtensions.FormatExceptionIntoClickableMultilineText(ex);
			var window = Resolve<Window>();
			window.CopyTextToClipboard(exceptionText);
			const string Header = "Fatal Initialization Error";
			var text = GetHintTextForKnownIssues(ex);
			text += "Unable to resolve class " + baseType + "\n";
			if (ExceptionExtensions.IsDebugMode)
				text += exceptionText;
			else
				text += ex.GetType().Name + " " + ex.Message;
			text += ErrorWasCopiedToClipboardMessage + ClickIgnoreToContinue;
			if (window.ShowMessageBox(Header, text, new[] { "Ignore", "Abort" }) != "Abort")
				return;
			Dispose();
			if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				Environment.Exit((int)AppRunner.ExitCode.InitializationError);
		}

		private static string GetHintTextForKnownIssues(Exception ex)
		{
			if (ex.ToString().Contains("OpenGLVersionDoesNotSupportShaders"))
			{
				string hintText = "Please verify that your video card supports OpenGL 3.0 or higher and" +
					" your driver is up to date.\n\n";
				hintText += "Exception details:\n";
				return hintText;
			}
			const string DirectX9NotSupportedFromSlimDX = "D3DERR_INVALIDCALL: Invalid call";
			if (ex.ToString().Contains(DirectX9NotSupportedFromSlimDX))
			{
				string hintText = "Please verify that your video card supports DirectX 9.0c or higher," +
					" your driver is up to date and you have installed the latest DirectX runtime.\n\n";
				hintText += "Exception details:\n";
				return hintText;
			}
			const string DirectX11NotSupportedFromSharpDX = "DXGI_ERROR_UNSUPPORTED/Unsupported";
			if (ex.ToString().Contains(DirectX11NotSupportedFromSharpDX))
			{
				string hintText = "Please verify that your video card supports DirectX 11," +
					" your driver is up to date and you have installed the latest DirectX runtime.\n\n";
				hintText += "Exception details:\n";
				return hintText;
			}
			return "";
		} //ncrunch: no coverage end

		public const string ErrorWasCopiedToClipboardMessage =
			"\n\nMessage was logged and copied to the clipboard.";
		protected const string ClickIgnoreToContinue = " Click Ignore to try to continue.";

		public virtual void Dispose()
		{
			if (!IsAlreadyInitialized)
				return;
			DisposeContainerOnlyOnce();
			foreach (IDisposable disposableObject in manuallyCreatedDisposableObjects)
				disposableObject.Dispose();
			manuallyCreatedDisposableObjects.Clear();
		}

		private void DisposeContainerOnlyOnce()
		{
			var remContainerToDispose = container;
			container = null;
			remContainerToDispose.Dispose();
			builder = null;
		}
	}
}