using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CustomCollection<int> custom = new();
        custom.Add(5);

        CustomCollection<string> custom2 = new();
        custom2.Add("Bob");
        
        CustomCollection<IceCreamFlavor> custom3 = new();
        custom3.Add(new IceCreamFlavor());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class IceCreamFlavor : IComparable
{
    public int chocolateChips;
    public float metricTonsOfCaramel;

    public int CompareTo(object obj)
    {
        if(obj is IceCreamFlavor otherIceCream)
        {
            if (metricTonsOfCaramel > otherIceCream.metricTonsOfCaramel)
                return 1;
            else if(metricTonsOfCaramel < otherIceCream.metricTonsOfCaramel)
                return -1;
            else
                return 0;

            throw new System.Exception($"Cannot compare { this.GetType().Name } to " + obj.GetType());
        }

        throw new NotImplementedException();
    }
}
