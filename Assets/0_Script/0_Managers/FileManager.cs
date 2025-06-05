using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public partial class FileManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }

    public static GraphicOptionValues SavedGraphicOptions { get; protected set; }

    private static Dictionary<CharacterType, GameObject> characterPrefabDict;
    private static Dictionary<ControllerType, GameObject> controllerPrefabDict;

    private string mainDirectory;
    private string saveDirectory;
    private string optionDirectory;
}

public partial class FileManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        // ���� ���α׷��� ����ϴ� ������ ��� : Application.persistentDataPath
        // Application.persistentDataPath : ��Ÿ�ӿ� �����Ǳ� ������ ��Ÿ�ӿ� ��� ����, �⺻������ ��� �Ұ���.
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
            // TODO �Ʒ� �� �׳� ���� ����� �ٲ� �� ����
            SaveFile(optionDirectory, "GraphicSettings.set", SavedGraphicOptions.Struct2ByteArray());
            Debug.LogWarning("GraphicSetting File Not Found");
        }

        yield return InitializeCharacterPrefabs();

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

    private IEnumerator InitializeCharacterPrefabs()
    {
        if (characterPrefabDict is not null) yield break;

        // �ε� �ð����� ������ �ε�.
        // Tick : ��ǻ�Ͱ� �� �� �����̴� ���� �ǹ���.
        // Tick�̸��� ����ϱ� ������, ��Ȯ���� ms����.
        // �ü������ �����ϴ� ����� ����ϱ� ������ �Ϻ��� ��Ȯ������ ����.
        int lastTime = Environment.TickCount;

        characterPrefabDict = new();
        for (CharacterType i = CharacterType.PlayerCharacterStart + 1;
            i < CharacterType.PlayerCharacterEnd;
            i++)
        {
            GameObject currentCharacter = Resources.Load<GameObject>($"Prefabs/Characters/{i.ToString()}");
            if (currentCharacter is not null)
                characterPrefabDict.TryAdd(i, currentCharacter);

            int currentTime = Environment.TickCount;

            if (lastTime + 200 < currentTime)
            {
                lastTime = currentTime;
                yield return null;
            }
        }

        // TODO TEST
        controllerPrefabDict = new();
        controllerPrefabDict.TryAdd(ControllerType.ControllerBase, Resources.Load<GameObject>("Prefabs/Controllers/ControllerBase"));



        /*
            Time.time -> float , ms ������ ��⿡�� 
            float�� �Ҽ��� �ִ� ���� �պ��� 6~7�ڸ� ��ŭ�� ������.
         */

        /*
            ���� ���� ��� �� ������, StartNew ������ �÷��Ͱ� ȣ��� ������ ����.   

            ������ ���� ����
            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();

            // �۾� �� �ڵ� ����

            timer.Stop();
         */
    }

    // HxD�� ���� ������ ������ �о �� ����.
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

    // Serilaize() <= ������ �غ��� �Ⱦ�.
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

    public static GameObject GetCharacterPrefab(CharacterType wantType)
        => characterPrefabDict.TryGetValue(wantType, out GameObject result) ? result : null;
    public static GameObject GetControllerPrefab(ControllerType wantType)
          => controllerPrefabDict.TryGetValue(wantType, out GameObject result) ? result : null;
}
