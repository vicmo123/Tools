using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializable
{
    public void SaveJSon<T>(T toSave, string filePath);
    public T LoadJson<T>(string filePath);
    public void SaveXML<T>(T toSave, string filePath);
    public T LoadXML<T>(string filePath);
    public void SaveBinary<T>(T toSave, string filePath);
    public T LoadBinary<T>(string filePath);
}
