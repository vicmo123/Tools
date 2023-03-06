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
using System.Reflection;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public Button[] buttons;
    public Action SaveJson, LoadJson, SaveXml, LoadXml, SaveBinary, LoadBinary, CreateNewSet;

    List<ObjectDataManager> objectList;

    #region object spawn stats
    [SerializeField] private int numberOfObjects = 0;
    private float minSpawnWidth = 2f;
    private float maxSpawnWidth = 25f;
    private float spawnHeight = 25f;
    private float angleRange = 360f;
    private Vector2 spawnHeightMultiplierRange = new Vector2(0.5f, 1.5f);
    private Vector2 fallSpeedRange = new Vector2(0.5f, 5f);
    Vector2Int shapeRange = new Vector2Int(0, 3);
    #endregion

    string filePathJSON;
    string filePathXML;
    string filePathBinary;

    private void Awake()
    {
        objectList = new List<ObjectDataManager>();

        filePathJSON = GeneratePath("JsonExamples/", "saveFile.txt");
        filePathXML = GeneratePath("XML/", "saveFile.xml");
        filePathBinary = GeneratePath("Binary/", "bfexample");
    }

    private void Start()
    {
        //JSON
        SaveJson += () =>
        {
            Serializer.Instance.Save(type: Serializer.SerializationType.JSON, toSave: objectList.GetWrappedList(), filePath: filePathJSON);
        };
        LoadJson += () =>
        {
            ListWrapper newList = Serializer.Instance.Load<ListWrapper>(type: Serializer.SerializationType.JSON, filePath: filePathJSON);
            LoadList(newList);
        };

        //XML
        SaveXml += () => {
            Serializer.Instance.Save(type: Serializer.SerializationType.XML, toSave: objectList.GetWrappedList(), filePath: filePathXML);
        };
        LoadXml += () =>
        {
            ListWrapper newList = Serializer.Instance.Load<ListWrapper>(type: Serializer.SerializationType.XML, filePath: filePathXML);
            LoadList(newList);
        };

        //BINARY
        SaveBinary += () => {
            Serializer.Instance.Save(type: Serializer.SerializationType.BINARY, toSave: objectList.GetWrappedList(), filePath: filePathBinary);
        };
        LoadBinary += () =>
        {
            ListWrapper newList = Serializer.Instance.Load<ListWrapper>(type: Serializer.SerializationType.BINARY, filePath: filePathBinary);
            LoadList(newList);
        };

        //OTHER
        CreateNewSet += () => { LoadList(); };

        SetButtonEvents();
    }

    private void SetButtonEvents()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            string buttonName = buttons[i].name;
            FieldInfo field = GetType().GetField(buttonName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null && field.FieldType == typeof(Action))
            {
                //Debug.Log("Delegate found for " + buttonName);
                var actionDelegate = (Action)field.GetValue(this);
                buttons[i].onClick.AddListener(() => { actionDelegate.Invoke(); });
            }
            else
            {
                Debug.LogError("Delegate not found for " + buttonName);
            }
        }
    }

    private string GeneratePath(string folderName, string fileName)
    {
        string directoryPath = Path.Combine(Application.streamingAssetsPath, folderName);

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        return Path.Combine(directoryPath, fileName);
    }

    private ObjectDataManager.ObjectData GenerateNewData()
    {
        ObjectDataManager.ObjectData objData;

        Vector2 rand = Random.insideUnitCircle.normalized * Random.Range(minSpawnWidth, maxSpawnWidth);
        Vector3 randomPosition = new Vector3(rand.x, spawnHeight * Random.Range(spawnHeightMultiplierRange.x, spawnHeightMultiplierRange.y), rand.y);
        Vector3 randomAngle = new Vector3(Random.Range(-angleRange, angleRange), Random.Range(-angleRange, angleRange), Random.Range(-angleRange, angleRange));
        Vector3 randomVelocity = -Vector3.up * Random.Range(fallSpeedRange.x, fallSpeedRange.y);

        objData = new ObjectDataManager.ObjectData(_shape: Random.Range(shapeRange.x, shapeRange.y),
                                                   _position: new ObjectDataManager.Vector3Wrapper(randomPosition),
                                                   _rotation: new ObjectDataManager.Vector3Wrapper(randomAngle),
                                                   _velocity: new ObjectDataManager.Vector3Wrapper(randomVelocity)
        );

        return objData;
    }

    private void InitObject(ObjectDataManager.ObjectData objData)
    {
        if (objData == null)
        {
            objData = GenerateNewData();
        }

        GameObject newObj = GameObject.CreatePrimitive((PrimitiveType)objData.shape);
        newObj.AddComponent<ObjectDataManager>().Init(objData);
        objectList.Add(newObj.GetComponent<ObjectDataManager>());
    }

    private void GeneratePrimitive(ObjectDataManager.ObjectData data = null)
    {
        InitObject(data);
    }

    private void LoadList(ListWrapper wrapper = null)
    {
        objectList.ClearListMonobehaviours();

        if (wrapper == null)
            for (int i = 0; i < numberOfObjects; i++)
                GeneratePrimitive();
        else
            for (int i = 0; i < wrapper.dataList.Count; i++)
                GeneratePrimitive(wrapper.dataList[i]);

    }

    [System.Serializable, XmlRoot("DataCollection")]
    public class ListWrapper
    {
        [XmlArray("DataList"), XmlArrayItem("ObjectData")]
        public List<ObjectDataManager.ObjectData> dataList;

        public ListWrapper() { }
    }
}