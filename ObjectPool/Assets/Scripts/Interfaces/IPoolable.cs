using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable<EnumType, ObjData>
{
    bool IsActive { get; set; }
    void Activate();
    void Deactivate();
    ObjData GetObjData();
    EnumType GetEnumType();
}
