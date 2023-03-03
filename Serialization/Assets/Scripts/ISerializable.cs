using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializable
{
    public void SaveJSon<T>(T toSave);
    public T LoadJson<T>();
    public void SaveXML<T>(T toSave);
    public T LoadXML<T>();
    public void SaveBinary<T>(T toSave);
    public T LoadBinary<T>();
}
