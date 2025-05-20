using System;
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

    public enum AntiAliasingSampling
    {
        None = 0,
        MSAA_2X = 2,
        MSAA_4X = 4,
        MSAA_8X = 8,
    }

    // public string version : ���� ���� �� ȣȯ�� ���� ����� �� �ִ� ����.
    public ResolutionType resolutionType;
    public ShadowQuality shadowLevel;
    public AntiAliasingSampling antiAliasing;
    public int frameRate;
    public int graphicLevel;
    public int brightness;
    public float contrast;
    public bool verticalSynchronization;
    public bool fullScreen;

    public static GraphicOptionValues defaultOption = new()
    {
        // �׷��� ���� �⺻ ���� ��
        resolutionType = GraphicOptionValues.ResolutionType._1920x1080,
        shadowLevel = ShadowQuality.All,
        antiAliasing = AntiAliasingSampling.MSAA_4X,
        frameRate = 65,
        graphicLevel = 2, // Project settings -> Quality -> setting�� ������ ����
        brightness = 4,
        contrast = 0.2f,
        verticalSynchronization = false,
        fullScreen = true,
    };
}

public partial class OptionManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }
}

public partial class OptionManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        ApplyGraphicSetting(FileManager.SavedGraphicOptions);
        yield break;
    }
    public void Exit()
    {

    }

    public void ApplyGraphicSetting(GraphicOptionValues value)
    {
        // resolution , fullScreen
        Vector2Int resolution = GetResolution(value.resolutionType);
        Screen.SetResolution(resolution.x, resolution.y, value.fullScreen);

        // framRate
        Application.targetFrameRate = value.frameRate;

        // shadowLevel
        QualitySettings.shadows = value.shadowLevel;

        // graphicLevel
        QualitySettings.SetQualityLevel(value.graphicLevel);

        // vsync : �����Ӱ� ������ ���̿� �׷����� ȭ�� ��
        QualitySettings.vSyncCount = value.verticalSynchronization ? 1 : 0;

        // antiAliasing : ��� ���� ���� -> ��踦 �ѿ��� ����.
        // rendering path
        // Vertex Shader : ���� ���̴�
        // Pixel Shader : ��ü�� ĥ�ϴ� �ܰ�
        // Post Process : ��ó��
        // �ֺ� �ȼ����� ����(�� ����)�� ���� ������ �Ǵ���.
        // ���ø��� �ȼ��� �� ( �� ���� �ȼ��� ������ ������ �Ǵ����� )
        QualitySettings.antiAliasing = (int)value.antiAliasing;

        // ��� ���� post processing �ʿ� (packageManager)
        // ��� ��� �ٲٷ��� ���� ���� �ʿ���.
        // ��ü �۾��� pixel Shader�ܰ迡�� �۾��� �Ϸ�Ǳ� ������ ���� ������ Post Process�ܰ迡�� ���, ��� ���� ����
        // brightness
        // contrast
    }

}

public partial class OptionManager : MonoBehaviour, IManagerBase // 
{
    // ���� ��ġ ���
    public static Vector2Int GetResolution(GraphicOptionValues.ResolutionType from) => from switch
    {
        GraphicOptionValues.ResolutionType._800x480 => new Vector2Int(800, 480),
        GraphicOptionValues.ResolutionType._800x600 => new Vector2Int(800, 600),
        GraphicOptionValues.ResolutionType._1152x768 => new Vector2Int(1152, 768),
        GraphicOptionValues.ResolutionType._1280x720 => new Vector2Int(1280, 720),
        GraphicOptionValues.ResolutionType._1440x1080 => new Vector2Int(1440, 1080),
        GraphicOptionValues.ResolutionType._2048x1080 => new Vector2Int(2048, 1080),
        GraphicOptionValues.ResolutionType._2560x1440 => new Vector2Int(2560, 1440),


        _ => new Vector2Int(1920, 1080),
    };


    public static Vector2Int CalulateResolution(GraphicOptionValues.ResolutionType from)
    {
        if (Enum.IsDefined(typeof(GraphicOptionValues.ResolutionType), from))
        {
            string originName = from.ToString();

            // string�� ��ȯ��Ű�� �Լ��� �纻�� ���� �� ��ȯ��, ���� ��ȯx
            originName = originName.TrimStart('_');
            string[] splits = originName.Split('x');

            if (splits.Length == 2)
            {
                if (int.TryParse(splits[0], out int width) && int.TryParse(splits[1], out int height))
                    return new Vector2Int(width, height);
            }
        }
        return CalulateResolution(GraphicOptionValues.defaultOption.resolutionType);
    }
}
