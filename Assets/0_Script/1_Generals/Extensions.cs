using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class Extensions
{
    public static bool GenericSingleton<T>(this T newTarget, ref T slot)
    {
        if (newTarget == null)
            return false;
        else if (newTarget.Equals(slot))
            return true;
        else if (slot == null)
        {
            slot = newTarget;
            return true;
        }
        else
        {
            return false;
        }
    }


    public static byte[] Struct2ByteArray<T>(this T instance)
    {
        Type resultType = typeof(T);
        if (resultType.IsStruct())
        {
            FieldInfo[] allFields = resultType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Array.Sort(allFields) <- ���������� �� ������, ������ �޸𸮸� ���� ������ ������ �ٲ�.

            // LinQ���
            //                                            ������ ������� ����,                       �ּҿ� ����ִ� ���� int32�� �޾ƿ� 
            allFields = allFields.OrderBy(currentField => Marshal.OffsetOf(resultType, currentField.Name).ToInt32()).ToArray();

            int totalLength = allFields.Sum(currentField =>
            {
                if (currentField.FieldType == typeof(string))
                {
                    return currentField.FieldType.GetSize() + (currentField.GetValue(instance)).ToByteArray().Length;
                }
                else
                    return currentField.FieldType.GetSize();
            });


            byte[] result = new byte[totalLength];
            int offset = 0;

            foreach (FieldInfo current in allFields)
            {
                // instance��ü�� current��� ������ ���� ������.
                byte[] buffer = current.GetValue(instance).ToByteArray();

                if (current.FieldType == typeof(string))
                {
                    int bufferSize = typeof(string).GetSize();
                    Array.Copy(((short)buffer.Length).ToByteArray(), 0, result, offset, bufferSize);
                    offset += bufferSize;
                }

                Array.Copy(buffer, 0, result, offset, buffer.Length);

                offset += buffer.Length;
            }
            return result;
        }


        return null;
    }

    public static T ByteArray2Struct<T>(this byte[] originArray)
    {
        Type resultType = typeof(T);
        if (!resultType.IsStruct()) return default;

        FieldInfo[] allFields = resultType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        allFields = allFields.OrderBy(currentField => Marshal.OffsetOf(resultType, currentField.Name).ToInt32()).ToArray();

        T result = Activator.CreateInstance<T>();

        int offset = 0;

        foreach (FieldInfo current in allFields)
        {
            int size = current.FieldType.GetSize();
            byte[] buffer = new byte[size];
            Array.Copy(originArray, offset, buffer, 0, size);

            if (current.FieldType == typeof(string))
            {
                short stringLength = (short)buffer.FromByteArray(typeof(short));
                offset += size;
                buffer = new byte[stringLength];
                Array.Copy(originArray, offset, buffer, 0, stringLength);
                size = stringLength;
            }

            object value = buffer.FromByteArray(current.FieldType);
            current.SetValue(result, value);
            offset += size;
        }


        return result;
    }


    public static int GetSize(this Type checkType)
    {
        if (checkType == typeof(int) || checkType.IsEnum || checkType == typeof(float))
            return 4;
        else if (checkType == typeof(long) || checkType == typeof(double))
            return 8;
        else if (checkType == typeof(short))
            return 2;
        else if (checkType == typeof(bool) || checkType == typeof(byte))
            return 1;
        else if (checkType == typeof(string))
        {
            // ���̰� ���� �� ������ ���� ���̰� �ʿ���.
            // TODO ����Ʈ �� �ּ� �ִ� �� �����;
            return 2;
        }
        else
        {
            try
            {
                // Marshal�� SizeOf
                // ������ ���ſ� �������� �� ���
                // �޸𸮸� ���� �����ϱ� ������, Ȥ�� �� ���輺�� �����ϹǷ� ����ó�� ��.
                return Marshal.SizeOf(checkType);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return 0;
            }
        }
    }


    public static byte[] ToByteArray(this object target)
    {
        Type targetType = target.GetType();

        if (targetType == typeof(int) || targetType.IsEnum)
        {
            Unions.instance.integer = (int)target;
            return Unions.instance.Bytes;
        }
        else if (targetType == typeof(float))
        {
            Unions.instance.real = (float)target;
            return Unions.instance.Bytes;
        }
        else if (targetType == typeof(short))
        {
            Unions.instance.short0 = (short)target;
            return new byte[] { Unions.instance.byte0, Unions.instance.byte1 };
        }
        else if (targetType == typeof(byte))
        {
            return new byte[] { (byte)target };
        }
        else if (targetType == typeof(bool))
        {
            return new byte[] { (bool)target ? (byte)1 : (byte)0 };
        }
        else if (targetType == typeof(string))
        {
            return Encoding.UTF8.GetBytes((string)target);
        }
        else if (targetType == typeof(double))
        {
            return BitConverter.GetBytes((double)target);
        }
        else if (targetType == typeof(long))
        {
            return BitConverter.GetBytes((long)target);
        }
        else
        {
            return null;
        }
    }


    public static object FromByteArray(this byte[] originArray, Type targetType)
    {
        if (targetType == typeof(int) || targetType.IsEnum)
        {
            Unions.instance.Bytes = originArray;
            return Unions.instance.integer;
        }
        else if (targetType == typeof(float))
        {
            Unions.instance.Bytes = originArray;
            return Unions.instance.real;
        }
        else if (targetType == typeof(short))
        {
            Unions.instance.Bytes = originArray;
            return Unions.instance.short0;
        }
        else if (targetType == typeof(byte))
        {
            Unions.instance.Bytes = originArray;
            return Unions.instance.byte0;
        }
        else if (targetType == typeof(bool))
        {
            Unions.instance.Bytes = originArray;
            return Unions.instance.byte0 != 0 ? true : false;
        }
        else if (targetType == typeof(string))
        {
            return Encoding.UTF8.GetString(originArray);
        }
        else if (targetType == typeof(double))
        {
            return BitConverter.ToDouble(originArray);
        }
        else if (targetType == typeof(long))
        {
            return BitConverter.ToInt64(originArray);
        }
        else
        {
            return null;
        }
    }


    // Primitive : ���� Ÿ�� (int float bool �� )
    // �� VisualScripting�� ������ �Ȱ��� ������ �Լ��� ���� , �ٵ� ����Ƽ�� �Լ� �̸��� ��ġ�� ��찡 ���� ����� �����Ѵٰ� �� ( ������ �ǰ� ) 
    public static bool IsStruct(this Type checkType) => checkType.IsValueType && !checkType.IsPrimitive && !checkType.IsEnum;

    public static bool IsStruct<T>() => IsStruct(typeof(T));
}
