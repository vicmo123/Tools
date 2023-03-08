using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FactoryPool
{
    public class GenericPool<EnumType, ObjType, ObjData> where ObjType : MonoBehaviour, IPoolable<EnumType, ObjData>, IFactoryInitializable<ObjData>
    {
        public GenericWrapper<EnumType, ObjType, ObjData> wrapper;
        Dictionary<EnumType, Queue<ObjType>> poolDict;

        public void Initialize()
        {
            wrapper = GenericWrapper<EnumType, ObjType, ObjData>.Instance;
            poolDict = new Dictionary<EnumType, Queue<ObjType>>();

            List<EnumType> enums = System.Enum.GetValues(typeof(EnumType)).Cast<EnumType>().ToList();

            foreach (EnumType enumType in enums)
            {
                poolDict.Add(enumType, new Queue<ObjType>());
            }
        }

        public void Pool(EnumType enumType, ObjType objToPool)
        {
            objToPool.Deactivate();
            poolDict[enumType].Enqueue(objToPool);
        }

        public ObjType Depool(EnumType type)
        {
            ObjType toRet = (poolDict[type].Count > 0) ? poolDict[type].Dequeue() : null;
            if (toRet)
                toRet.Activate();
            return toRet;
        }

        //TODO
        //public void PrefillPool(int quantity, EnumType enumType, ObjType objToPool)
        //{
        //    for (int i = 0; i < quantity; i++)
        //    {
        //        Pool(enumType, objToPool);
        //    }
        //}
    }
}
    




