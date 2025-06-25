using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

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

            // Array.Sort(allFields) <- 성능적으로 더 유리함, 하지만 메모리를 섞기 때문에 원본이 바뀜.

            // LinQ사용
            //                                            선언한 순서대로 정렬,                       주소에 들어있는 값을 int32로 받아옴 
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
                // instance객체의 current멤버 변수의 값을 가져옴.
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
        // throw는 try-catch문 안에서만 사용 가능.
        // 에디터에서 실행할 때 유니티 자체적으로 try-catch를 사용해 실행시킴
        // throw발생 시 현재 실행중이던 함수만 끝내고 콘솔창에 Exception을 보여줌 

        if (originArray.Length == 0)
            throw new Exception("[ByteArray2Struct Error] OriginArray Has Not Value");


        Type resultType = typeof(T);
        if (!resultType.IsStruct()) return default;

        FieldInfo[] allFields = resultType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        allFields = allFields.OrderBy(currentField => Marshal.OffsetOf(resultType, currentField.Name).ToInt32()).ToArray();

        object result = Activator.CreateInstance<T>();

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


        return (T)result;
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
            // 길이가 변할 수 있으니 가변 길이가 필요함.
            // TODO 바이트 별 최소 최대 값 써놓자;
            return 2;
        }
        else
        {
            try
            {
                // Marshal의 SizeOf
                // 연산이 무거워 마지막에 만 사용
                // 메모리를 직접 참조하기 때문에, 혹시 모를 위험성이 존재하므로 예외처리 함.
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


    // Primitive : 원시 타입 (int float bool 등 )
    // ↓ VisualScripting에 완전히 똑같은 내용의 함수가 있음 , 근데 유니티랑 함수 이름이 겹치는 경우가 많아 사용을 지양한다고 함 ( 개인적 의견 ) 
    public static bool IsStruct(this Type checkType) => checkType.IsValueType && !checkType.IsPrimitive && !checkType.IsEnum;

    public static bool IsStruct<T>() => IsStruct(typeof(T));


    public static T[] GetComponent<T>(this GameObject[] from) where T : Component
    {
        T[] result = new T[from.Length];
        int index = 0;
        foreach (GameObject currrent in from)
        {
            // 후위 연산자
            // C# , java : 메모리에 접근 후 접근 해제 시 증감.
            // C++ : 해당 줄이 끝난 후 한 번에 증감.

            // 전위 연산 : 메모리에 가서 즉시 더하기 1함, 후위 연산 : 메모리에서 가져와 저장 후 다시 메모리로 가져가 더하기 1함
            // 후위 연산자 : 임시 변수가 생기기 때문에 구조체나 클래스의 ++ --연산자를 오버로딩 할 때에는 전위 연산자 사용 지향.

            // for문 에서 컴파일러는 ++i와 i++이 실제 어딘가 대입되거나 순서가 중요하지 않으면 정확히 같은 코드로 변환시킴.
            result[index++] = currrent.GetComponent<T>();
        }
        return result;
    }

    public static void AddComponents<T>(this Dictionary<string, T[]> target, params GameObject[] objects)
    {
        foreach (GameObject currentObject in objects)
        {
            T currentComponent;
            if (currentObject?.TryGetComponent<T>(out currentComponent) ?? false)
            {
                string currentName = currentObject.name;
                if (target.ContainsKey(currentName))
                {
                    /*1 배열 새로 만든 후 복사
                    T[] values = new T[target[currentObject.name].Length + 1];
                    Array.Copy(target[currentObject.name], values, values.Length - 1);
                    values[^1] = currentComponent;
                    */

                    /* 2 IEnumerable사용 (list생성 후 복사)
                    // IEnumerable : 반복자를 생성할 수 있는 인터페이스
                    // 반복자 : 자료 구조를 탐색하는 역할, 배열 리스트 딕셔너리 등..
                    
                    List<T> tempList = new(target[currentName]);
                    tempList.Add(currentComponent);
                    */

                    // 3
                    if (target[currentName].Contains(currentComponent))
                        continue;

                    //List<T> asList = new(target[currentName]);
                    //asList.Add(currentComponent);
                    //target[currentName] = asList.ToArray();

                    T[] temp = new T[target[currentName].Length + 1];
                    Array.Copy(target[currentName], temp, target[currentName].Length);
                    temp[^1] = currentComponent;
                    target[currentName] = temp;

                    // Append : String의 함수처럼 결과값을 반환함, 바로 원본 배열에 요소가 추가되는 것이 아님.
                    // target[currentName].Append(currentComponent);
                }
                else
                {
                    target.Add(currentName, new T[] { currentComponent });
                }
            }
        }
    }


    public static T Get<T>(this T[] target, int index)
    {
        target.TryGet(index, out T result);

        return result;
    }


    public static bool TryGet<T>(this T[] target, int index, out T result)
    {
        if (target.Length <= index || index < 0)
        {
            result = default;
            return false;
        }

        result = target[index];
        return true;
    }

    public static T GetOrAddComponent<T>(this GameObject target) where T : Component
    {
        // visualScripting에 있는데 Unity.Engine랑 함수이름이 겹치는 경우가 많아 namespace이슈 때문에 만듦.
        return target.GetComponent<T>() ?? target.AddComponent<T>();
    }


    public static float ClampAngle(this float target, float min, float max)
    {
        if (MathF.Abs(target) > 360.0f) target %= 360.0f;

        if (MathF.Abs(target) > 180.0f) target -= 360.0f * Mathf.Sign(target);

        return Mathf.Clamp(target, min, max);
    }


    // pitch ( x축 회전 )제한을 두며 더하기
    public static Quaternion Add(this Quaternion target, Quaternion value, float minPitch = -89.9f, float maxPitch = 89.9f)
    {
        Vector3 euler = target.eulerAngles + value.eulerAngles;
        euler.x = euler.x.ClampAngle(minPitch, maxPitch);
        return Quaternion.Euler(euler);
    }


    public static Vector3 RotationHorizontal(this Vector3 target, float angle)
    {
        // angle, degree ( 360 ) => radian 2 pi
        float theta = angle * Mathf.Deg2Rad;
        float cosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);

        return new Vector3
            (
            target.x * cosTheta - target.z * sinTheta,
            target.y,
            target.x * sinTheta + target.z * cosTheta
            );
    }

    public static Vector3 RotationVertical(this Vector3 target, float angle)
    {
        float theta = angle * Mathf.Deg2Rad;
        float cosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);
        // vector x,z의 길이
        float hr = Mathf.Sqrt(target.x * target.x + target.z * target.z);

        float hrAfter = hr * cosTheta - target.y * sinTheta;
        float radiusRatio = hrAfter / hr;

        return new Vector3
            (
            target.x * radiusRatio,
            hr * sinTheta + target.y * cosTheta,
            target.z * radiusRatio
            );
    }

    public static void Degree2RadianCosSin(this float target, out float cos, out float sin)
    {
        float theta = target * Mathf.Deg2Rad;
        cos = Mathf.Cos(theta);
        sin = Mathf.Sin(theta);
    }


    public static float GetHorizontalAngle(this Vector3 target)
    {
        // Atan : 대상의 각도를 가져오는 함수.
        float atan = Mathf.Atan2(target.z, target.x);

        // Atan 반환 값은 Radian
        return atan * Mathf.Rad2Deg;
    }


    public static float GetVerticalAngle(this Vector3 target)
    {
        float atan = Mathf.Atan2(target.y, Mathf.Sqrt(target.x * target.x + target.z * target.z));
        return atan * Mathf.Rad2Deg;
    }

    public static Vector3 RotationVerticalClamped(this Vector3 target, float hor, float ver, float min = -89.9f, float max = 89.9f)
    {
        float wanted = ver + target.GetVerticalAngle();
        float clamped = Mathf.Clamp(wanted, min, max);
        return Rotation(target, hor, ver + (clamped - wanted));
    }

    public static Vector3 Rotation(this Vector3 target, float hor, float ver)
    {
        hor.Degree2RadianCosSin(out float horCosTheta, out float horSinTheta);
        ver.Degree2RadianCosSin(out float verCosTheta, out float verSinTheta);

        Vector3 result = new Vector3
            (
            target.x * horCosTheta - target.z * horSinTheta,
            0,
            target.x * horSinTheta + target.z * horCosTheta
            );

        float hr = result.magnitude;
        float hrAfter = hr * verCosTheta - target.y * verSinTheta;
        float radiusRatio = hr != 0 ? hrAfter / hr : 1.0f;

        result *= radiusRatio;
        result.y = hr * verSinTheta + target.y * verCosTheta;

        return result;
    }


    // magnitude보다 sqrMagnitude가 성능적으로 유리함.
    // 예시로 vector가 0인지 확인할 때 sqr사용.
    public static float HorizontalSquarMagitude(this Vector3 target)
        => target.x * target.x + target.z * target.z;

    public static float HorizontalMagitude(this Vector3 target) => Mathf.Sqrt(target.HorizontalSquarMagitude());

    public static Vector3 HorizontalNormalize(this Vector3 target)
    {
        target.y = 0; target.Normalize();
        return target;
    }
}