using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour, ISerializable
{
    public Button saveJson;
    public Button loadJson;
    public Button saveXml;
    public Button loadXml;
    public Button saveBinary;
    public Button loadBinary;
    public Button createNewSet;

    List<ObjectDataManager> objectList;

    [SerializeField]private int numberOfObjects = 0;
    private float minSpawnDistance = 2f;
    private float maxSpawnDistance = 25f;
    private float spawnHeight = 25f;

    string filePathJSON;
    string directoryPathJson;

    string filePathXML;
    string directoryPathXML;

    string filePathBinary;
    string directoryPathBinary;

    private void Awake()
    {
        objectList = new List<ObjectDataManager>();

        directoryPathJson = Path.Combine(Application.streamingAssetsPath, "JsonExamples/");
        directoryPathXML = Path.Combine(Application.streamingAssetsPath, "XML/");
        directoryPathBinary = Path.Combine(Application.streamingAssetsPath, "Binary/");

        if (!Directory.Exists(directoryPathJson)) 
            Directory.CreateDirectory(directoryPathJson);
        if (!Directory.Exists(directoryPathXML))
            Directory.CreateDirectory(directoryPathXML);
        if (!Directory.Exists(directoryPathBinary))
            Directory.CreateDirectory(directoryPathBinary);

        filePathJSON = Path.Combine(directoryPathJson, "saveFile.txt");
        filePathBinary = Path.Combine(directoryPathXML, "bfexample");
        filePathXML = Path.Combine(directoryPathBinary, "saveFile.xml");
    }

    private void Start()
    {
        saveJson.onClick.AddListener(() => {
            SaveJSon<ListWrapper>(new ListWrapper { dataList = (objectList.FromTtoG((val) => { return val.objData; })).ToList<ObjectDataManager.ObjectData>()}); 
        });
        loadJson.onClick.AddListener(() => {
            ClearList();
            LoadList(LoadJson<ListWrapper>());
        });

        saveXml.onClick.AddListener(() => {
            SaveXML<ListWrapper>(new ListWrapper { dataList = (objectList.FromTtoG((val) => { return val.objData; })).ToList<ObjectDataManager.ObjectData>() });
        });
        loadXml.onClick.AddListener(() => {
            ClearList();
            LoadList(LoadXML<ListWrapper>());
        });

        saveBinary.onClick.AddListener(() => {
            SaveBinary<ListWrapper>(new ListWrapper { dataList = (objectList.FromTtoG((val) => { return val.objData; })).ToList<ObjectDataManager.ObjectData>() });
        });
        loadBinary.onClick.AddListener(() => {
            ClearList();
            LoadList(LoadBinary<ListWrapper>());
        });

        createNewSet.onClick.AddListener(() => { GenerateNewSet(); });
    }

    private void Update()
    {
    }

    private ObjectDataManager.ObjectData SetObjData(ObjectDataManager.ObjectData data)
    {
        if (data == null)
        {
            Vector2 randPos = Random.insideUnitCircle;
            randPos.Normalize();
            randPos *= Random.Range(minSpawnDistance, maxSpawnDistance);
            data = new ObjectDataManager.ObjectData(Random.Range(0, 3), new ObjectDataManager.Vector3Wrapper(randPos.x, spawnHeight * Random.Range(0.5f, 1.5f), randPos.y), new ObjectDataManager.Vector3Wrapper(0, 0, 0), new ObjectDataManager.Vector3Wrapper(0, -1, 0));
        }

        return data;
    }

    private void Init(ObjectDataManager.ObjectData objData)
    {
        GameObject newObj = GameObject.CreatePrimitive((PrimitiveType)objData.shape);
        newObj.transform.position = objData.position.Unwrap();
        newObj.transform.eulerAngles = objData.rotation.Unwrap();

        Rigidbody rb = newObj.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = objData.velocity.Unwrap();

        newObj.AddComponent<ObjectDataManager>().objData = objData;
        objectList.Add(newObj.GetComponent<ObjectDataManager>());
    }

    private void GeneratePrimitive(ObjectDataManager.ObjectData data = null)
    {
        Init(SetObjData(data));
    }

    private void GenerateNewSet()
    {
        ClearList();

        for (int i = 0; i < numberOfObjects; i++)
        {
            GeneratePrimitive();
        }
    }

    private void ClearList()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            Destroy(objectList[i].gameObject);
        }

        objectList.Clear();
    }

    private void LoadList(ListWrapper wrapper)
    {
        if(wrapper != null)
        {
            for (int i = 0; i < wrapper.dataList.Count; i++)
            {
                GeneratePrimitive(wrapper.dataList[i]);
            }
        }
    }

    public void SaveJSon<T>(T toSave)
    {
        string jsonSerializationOfNewClass = JsonUtility.ToJson(toSave);
        File.WriteAllText(filePathJSON, jsonSerializationOfNewClass);
    }

    public T LoadJson<T>()
    {
        string jsonDeserialized = File.ReadAllText(filePathJSON);
        T newClassLoadedFromJson = JsonUtility.FromJson<T>(jsonDeserialized);

        return newClassLoadedFromJson == null ? default(T) : newClassLoadedFromJson;
    }

    public void SaveXML<T>(T toSave)
    {
        var serializer = new XmlSerializer(typeof(T));
        using (var stream = new FileStream(filePathXML, FileMode.Create))
        {
            serializer.Serialize(stream, toSave);
        }
    }

    public T LoadXML<T>()
    {
        var serializer = new XmlSerializer(typeof(T));
        using (var stream = new FileStream(filePathXML, FileMode.Open))
        {
            return (T)serializer.Deserialize(stream) ?? default(T);
        }
    }

    public void SaveBinary<T>(T toSave)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePathBinary);
        bf.Serialize(file, toSave);
        file.Close();
    }

    public T LoadBinary<T>()
    {
        T toRet = default(T);
        if (File.Exists(filePathBinary))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePathBinary, FileMode.Open);
            toRet = (T)bf.Deserialize(file);
            file.Close();
        }
        return toRet == null ? default(T) : toRet;
    }

    [System.Serializable, XmlRoot("DataCollection")]
    public class ListWrapper
    {
        [XmlArray("DataList"), XmlArrayItem("ObjectData")]
        public List<ObjectDataManager.ObjectData> dataList;

        public ListWrapper() { }
    }
}