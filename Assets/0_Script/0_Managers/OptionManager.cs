using System.Collections;
using UnityEngine;

public partial class OptionManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }
}

public partial class OptionManager : MonoBehaviour, IManagerBase // Initialize
{
    public void Exit()
    {

    }

    public IEnumerator Initialize()
    {
        yield break;
    }
}

public partial class OptionManager : MonoBehaviour, IManagerBase // 
{

}
