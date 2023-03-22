using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FactoryPool
{
    public class GenericFactory<EnumType, ObjType, ObjData> where ObjType : MonoBehaviour, IPoolable<EnumType, ObjData>, IFactoryInitializable<ObjData>
    {
        public Dictionary<EnumType, GameObject> resourceDict;
        GenericWrapper<EnumType, ObjType, ObjData> wrapper;

        public void Initialize()
        {
            wrapper = GenericWrapper<EnumType, ObjType, ObjData>.Instance;
            resourceDict = new Dictionary<EnumType, GameObject>();
            List<EnumType> enums = System.Enum.GetValues(typeof(EnumType)).Cast<EnumType>().ToList();

            foreach (EnumType enumType in enums)
            {
                try
                {
                    GameObject prefab = LoadPrefab(enumType.ToString(), "Prefabs/");
                    if (prefab == null)
                        Debug.LogError("Unable to load prefab of type : " + enumType.ToString() + " because it is null");
                    else
                        resourceDict.Add(enumType, prefab);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Unable to load prefab of type : " + enumType.ToString());
                }
            }
        }

        public GameObject LoadPrefab(string path, string folder = null)
        {
            return Resources.Load<GameObject>(folder + path);
        }

        public ObjType CreateObject(EnumType enumType, ObjData objDat)
        {
            ObjType toRet = ReleaseObject(enumType);
            if (toRet == null)
            {
                toRet = Instanciate(enumType);
            }
            toRet.Initialize(objDat);
            return toRet;
        }

        public ObjType Instanciate(EnumType enumType)
        {
            GameObject toRet = GameObject.Instantiate<GameObject>(resourceDict[enumType].gameObject);
            return toRet.AddComponent<ObjType>();
        }

        private ObjType ReleaseObject(EnumType enumType)
        {
            ObjType toRet = wrapper.objectPool.Depool(enumType);
            if (toRet == null)
            {
                return null;
            }
            return toRet;
        }
    }
}
 
