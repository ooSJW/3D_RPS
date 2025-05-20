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

    // public string version : 버전 변경 시 호환을 위해 사용할 수 있는 변수.
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
        // 그래픽 설정 기본 세팅 값
        resolutionType = GraphicOptionValues.ResolutionType._1920x1080,
        shadowLevel = ShadowQuality.All,
        antiAliasing = AntiAliasingSampling.MSAA_4X,
        frameRate = 65,
        graphicLevel = 2, // Project settings -> Quality -> setting의 순서를 뜻함
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

        // vsync : 프레임과 프레임 사이에 그려놓을 화면 수
        QualitySettings.vSyncCount = value.verticalSynchronization ? 1 : 0;

        // antiAliasing : 계단 현상 방지 -> 경계를 뿌옇게 만듦.
        // rendering path
        // Vertex Shader : 정점 쉐이더
        // Pixel Shader : 색체를 칠하는 단계
        // Post Process : 후처리
        // 주변 픽셀과의 괴리(색 차이)를 통해 경계면을 판단함.
        // 샘플링할 픽셀의 수 ( 몇 개의 픽셀을 가져와 경계면을 판단할지 )
        QualitySettings.antiAliasing = (int)value.antiAliasing;

        // 밝기 대비는 post processing 필요 (packageManager)
        // 밝기 대비를 바꾸려면 기존 색이 필요함.
        // 색체 작업은 pixel Shader단계에서 작업이 완료되기 때문에 이후 순서인 Post Process단계에서 밝기, 대비 조절 가능
        // brightness
        // contrast
    }

}

public partial class OptionManager : MonoBehaviour, IManagerBase // 
{
    // 패턴 일치 방식
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

            // string을 변환시키는 함수를 사본을 만든 후 반환함, 원본 변환x
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
