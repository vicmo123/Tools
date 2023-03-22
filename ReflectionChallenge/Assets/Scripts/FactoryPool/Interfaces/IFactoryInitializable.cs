using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFactoryInitializable<ObjData>
{
    void Initialize(ObjData data);
}
