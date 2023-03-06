using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

public class Serializer : ISerializable
{ 
    public enum SerializationType
    {
        JSON,
        XML,
        BINARY
    }

    #region singleton
    private static readonly System.Lazy<Serializer> instance = new System.Lazy<Serializer>(() => new Serializer());
    public static Serializer Instance => instance.Value;
    private Serializer() { }
    #endregion

    public void Save<T>(SerializationType type, T toSave, string filePath)
    {
        switch (type)
        {
            case SerializationType.JSON:
                SaveJSon(toSave, filePath);
                break;
            case SerializationType.XML:
                SaveXML(toSave, filePath);
                break;
            case SerializationType.BINARY:
                SaveBinary(toSave, filePath);
                break;
            default:
                Debug.LogError("Unhandeled switch case : " + type);
                break;
        }
    }

    public T Load<T>(SerializationType type, string filePath)
    {
        T toRet = default(T);

        switch (type)
        {
            case SerializationType.JSON:
                toRet = LoadJson<T>(filePath);
                break;
            case SerializationType.XML:
                toRet = LoadXML<T>(filePath);
                break;
            case SerializationType.BINARY:
                toRet = LoadBinary<T>(filePath);
                break;
            default:
                Debug.LogError("Unhandeled switch case : " + type);
                break;
        }

        return toRet;
    }

    public void SaveJSon<T>(T toSave, string filePath)
    {
        CheckInputSave(toSave, filePath);

        string jsonSerializationOfNewClass = JsonUtility.ToJson(toSave);
        File.WriteAllText(filePath, jsonSerializationOfNewClass);
    }

    public T LoadJson<T>(string filePath)
    {
        CheckInputLoad(filePath);

        try
        {
            string jsonDeserialized = File.ReadAllText(filePath);
            T toRet = JsonUtility.FromJson<T>(jsonDeserialized);
            return toRet == null ? default(T) : toRet;
        }
        catch (System.Exception ex)
        {
            throw new Newtonsoft.Json.JsonException($"Failed to deserialize JSON from file '{filePath}'.", ex);
        }
    }

    public void SaveXML<T>(T toSave, string filePath)
    {
        CheckInputSave(toSave, filePath);

        var xmlSerializer = new XmlSerializer(typeof(T));
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            xmlSerializer.Serialize(stream, toSave);
        }
    }

    public T LoadXML<T>(string filePath)
    {
        CheckInputLoad(filePath);

        try
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream) ?? default(T);
            }
        }
        catch (System.Exception ex)
        {
            throw new XmlException($"Failed to deserialize XML from file '{filePath}'.", ex);
        }
    }

    public void SaveBinary<T>(T toSave, string filePath)
    {
        CheckInputSave(toSave, filePath);

        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Create(filePath))
        {
            bf.Serialize(file, toSave);
        }
    }

    public T LoadBinary<T>(string filePath)
    {
        CheckInputLoad(filePath);

        T toRet = default(T);
        try
        {
            if (File.Exists(filePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream file = File.Open(filePath, FileMode.Open))
                {
                    toRet = (T)bf.Deserialize(file);
                }
            }
        }
        catch (System.Exception ex)
        {
            throw new System.InvalidCastException($"The deserialized object could not be cast to type {typeof(T).FullName}.");
        }
        return toRet;
    }

    public static void CheckInputSave<T>(T toSave, string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new System.ArgumentNullException(nameof(filePath));
        if (toSave == null)
            throw new System.ArgumentNullException(nameof(toSave));
    }

    public static void CheckInputLoad(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new System.ArgumentNullException(nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException("The specified file could not be found.", filePath);
    }
}
