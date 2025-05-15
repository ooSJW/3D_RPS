using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

// Union : 공용체
// 데이터 형식에 상관없이 하나의 메모리에 저장.

// 운영체제 32 / 64
// 한 번에 읽는 메모리의 크기

// littleEndian : 작은 수를 먼저 씀.  ![0][0][1][1]이 [1][1][0][0]이 되는건 절대 아님! 바이트의 칸 위치가 작은 자릿 수가 앞으로 위치함.
// bigEndian : 큰 수를 그대로 씀.

[StructLayout(LayoutKind.Explicit)]
public class Unions
{
    public static Unions instance = new();
    // 변수가 여러개라도 같은 메모리를 사용하는 형태, 현재 변수가 여러개라도 4바이트 크기가 유지되어있음.
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
                // SendMessage, BroadcastMessage, 인스펙터에서 변수 조정 등
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
            if (value.Length != 4) return;

            byte0 = value[0];
            byte1 = value[1];
            byte2 = value[2];
            byte3 = value[3];
        }
    }
}
