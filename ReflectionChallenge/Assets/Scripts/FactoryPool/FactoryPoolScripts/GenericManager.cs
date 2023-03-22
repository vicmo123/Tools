using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace FactoryPool
{
    public class GenericManager<EnumType, ObjType, ObjData> : IManager<EnumType, ObjType, ObjData> where ObjType : MonoBehaviour, IFactoryInitializable<ObjData>, IPoolable<EnumType, ObjData>
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

        public void ManageCollectionWithConditions(params Predicate<ObjType>[] conditions)
        {
            Queue<ObjType> toDelete = new Queue<ObjType>();

            foreach (var obj in Collection)
            {
                foreach (var condition in conditions)
                {
                    if (condition.Invoke(obj))
                    {
                        toDelete.Enqueue(obj);
                        break;
                    }
                }
            }

            for (int i = 0; i < toDelete.Count; i++)
            {
                ObjType objToDel = toDelete.Dequeue();
                RemoveObjectFromCollection(objToDel.GetEnumType(), objToDel);
            }
        }
    }
}

