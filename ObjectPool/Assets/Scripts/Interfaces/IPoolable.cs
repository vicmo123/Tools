using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    bool IsActive { get; set; }
    void Activate();
    void Deactivate();
}
