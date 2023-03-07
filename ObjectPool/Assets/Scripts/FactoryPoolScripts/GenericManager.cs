using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FactoryPool
{
    public class GenericManager<EnumType, ObjType, ObjData> : IManager<EnumType, ObjType, ObjData> where ObjType : MonoBehaviour, IFactoryInitializable<ObjData>, IPoolable
    {
        public GenericWrapper<EnumType, ObjType, ObjData> wrapper = GenericWrapper<EnumType, ObjType, ObjData>.Instance;
        public HashSet<ObjType> Collection = new HashSet<ObjType>();

        public Action Init = () => { };
        public Action Update = () => { };
        public Action PhysicsUpdate = () => { };

        public void Initialize()
        {
            Init.Invoke();
        }

        public void Refresh()
        {
            Update.Invoke();
        }

        public void FixedRefresh()
        {
            PhysicsUpdate.Invoke();
        }

        public void RemoveObjectFromCollection(EnumType enumType, ObjType type)
        {
            Collection.Remove(type);
            wrapper.objectPool.Pool(enumType, type);
        }

        public void AddObjectToCollection(EnumType enumType, ObjData objData)
        {
            ObjType newObj = wrapper.objectFactory.CreateObject(enumType, objData);
            Collection.Add(newObj);
        }
    }
}

