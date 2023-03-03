using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour, ISerializable
{
    List<GameObject> objectList;
    List<ObjectData> objDataList;

    [SerializeField]private int numberOfObjects = 0;
    string filePathJSON;
    string directoryPath;
    string filePathBinary;
    string filePathXML;

    [SerializeField, Range(0.5f, 5f)] private float minSpawnDistance;
    [SerializeField, Range(5f, 20f)] private float maxSpawnDistance;

    private void Awake()
    {
        objectList = new List<GameObject>();
        objDataList = new List<ObjectData>();

        directoryPath = Path.Combine(Application.streamingAssetsPath, "JsonExamples/"); 
        if (!Directory.Exists(directoryPath)) 
            Directory.CreateDirectory(directoryPath);

        filePathJSON = Path.Combine(directoryPath, "saveFile.txt");
        filePathBinary = Path.Combine(directoryPath, "bfexamplemain");
        filePathXML = Path.Combine(directoryPath, "monsters.xml");
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewSet();
            //SaveXML<ListWrapper>(new ListWrapper { dataList = objDataList });
            SaveJSon<ListWrapper>(new ListWrapper { dataList = objDataList });
            //SaveBinary<ListWrapper>(new ListWrapper { dataList = objDataList });
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GenerateOldSet();
        }
    }

    private void GeneratePrimitive(ObjectData objData)
    {
        GameObject gameObject = GameObject.CreatePrimitive((PrimitiveType)objData.shape);
        gameObject.transform.position = objData.position;
        gameObject.transform.eulerAngles = objData.rotation;
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = objData.velocity;

        objDataList.Add(objData);
        objectList.Add(gameObject);
    }

    private void GenerateNewSet()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            Destroy(objectList[i].gameObject);
        }

        objDataList.Clear();

        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector2 randPos = Random.insideUnitCircle;
            randPos.Normalize();
            randPos *= Random.Range(minSpawnDistance, maxSpawnDistance);
            ObjectData objData = new ObjectData(Random.Range(0, 3), new Vector3(randPos.x, Random.Range(maxSpawnDistance, maxSpawnDistance), randPos.y), new Vector3(0, 0, 0), new Vector3(0, -1, 0));
            GeneratePrimitive(objData);
        }
    }

    private void GenerateOldSet()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            Destroy(objectList[i].gameObject);
        }

        objDataList.Clear();

        //ListWrapper wrapper = LoadXML<ListWrapper>();
        ListWrapper wrapper = LoadJson<ListWrapper>();
        //ListWrapper wrapper = LoadBinary<ListWrapper>();
        objDataList = wrapper.dataList;
        numberOfObjects = objDataList.Count;

        for (int i = 0; i < numberOfObjects; i++)
        {
            GeneratePrimitive(objDataList[i]);
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

        return newClassLoadedFromJson;
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
            return (T)serializer.Deserialize(stream);
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
        T toRet;
        if (File.Exists(filePathBinary))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePathBinary, FileMode.Open);
            toRet = (T)bf.Deserialize(file);
            file.Close();
        }
        else
            toRet = default(T);
        return toRet;
    }

    [System.Serializable, XmlRoot("DataCollection")]

    public class ListWrapper
    {
        [XmlArray("DataList"), XmlArrayItem("ObjectData")]
        public List<ObjectData> dataList;
    }

    [System.Serializable]
    public class ObjectData 
    {
        [XmlAttribute("shape")]
        public int shape;
        [XmlAttribute("position")]
        public Vector3 position;
        [XmlAttribute("rotation")]
        public Vector3 rotation;
        [XmlAttribute("velocity")]
        public Vector3 velocity;

        public ObjectData(int _shape, Vector3 _position, Vector3 _rotation, Vector3 _velocity)
        {
            shape = _shape;
            position = _position;
            rotation = _rotation;
            velocity = _velocity;
        }
    }
}