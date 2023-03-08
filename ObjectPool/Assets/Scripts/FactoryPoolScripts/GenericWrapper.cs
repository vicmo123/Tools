using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericWrapper<EnumType, ObjType, ObjData> where ObjType : MonoBehaviour, IPoolable<EnumType, ObjData>, IFactoryInitializable<ObjData>
{
    #region singleton
    private static readonly System.Lazy<GenericWrapper<EnumType, ObjType, ObjData>> instance = new System.Lazy<GenericWrapper<EnumType, ObjType, ObjData>>(() => new GenericWrapper<EnumType, ObjType, ObjData>());
    public static GenericWrapper<EnumType, ObjType, ObjData> Instance => instance.Value;
    private GenericWrapper() { }
    #endregion

    public FactoryPool.GenericPool<EnumType, ObjType, ObjData> objectPool;
    public FactoryPool.GenericManager<EnumType, ObjType, ObjData> manager;
    public FactoryPool.GenericFactory<EnumType, ObjType, ObjData> objectFactory;

    public void PreInitialize()
    {
        objectPool = new FactoryPool.GenericPool<EnumType, ObjType, ObjData>();
        manager = new FactoryPool.GenericManager<EnumType, ObjType, ObjData>();
        objectFactory = new FactoryPool.GenericFactory<EnumType, ObjType, ObjData>();
    }

    public void Initialize()
    {
        objectPool.Initialize();
        objectFactory.Initialize();
        manager.Initialize();
    }

    public void Refresh()
    {
        manager.Refresh();
    }

    public void PhysicsRefresh()
    {
        manager.FixedRefresh();
    }
}
