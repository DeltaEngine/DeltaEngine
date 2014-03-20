using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DeltaEngine.Content;
using DeltaEngine.Entities;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Easily save data objects with the full type names like other Serializers, but much faster.
	/// </summary>
	internal static class BinaryDataSaver
	{
		internal static void SaveDataType(object data, Type type, BinaryWriter writer)
		{
			topLevelTypeToSave = type;
			SaveData(data, type, writer);
		}

		private static Type topLevelTypeToSave;

		private static void SaveData(object data, Type type, BinaryWriter writer)
		{
			if (type.NeedToSaveDataLength())
				SaveDataWithHeader(data, type, writer);
			else
				SaveDataBody(data, type, writer);
		}

		private static void SaveDataWithHeader(object data, Type type, BinaryWriter writer)
		{
			using (var memoryStream = new MemoryStream())
			using (var memoryWriter = new BinaryWriter(memoryStream))
			{
				SaveDataBody(data, type, memoryWriter);
				writer.Write((int)memoryStream.Length);
				memoryStream.Seek(0, SeekOrigin.Begin);
				memoryStream.CopyTo(writer.BaseStream);
			}
		}

		private static void SaveDataBody(object data, Type type, BinaryWriter writer)
		{
			try
			{
				TrySaveData(data, type, writer);
			}
			catch (Exception ex)
			{
				throw new UnableToSave(data, ex);
			}
		}

		private static void TrySaveData(object data, Type type, BinaryWriter writer)
		{
			if (data == null)
				throw new NullReferenceException();
			if (data is ContentData)
			{
				var justSaveContentName = topLevelTypeToSave != type &&
																	!(data as ContentData).Name.StartsWith("<Generated");
				writer.Write(justSaveContentName);
				if (justSaveContentName)
				{
					writer.Write((data as ContentData).Name);
					return;
				}
			}
			if (data is Entity)
			{
				SaveEntity(data as Entity, writer);
				return;
			}
			if (type.Name.StartsWith("Xml"))
				throw new DoNotSaveXmlDataAsBinaryData(data);
			if (type.Name.StartsWith("Mock"))
				throw new DoNotSaveMockTypes(data);
			if (type.Name.EndsWith("Image"))
				throw new DoNotSaveImagesAsBinaryData(data);
			if (type.Name.EndsWith("Sound") || type.Name.EndsWith("Music") || type.Name.EndsWith("Video"))
				throw new DoNotSaveMultimediaDataAsBinaryData(data);
			if (data is Stream && !(data is MemoryStream))
				throw new OnlyMemoryStreamSavingIsSupported(data);
			if (SaveIfIsPrimitiveData(data, type, writer))
				return;
			if (type == typeof(Material))
				SaveCustomMaterial(data, writer);
			else if (type == typeof(MemoryStream))
				SaveMemoryStream(data, writer);
			else if (type == typeof(byte[]))
				SaveByteArray(data, writer);
			else if (type == typeof(char[]))
				SaveCharArray(data, writer);
			else if (data is IList || type.IsArray)
				SaveArray(data as IList, writer);
			else if (data is IDictionary || type == typeof(Dictionary<,>))
				SaveDictionary(data as IDictionary, writer);
			else if (type.IsClass || type.IsValueType)
				SaveClassData(data, type, writer);
		}

		internal class UnableToSave : Exception
		{
			public UnableToSave(object data, Exception exception)
				: base(data.ToString(), exception) {}
		}

		private static void SaveEntity(Entity entity, BinaryWriter writer)
		{
			SaveArray(entity.GetComponentsForSaving(), writer);
			SaveArray(entity.GetTags(), writer);
			SaveEntityBehaviors(entity, writer);
			if (entity is DrawableEntity)
				SaveDrawableEntityDrawBehaviors(entity as DrawableEntity, writer);
		}

		private static void SaveEntityBehaviors(Entity entity, BinaryWriter writer)
		{
			List<UpdateBehavior> behaviorTypes = entity.GetActiveBehaviors();
			var behaviorNames = new List<string>();
			foreach (UpdateBehavior behaviorType in behaviorTypes)
				behaviorNames.Add(behaviorType.GetShortNameOrFullNameIfNotFound());
			SaveArray(behaviorNames, writer);
		}

		private static void SaveDrawableEntityDrawBehaviors(DrawableEntity drawable,
			BinaryWriter writer)
		{
			List<DrawBehavior> drawBehaviorTypes = drawable.GetDrawBehaviors();
			var drawBehaviorNames = new List<string>();
			foreach (DrawBehavior behaviorType in drawBehaviorTypes)
				drawBehaviorNames.Add(behaviorType.GetShortNameOrFullNameIfNotFound());
			SaveArray(drawBehaviorNames, writer);
		}

		private class DoNotSaveXmlDataAsBinaryData : Exception
		{
			public DoNotSaveXmlDataAsBinaryData(object data)
				: base(data.ToString()) {}
		}

		private class DoNotSaveMockTypes : Exception
		{
			public DoNotSaveMockTypes(object data)
				: base(data.ToString()) { }
		}

		private class DoNotSaveImagesAsBinaryData : Exception
		{
			public DoNotSaveImagesAsBinaryData(object data)
				: base(data.ToString()) { }
		}

		private class DoNotSaveMultimediaDataAsBinaryData : Exception
		{
			public DoNotSaveMultimediaDataAsBinaryData(object data)
				: base(data.ToString()) { }
		}

		private class OnlyMemoryStreamSavingIsSupported : Exception
		{
			public OnlyMemoryStreamSavingIsSupported(object data)
				: base(data.ToString()) {}
		}

		private static bool SaveIfIsPrimitiveData(object data, Type type, BinaryWriter writer)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				writer.Write((bool)data);
				return true;
			case TypeCode.Byte:
				writer.Write((byte)data);
				return true;
			case TypeCode.Char:
				writer.Write((char)data);
				return true;
			case TypeCode.Decimal:
				writer.Write((decimal)data);
				return true;
			case TypeCode.Double:
				writer.Write((double)data);
				return true;
			case TypeCode.Single:
				writer.Write((float)data);
				return true;
			case TypeCode.Int16:
				writer.Write((short)data);
				return true;
			case TypeCode.Int32:
				writer.Write((int)data);
				return true;
			case TypeCode.Int64:
				writer.Write((long)data);
				return true;
			case TypeCode.String:
				writer.Write((string)data);
				return true;
			case TypeCode.SByte:
				writer.Write((sbyte)data);
				return true;
			case TypeCode.UInt16:
				writer.Write((ushort)data);
				return true;
			case TypeCode.UInt32:
				writer.Write((uint)data);
				return true;
			case TypeCode.UInt64:
				writer.Write((ulong)data);
				return true;
			}
			return false;
		}

		private static void SaveCustomMaterial(object data, BinaryWriter writer)
		{
			var material = data as Material;
			writer.Write((int)material.Shader.Flags);
			var isCustomImage = material.DiffuseMap == null || material.DiffuseMap.Name.StartsWith("<");
			writer.Write((byte)(material.DiffuseMap == null ? 2 : isCustomImage ? 1 : 0));
			if (isCustomImage)
			{
				writer.Write(material.pixelSize.Width);
				writer.Write(material.pixelSize.Height);
			}
			else if (material.Animation != null)
				writer.Write(material.Animation.Name);
			else if (material.SpriteSheet != null)
				writer.Write(material.SpriteSheet.Name);
			else
				writer.Write(material.DiffuseMap != null ? material.DiffuseMap.Name : "");
			writer.Write(material.DefaultColor.PackedRgba);
			writer.Write(material.Duration);
		}

		private static void SaveMemoryStream(object data, BinaryWriter writer)
		{
			var stream = data as MemoryStream;
			writer.WriteNumberMostlyBelow255((int)stream.Length);
			writer.Write(stream.ToArray());
		}

		private static void SaveByteArray(object data, BinaryWriter writer)
		{
			writer.WriteNumberMostlyBelow255(((byte[])data).Length);
			writer.Write((byte[])data);
		}

		private static void SaveCharArray(object data, BinaryWriter writer)
		{
			writer.WriteNumberMostlyBelow255(((char[])data).Length);
			writer.Write((char[])data);
		}

		private static void SaveArray(IList list, BinaryWriter writer)
		{
			writer.WriteNumberMostlyBelow255(list.Count);
			if (list.Count == 0)
				return;
			if (AreAllElementsTheSameType(list))
				SaveArrayWhenAllElementsAreTheSameType(list, writer);
			else
				SaveArrayWhenAllElementsAreNotTheSameType(list, writer);
		}

		internal static void WriteNumberMostlyBelow255(this BinaryWriter writer, int number)
		{
			if (number < 255)
				writer.Write((byte)number);
			else
			{
				writer.Write((byte)255);
				writer.Write(number);
			}
		}

		private static bool AreAllElementsTheSameType(IList list)
		{
			var firstType = BinaryDataExtensions.GetTypeOrObjectType(list[0]);
			foreach (object element in list)
				if (BinaryDataExtensions.GetTypeOrObjectType(element) != firstType)
					return false;
			return true;
		}

		private static void SaveArrayWhenAllElementsAreTheSameType(IList list, BinaryWriter writer)
		{
			var arrayType = list[0] != null && list[0].GetType().Name == "RuntimeType"
				? ArrayElementType.AllTypesAreTypes : ArrayElementType.AllTypesAreTheSame;
			var firstElementType = BinaryDataExtensions.GetTypeOrObjectType(list[0]);
			if (arrayType == ArrayElementType.AllTypesAreTheSame && firstElementType == typeof(object) &&
				list[0] == null)
				arrayType = ArrayElementType.AllTypesAreNull;
			writer.Write((byte)arrayType);
			if (arrayType == ArrayElementType.AllTypesAreNull)
				return;
			if (arrayType == ArrayElementType.AllTypesAreTheSame)
				writer.Write(list[0].GetShortNameOrFullNameIfNotFound());
			foreach (object value in list)
				SaveData(value, firstElementType, writer);
		}

		private static void SaveArrayWhenAllElementsAreNotTheSameType(IEnumerable list,
			BinaryWriter writer)
		{
			writer.Write((byte)ArrayElementType.AllTypesAreDifferent);
			foreach (object value in list)
				SaveElementWithItsType(writer, value);
		}

		public enum ArrayElementType : byte
		{
			AllTypesAreDifferent,
			AllTypesAreTypes,
			AllTypesAreTheSame,
			AllTypesAreNull
		}

		private static void SaveElementWithItsType(BinaryWriter writer, object value)
		{
			writer.Write(value.GetShortNameOrFullNameIfNotFound());
			writer.Write(value != null);
			if (value != null)
				SaveData(value, BinaryDataExtensions.GetTypeOrObjectType(value), writer);
		}

		private static void SaveDictionary(IDictionary data, BinaryWriter writer)
		{
			writer.WriteNumberMostlyBelow255(data.Count);
			if (data.Count == 0)
				return;
			if (AreAllDictionaryValuesTheSameType(data))
				SaveDictionaryWhenAllValuesAreTheSameType(data, writer);
			else
				SaveDictionaryWhenAllValuesAreNotTheSameType(data, writer);
		}

		private static bool AreAllDictionaryValuesTheSameType(IDictionary data)
		{
			Type firstType = null;
			foreach (object element in data.Values)
				if (firstType == null)
					firstType = BinaryDataExtensions.GetTypeOrObjectType(element);
				else if (BinaryDataExtensions.GetTypeOrObjectType(element) != firstType)
					return false;
			return true;
		}

		private static void SaveDictionaryWhenAllValuesAreTheSameType(IDictionary data,
			BinaryWriter writer)
		{
			writer.Write((byte)ArrayElementType.AllTypesAreTheSame);
			Type keyType = null;
			Type valueType = null;
			var pair = data.GetEnumerator();
			while (pair.MoveNext())
			{
				if (keyType == null)
				{
					keyType = BinaryDataExtensions.GetTypeOrObjectType(pair.Key);
					valueType = BinaryDataExtensions.GetTypeOrObjectType(pair.Value);
					writer.Write(pair.Key.GetShortNameOrFullNameIfNotFound());
					writer.Write(pair.Value.GetShortNameOrFullNameIfNotFound());
				}
				SaveData(pair.Key, keyType, writer);
				SaveData(pair.Value, valueType, writer);
			}
		}

		private static void SaveDictionaryWhenAllValuesAreNotTheSameType(IDictionary data,
			BinaryWriter writer)
		{
			writer.Write((byte)ArrayElementType.AllTypesAreDifferent);
			Type keyType = null;
			var pair = data.GetEnumerator();
			while (pair.MoveNext())
			{
				if (keyType == null)
				{
					keyType = BinaryDataExtensions.GetTypeOrObjectType(pair.Key);
					writer.Write(pair.Key.GetShortNameOrFullNameIfNotFound());
				}
				writer.Write(pair.Value.GetShortNameOrFullNameIfNotFound());
				SaveData(pair.Key, keyType, writer);
				SaveData(pair.Value, BinaryDataExtensions.GetTypeOrObjectType(pair.Value), writer);
			}
		}

		private static void SaveClassData(object data, Type type, BinaryWriter writer)
		{
			foreach (FieldInfo field in type.GetBackingFields())
			{
				object fieldData = field.GetValue(data);
				Type fieldType = field.FieldType;
				if (fieldType.DoNotNeedToSaveType(field.Attributes) || fieldType == type)
					continue;
				if (fieldType.IsClass)
				{
					writer.Write(fieldData != null);
					if (fieldData == null)
						continue;
					fieldType = fieldData.GetType();
					if (fieldType.NeedToSaveTypeName() ||
						(field.FieldType == typeof(object) && fieldType == typeof(string)))
						writer.Write(fieldData.GetShortNameOrFullNameIfNotFound());
				}
				SaveData(fieldData, fieldType, writer);
			}
		}
	}
}