using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DeltaEngine.Content;
using DeltaEngine.Extensions;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Allows to easily save and recreate binary data objects with the full type names like other
	/// Serializers, but way faster (100x). Before reconstructing types load all needed assemblies.
	/// </summary>
	public static class BinaryDataExtensions
	{
		//ncrunch: no coverage start (faster to not profile this code)
		static BinaryDataExtensions()
		{
			AddPrimitiveTypes();
			AppDomain.CurrentDomain.AssemblyLoad += (o, args) =>
			{
				if (ShouldLoadTypes(args.LoadedAssembly))
					AddAssemblyTypes(args.LoadedAssembly);
			};
			RegisterAvailableBinaryDataImplementation();
		}

		private static void RegisterAvailableBinaryDataImplementation()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
				if (ShouldLoadTypes(assembly))
					AddAssemblyTypes(assembly);
		}

		private static void AddPrimitiveTypes()
		{
			AddType(typeof(object));
			AddType(typeof(bool));
			AddType(typeof(byte));
			AddType(typeof(char));
			AddType(typeof(decimal));
			AddType(typeof(double));
			AddType(typeof(float));
			AddType(typeof(short));
			AddType(typeof(int));
			AddType(typeof(long));
			AddType(typeof(string));
			AddType(typeof(sbyte));
			AddType(typeof(ushort));
			AddType(typeof(uint));
			AddType(typeof(ulong));
		}

		private static bool ShouldLoadTypes(Assembly assembly)
		{
			var name = assembly.GetName().Name;
			return name == "DeltaEngine" || name.EndsWith("Messages") ||
				!name.StartsWith("nunit") && !name.EndsWith(".Xml") && assembly.IsAllowed() &&
				!AssemblyExtensions.IsPlatformAssembly(name) && !AssemblyExtensions.IsEditorAssembly(name);
		}

		private static void AddAssemblyTypes(Assembly assembly)
		{
			try
			{
				TryAddAssemblyTypes(assembly);
			}
			catch (ReflectionTypeLoadException ex)
			{
				foreach (var failedLoader in ex.LoaderExceptions)
					Logger.Error(failedLoader);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		private static void TryAddAssemblyTypes(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
				if (IsValidBinaryDataType(type))
					AddType(type);
		}

		private static bool IsValidBinaryDataType(Type type)
		{
			return !type.IsAbstract && !typeof(Exception).IsAssignableFrom(type) &&
				!type.Name.StartsWith("<") && !type.Name.StartsWith("__") && !type.Name.EndsWith(".Tests");
		}

		private static void AddType(Type type)
		{
			string shortName = type.Name;
			if (TypeMap.ContainsKey(shortName))
			{
				shortName = type.FullName;
				if (TypeMap.ContainsKey(shortName))
					return;
			}
			ShortNames.Add(type, shortName);
			TypeMap.Add(shortName, type);
			if (!type.IsGenericType)
				return;
			GenericTypeMap.Add(shortName.Replace("`1", ""), type);
			GenericShortNames.Add(type, shortName.Replace("`1", ""));
		}

		private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();
		private static readonly Dictionary<Type, string> ShortNames = new Dictionary<Type, string>();
		private static readonly Dictionary<string, Type> GenericTypeMap =
			new Dictionary<string, Type>();
		private static readonly Dictionary<Type, string> GenericShortNames =
			new Dictionary<Type, string>();
		//ncrunch: no coverage end

		internal static string GetShortName(object data)
		{
			var type = data.GetType();
			if (ShortNames.ContainsKey(type))
				return ShortNames[type];
			if (IsGenericType(type))
				return CreateGenericType(type);
			throw new NoShortNameStoredFor(data);
		}

		private static bool IsGenericType(Type type)
		{
			return type.IsGenericType && GenericShortNames.ContainsKey(type.GetGenericTypeDefinition());
		}

		private static string CreateGenericType(Type type)
		{
			return GenericShortNames[type.GetGenericTypeDefinition()] + GenericTypeSeparator +
				ShortNames[type.GetGenericArguments()[0]];
		}

		private const char GenericTypeSeparator = '|';

		internal class NoShortNameStoredFor : Exception
		{
			public NoShortNameStoredFor(object data)
				: base(data.ToString()) {}
		}

		internal static string GetShortNameOrFullNameIfNotFound(this object data)
		{
			var type = GetTypeOrObjectType(data);
			if (ShortNames.ContainsKey(type))
				return ShortNames[type];
			if (IsGenericType(type))
				return CreateGenericType(type); // ncrunch: no coverage
			if (data is ContentData && ShortNames.ContainsKey(type.BaseType))
				return ShortNames[type.BaseType]; // ncrunch: no coverage
			return type.AssemblyQualifiedName;
		}

		internal static Type GetTypeOrObjectType(Object element)
		{
			return element == null ? typeof(object) : element.GetType();
		}

		public static Type GetTypeFromShortNameOrFullNameIfNotFound(string typeName)
		{
			return TypeMap.ContainsKey(typeName)
				? TypeMap[typeName] : GetGenericTypeFromShortNameOrFullName(typeName);
		}

		private static Type GetGenericTypeFromShortNameOrFullName(string typeName)
		{
			if (typeName.Contains(GenericTypeSeparator))
			{
				var typeParts = typeName.Split(GenericTypeSeparator);
				if (typeParts.Length == 2 && GenericTypeMap.ContainsKey(typeParts[0]) &&
					TypeMap.ContainsKey(typeParts[1]))
					return GenericTypeMap[typeParts[0]].MakeGenericType(TypeMap[typeParts[1]]);
			} // ncrunch: no coverage
			return GetTypeFromFullName(typeName);
		}

		private static Type GetTypeFromFullName(string typeName)
		{
			if (IsTypeNameWithFullAssemblyInformation(typeName))
				return Type.GetType(typeName, true);
			string requestedTypeNamespace = Path.GetFileNameWithoutExtension(typeName);
			if (IsDeltaEngineBaseAssemblyFullNameType(requestedTypeNamespace))
				return Type.GetType(typeName, true);
			string requestedParentNamespace = Path.GetFileNameWithoutExtension(requestedTypeNamespace);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var currentAssembly = Assembly.GetExecutingAssembly();
			foreach (Assembly assembly in assemblies)
				if (assembly != currentAssembly &&
					(assembly.GetName().Name == requestedTypeNamespace ||
					assembly.GetName().Name == requestedParentNamespace))
					return assembly.GetType(typeName, true);
			throw new TypeLoadException("Unable to find type: " + typeName);
		}

		private static bool IsTypeNameWithFullAssemblyInformation(string typeName)
		{
			return typeName.Contains(",");
		}

		private static bool IsDeltaEngineBaseAssemblyFullNameType(string assemblyName)
		{
			return assemblyName == "DeltaEngine.Core" || assemblyName == "DeltaEngine.Content" ||
				assemblyName == "DeltaEngine.Datatypes" || assemblyName == "DeltaEngine.Entities" ||
				assemblyName == "DeltaEngine.Commands";
		}

		/// <summary>
		/// Saves any object type information and the actual data contained in in, use Create to load.
		/// </summary>
		public static void Save(object data, BinaryWriter writer)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			writer.Write(GetShortName(data));
			WriteDataVersionNumber(data, writer);
			BinaryDataSaver.SaveDataType(data, data.GetType(), writer);
		}

		private static void WriteDataVersionNumber(object data, BinaryWriter writer)
		{
			var dataVersion = data.GetType().Assembly.GetName().Version;
			writer.Write((byte)dataVersion.Major);
			writer.Write((byte)dataVersion.Minor);
			writer.Write((byte)dataVersion.Build);
			writer.Write((byte)dataVersion.Revision);
		}

		/// <summary>
		/// Loads a binary data object and reconstructs the object based on the saved type information.
		/// </summary>
		public static object Create(this BinaryReader reader)
		{
			if (reader.BaseStream.Position + 6 > reader.BaseStream.Length)
				throw new NotEnoughDataLeftInStream(reader.BaseStream.Length);
			string shortName = reader.ReadString();
			var dataVersion = ReadDataVersionNumber(reader);
			var type = GetTypeFromShortNameOrFullNameIfNotFound(shortName);
			return BinaryDataLoader.CreateAndLoad(type, reader, dataVersion);
		}

		public class NotEnoughDataLeftInStream : Exception
		{
			public NotEnoughDataLeftInStream(long length)
				: base("Length=" + length) {}
		}

		private static Version ReadDataVersionNumber(BinaryReader r)
		{
			return new Version(r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte());
		}

		public static MemoryStream SaveToMemoryStream(object binaryData)
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			Save(binaryData, writer);
			return data;
		}

		public static object CreateFromMemoryStream(this MemoryStream data)
		{
			data.Seek(0, SeekOrigin.Begin);
			return Create(new BinaryReader(data));
		}

		public static byte[] ToByteArrayWithLengthHeader(object message)
		{
			byte[] data = ToByteArrayWithTypeInformation(message);
			using (var total = new MemoryStream())
			using (var writer = new BinaryWriter(total))
			{
				writer.WriteNumberMostlyBelow255(data.Length);
				writer.Write(data);
				return total.ToArray();
			}
		}

		public static byte[] ToByteArrayWithTypeInformation(object data)
		{
			using (var messageStream = new MemoryStream())
			using (var messageWriter = new BinaryWriter(messageStream))
			{
				Save(data, messageWriter);
				return messageStream.ToArray();
			}
		}

		public static byte[] ToByteArray(object data)
		{
			using (var messageStream = new MemoryStream())
			using (var messageWriter = new BinaryWriter(messageStream))
			{
				if (data is IList)
					foreach (object value in data as IList)
						BinaryDataSaver.SaveDataType(value, value.GetType(), messageWriter);
				else
					BinaryDataSaver.SaveDataType(data, data.GetType(), messageWriter);
				return messageStream.ToArray();
			}
		}

		public static object ToBinaryData(this byte[] data)
		{
			using (var messageStream = new MemoryStream(data))
			using (var messageReader = new BinaryReader(messageStream))
				return Create(messageReader);
		}

		public static MemoryStream SaveDataIntoMemoryStream<DataType>(DataType input)
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			BinaryDataSaver.SaveDataType(input, typeof(DataType), writer);
			return data;
		}

		public static DataType LoadKnownTypeWithoutVersionCheck<DataType>(this BinaryReader reader)
		{
			return (DataType)BinaryDataLoader.CreateAndLoad(typeof(DataType), reader, null);
		}

		public static DataType LoadDataWithKnownTypeFromMemoryStream<DataType>(MemoryStream data)
		{
			data.Seek(0, SeekOrigin.Begin);
			var reader = new BinaryReader(data);
			var version = typeof(DataType).Assembly.GetName().Version;
			return (DataType)BinaryDataLoader.CreateAndLoad(typeof(DataType), reader, version);
		}

		internal static bool DoNotNeedToSaveType(this Type fieldType, FieldAttributes fieldAttributes)
		{
			return fieldAttributes.HasFlag(FieldAttributes.NotSerialized) ||
				fieldType == typeof(Action) || fieldType == typeof(Action<>) ||
				fieldType.BaseType == typeof(MulticastDelegate) || fieldType == typeof(BinaryWriter) ||
				fieldType == typeof(BinaryReader) || fieldType == typeof(Pointer) ||
				fieldType == typeof(IntPtr) || fieldType == typeof(ISerializable);
		}

		internal static bool NeedToSaveTypeName(this Type fieldType)
		{
			return fieldType.AssemblyQualifiedName != null && fieldType != typeof(string) &&
				!fieldType.IsArray && !typeof(IList).IsAssignableFrom(fieldType) &&
				!typeof(IDictionary).IsAssignableFrom(fieldType) && fieldType != typeof(MemoryStream);
		}

		internal static bool NeedToSaveDataLength(this Type classType)
		{
			return classType.GetCustomAttributes(typeof(SaveSafely), true).Length > 0;
		}
	}
}