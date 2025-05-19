using System.Collections;
using UnityEngine;

public struct GraphicOptionValues
{
    public enum ResolutionType
    {
        _800x480,
        _800x600,
        _1152x768,
        _1280x720,
        _1440x1080,
        _1920x1080,
        _2048x1080,
        _2560x1440,
        _LENGTH
    }

    public string test1;
    public ResolutionType resolutionType;
    public int frameRate;
    public int graphicLevel;
    public int brightness;
    public int shadowLevel;
    public float contrast;
    public bool verticalSynchronization;
    public bool antiAliasing;
    public bool fullScreen;
    public string test2;
}

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
