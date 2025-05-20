using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public partial class FileManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }

    public static GraphicOptionValues SavedGraphicOptions { get; private set; }

    private string mainDirectory;
    private string saveDirectory;
    private string optionDirectory;
}

public partial class FileManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        // 응용 프로그램이 사용하는 데이터 경로 : Application.persistentDataPath
        // Application.persistentDataPath : 런타임에 결정되기 때문에 런타임에 사용 가능, 기본값에서 사용 불가능.
        mainDirectory = $"{Application.persistentDataPath}/Datas";
        saveDirectory = $"{mainDirectory}/Saves";
        optionDirectory = $"{mainDirectory}/Options";

        try
        {
            SavedGraphicOptions = LoadFile(optionDirectory, "GraphicSettings.set").ByteArray2Struct<GraphicOptionValues>();
        }
        catch
        {
            SavedGraphicOptions = GraphicOptionValues.defaultOption;
            // TODO 아랫 줄 그냥 내가 써놓음 바뀔 수 있음
            SaveFile(optionDirectory, "GraphicSettings.set", SavedGraphicOptions.Struct2ByteArray()); 
            Debug.LogWarning("GraphicSetting File Not Found");
        }

        yield break;
    }
    public void Exit()
    {

    }
}

public partial class FileManager : MonoBehaviour, IManagerBase // Property
{
    public static GameObject GetPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }


    // HxD를 통해 저장한 파일을 읽어볼 수 있음.
    public static void SaveFile(string directory, string fileName, params byte[] data)
    {
        // params : 가변 매개변수, 매개변수를 넣지 않거나  ,로 구분하여 넣거나 byme[]을 매개변수로 넣을 수 있음;
        string totalDirectory = $"{directory}/{fileName}";

        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // File.Create -> FileStream 반환, 하나의 프로그램이라도 여러개의 FileStream으로 하나의 파일을 관리할 수 없음.
        if (!File.Exists(totalDirectory)) File.Create(totalDirectory).Close();
        File.WriteAllBytes(totalDirectory, data);
    }

    public byte[] LoadFile(string path, string fileName)
    {
        if (Directory.Exists(path))
        {
            string totalDirectory = $"{path}/{fileName}";
            if (File.Exists(totalDirectory))
            {
                return File.ReadAllBytes(totalDirectory);
            }
        }
        return null;
    }

    // Serilaize() <= 구현만 해보고 안씀.
    public byte[] Serialize(string name, int level, float exp)
    {
        // ↓↓ name의 최대 크기는 6으로 확정지은 상황 ↓↓
        // UTF-8을 사용하기 때문에 길이를 여유롭게 선언함.
        byte[] result = new byte[20];

        // 문자를 바이트로 변환할 때 필요한 요소 : 유니코드
        // C#의 char == 2byte -> 전 세계의 언어를 다 표시해야하기 때문

        // 인코딩 방식 : 다른 인코딩 방식으로 저장하거나 읽으면 의도하지않은 값이 저장되거나 읽어짐.
        // 유니코드 인코딩 방식 : UTF-8,16,32 , ANSI - CP949
        byte[] nameByte = Encoding.UTF8.GetBytes(name);
        Array.Resize(ref nameByte, 12);
        Array.Copy(nameByte, 0, result, 0, 12);


        // SourceArray (Bytes)의 인덱스[0]부터 4개 요소(지금은 4바이트이기 때문에)를 destArray (result)의 인덱스[6]부터 복사;
        Unions.instance.integer = level;
        Array.Copy(Unions.instance.Bytes, 0, result, 12, 4);

        Unions.instance.real = exp;
        Array.Copy(Unions.instance.Bytes, 0, result, 16, 4);

        return result;
    }
}
