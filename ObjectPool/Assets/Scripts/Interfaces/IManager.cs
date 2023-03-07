using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager<EnumType, ObjType, ObjData>
{
    void Initialize();
    void Refresh();
    void FixedRefresh();
    void AddObjectToCollection(EnumType enumType, ObjData objData);
    void RemoveObjectFromCollection(EnumType enumType, ObjType type);
}
