using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    public Queue<GameObject> RootQueue { get; set; }
    public void Initialize();
    public void Return2Pool();
}
