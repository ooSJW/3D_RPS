using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterBase : MonoBehaviour, IPoolable // Data Field
{
    public Queue<GameObject> RootQueue { get; set; }


}
public partial class CharacterBase // Initialize
{
    public void Initialize()
    {

    }

    public void Return2Pool()
    {

    }
}


public partial class CharacterBase
{

}