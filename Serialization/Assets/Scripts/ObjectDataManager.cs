using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public class ObjectDataManager : MonoBehaviour
{
    public ObjectData objData { get; set; }
    public Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateData();
    }

    public void Init(ObjectData data)
    {
        objData = data;

        transform.position = objData.position.Unwrap();
        transform.eulerAngles = objData.rotation.Unwrap();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = objData.velocity.Unwrap();
    }

    public void UpdateData()
    {
        objData.position = new Vector3Wrapper(transform.position);
        objData.rotation = new Vector3Wrapper(transform.eulerAngles);
        objData.velocity = new Vector3Wrapper(rb.velocity);
    }

    [System.Serializable]
    public class ObjectData
    {
        [XmlElement("shape")]
        public int shape;
        [XmlElement("position")]
        public Vector3Wrapper position;
        [XmlElement("rotation")]
        public Vector3Wrapper rotation;
        [XmlElement("velocity")]
        public Vector3Wrapper velocity;

        public ObjectData() { }

        public ObjectData(int _shape, Vector3Wrapper _position, Vector3Wrapper _rotation, Vector3Wrapper _velocity)
        {
            shape = _shape;
            position = _position;
            rotation = _rotation;
            velocity = _velocity;
        }
    }

    [System.Serializable]
    public class Vector3Wrapper
    {
        public float x, y, z;

        public Vector3Wrapper() { }

        public Vector3Wrapper(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3Wrapper(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Vector3 Unwrap()
        {
            return new Vector3(x, y, z);
        }
    }
}
