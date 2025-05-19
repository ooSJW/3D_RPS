using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public partial class FileManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }

    private string mainDirectory;
    private string saveDirectory;
    private string optionDirectory;
}

public partial class FileManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        GraphicOptionValues test = new()
        {
            test1 = "bcdbcdbcd",
            test2 = "aba",
            resolutionType = GraphicOptionValues.ResolutionType._800x480,
            frameRate = 1,
            graphicLevel = 2,
            brightness = 3,
            shadowLevel = 4,
            contrast = 5.0f,
            verticalSynchronization = false,
            antiAliasing = true,
            fullScreen = false,
        };
        foreach (byte current in test.Struct2ByteArray())
        {
           Debug.Log(current);
        }

        GraphicOptionValues test2 = test.Struct2ByteArray().ByteArray2Struct<GraphicOptionValues>();
        Debug.Log($"test2.test1:{test2.test1}");
        Debug.Log($"test2.test2:{test2.test2}");
        Debug.Log($"test2.antiAliasing:{test2.antiAliasing}");
        Debug.Log($"test2.brightness:{test2.brightness}");
        Debug.Log($"test2.contrast:{test2.contrast}");
        Debug.Log($"test2.frameRate:{test2.frameRate}");
        Debug.Log($"test2.fullScreen:{test2.fullScreen}");
        Debug.Log($"test2.graphicLevel:{test2.graphicLevel}");
        Debug.Log($"test2.shadowLevel:{test2.shadowLevel}");
        Debug.Log($"test2.verticalSynchronization:{test2.verticalSynchronization}");


        // ���� ���α׷��� ����ϴ� ������ ��� : Application.persistentDataPath
        // Application.persistentDataPath : ��Ÿ�ӿ� �����Ǳ� ������ ��Ÿ�ӿ� ��� ����, �⺻������ ��� �Ұ���.
        mainDirectory = $"{Application.persistentDataPath}/Datas";
        saveDirectory = $"{mainDirectory}/Saves";
        optionDirectory = $"{mainDirectory}/Options";

        SaveFile(saveDirectory, "TestFile.sav", Serialize("�����ٶ󸶹ٻ��", 10, 98.5f));
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
    public static void SaveFile(string directory, string fileName, params byte[] data)
    {
        // params : ���� �Ű�����, �Ű������� ���� �ʰų�  ,�� �����Ͽ� �ְų� byme[]�� �Ű������� ���� �� ����;
        string totalDirectory = $"{directory}/{fileName}";

        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // File.Create -> FileStream ��ȯ, �ϳ��� ���α׷��̶� �������� FileStream���� �ϳ��� ������ ������ �� ����.
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

    public byte[] Serialize(string name, int level, float exp)
    {
        // ��� name�� �ִ� ũ��� 6���� Ȯ������ ��Ȳ ���
        // UTF-8�� ����ϱ� ������ ���̸� �����Ӱ� ������.
        byte[] result = new byte[20];

        // ���ڸ� ����Ʈ�� ��ȯ�� �� �ʿ��� ��� : �����ڵ�
        // C#�� char == 2byte -> �� ������ �� �� ǥ���ؾ��ϱ� ����

        // ���ڵ� ��� : �ٸ� ���ڵ� ������� �����ϰų� ������ �ǵ��������� ���� ����ǰų� �о���.
        // �����ڵ� ���ڵ� ��� : UTF-8,16,32 , ANSI - CP949
        byte[] nameByte = Encoding.UTF8.GetBytes(name);
        Array.Resize(ref nameByte, 12);
        Array.Copy(nameByte, 0, result, 0, 12);


        // SourceArray (Bytes)�� �ε���[0]���� 4�� ���(������ 4����Ʈ�̱� ������)�� destArray (result)�� �ε���[6]���� ����;
        Unions.instance.integer = level;
        Array.Copy(Unions.instance.Bytes, 0, result, 12, 4);

        Unions.instance.real = exp;
        Array.Copy(Unions.instance.Bytes, 0, result, 16, 4);

        return result;
    }
}
