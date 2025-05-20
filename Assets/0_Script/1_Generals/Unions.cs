using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

// Union : ����ü
// ������ ���Ŀ� ������� �ϳ��� �޸𸮿� ����.

// �ü�� 32 / 64
// �� ���� �д� �޸��� ũ��

// littleEndian : ���� ���� ���� ��.  ![0][0][1][1]�� [1][1][0][0]�� �Ǵ°� ���� �ƴ�! ����Ʈ�� ĭ ��ġ�� ���� �ڸ� ���� ������ ��ġ��.
// bigEndian : ū ���� �״�� ��.

[StructLayout(LayoutKind.Explicit)]
public class Unions
{
    public static Unions instance = new();
    // ������ �������� ���� �޸𸮸� ����ϴ� ����, ���� ������ �������� 4����Ʈ ũ�Ⱑ �����Ǿ�����.
    [FieldOffset(0)]
    public int integer;

    [FieldOffset(0)]
    public float real;

    [FieldOffset(0)]
    public short short0;

    [FieldOffset(2)]
    public short short1;

    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;


    public short[] Shorts
    {
        get => new short[] { short0, short1 };
        set
        {
            if (value.Length != 2) return;

            Type type = GetType();
            for (int i = 0; i < value.Length; i++)
            {
                // Reflection
                // SendMessage, BroadcastMessage, �ν����Ϳ��� ���� ���� ��
                if (value[i] >= short.MinValue && value[i] <= short.MaxValue)
                {
                    FieldInfo fieldInfo = type.GetField($"short{i}");
                    fieldInfo.SetValue(this, value[i]);
                }
            }
        }
    }
    public byte[] Bytes
    {
        get => new byte[4] { byte0, byte1, byte2, byte3 };
        set
        {
            int maxLength = Mathf.Min(4, value.Length);
            for (int i = 0; i < maxLength; i++) this[i] = value[i];
        }
    }

    public byte this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return byte0;
                case 1: return byte1;
                case 2: return byte2;
                case 3: return byte3;
                default: return (byte)0;
            }
        }
        set
        {
            switch (index)
            {
                case 0: byte0 = value; break;
                case 1: byte1 = value; break;
                case 2: byte2 = value; break;
                case 3: byte3 = value; break;
            }
        }

    }
}
