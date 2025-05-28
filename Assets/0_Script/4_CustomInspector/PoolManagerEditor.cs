using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.Rendering.DebugUI.Table;

// 어떤 스크립트의 커스텀 인스펙터인지 명시
[CustomEditor(typeof(PoolManager))]
public partial class PoolManagerEditor : Editor // Editor 상속
{
    public struct TableState
    {
        public SerializedProperty property;
        public int lastCount;
        public bool isOpen;

        public void Set(SerializedProperty target)
        {
            property = target;
            lastCount = target.arraySize;
        }
    }

    private TableState characterTable;
    private TableState controllerTable;
}

public partial class PoolManagerEditor : Editor // Main
{
    // 유니티의 Inspector는 모두 reflection으로 작동함.
    // 직렬화 된 오브젝트만 받을 수 있음.
    private void OnEnable()
    {
        // 모든 씬에 존재하는 상단에 선언된 오브젝트를 가진 gameObject를 뜻함. (현재는 PoolManager를 가진 모든 오브젝트를 순환하는 방식.)
        characterTable.Set(serializedObject.FindProperty("requestCharacter"));
        controllerTable.Set(serializedObject.FindProperty("requestController"));
    }
}

public partial class PoolManagerEditor : Editor // Property
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTable(ref characterTable);
        DrawTable(ref controllerTable);

        // 변경된 사항을 실제 오브젝트에 적용.
        serializedObject.ApplyModifiedProperties();
    }


    private void DrawTable(ref TableState state)
    {
        #region Title/Button
        EditorGUILayout.BeginHorizontal();

        // FoldOut은 클릭하면 반대 상태로 이동하기 때문에 반대값 반환.
        state.isOpen = EditorGUILayout.Foldout(state.isOpen, state.property.displayName);

        // EditorGUILayout.IntField : 바꾸는 순간 바로 적용.
        // EditorGUILayout.DelayedIntField : Enter키를 누르거나 다른 곳을 포커싱할 때 적용.
        int enterCount = EditorGUILayout.DelayedIntField(state.lastCount, GUILayout.Width(50.0f));
        OnCountChanged(ref state, enterCount);

        // EditorGUILayout.LabelField(property.name, EditorStyles.boldLabel);
        if (GUILayout.Button("+", GUILayout.Width(20.0f)))
        {
            OnAddButtonClick(ref state);
        }

        EditorGUILayout.EndHorizontal();
        #endregion
        #region Row

        if (state.isOpen)
        {
            for (int i = 0; i < state.lastCount; i++)
            {
                if (DrawRow(state.property.GetArrayElementAtIndex(i)))
                {
                    state.property.DeleteArrayElementAtIndex(i);
                    state.lastCount--;
                }
            }
        }

        #endregion
    }

    // 함수에 설명 다는 법
    /// <summary>
    /// 하나의 행을 그리는 함수
    /// </summary>
    /// <param name="row">해당 행에 표현할 프로퍼티</param>
    /// <returns>삭제될 경우 true 아닐 경우 false</returns>
    private bool DrawRow(SerializedProperty row)
    {
        bool isDelete = false;

        SerializedProperty wantType = row.FindPropertyRelative("wantType");
        SerializedProperty amount = row.FindPropertyRelative("amount");

        EditorGUILayout.BeginHorizontal();

        // wantType에 enum이 아닌 다른 형식이 들어와도 사용할 수 있도록 만듦.
        EditorGUILayout.PropertyField(wantType);

        amount.intValue = EditorGUILayout.DelayedIntField(amount.intValue, GUILayout.Width(40.0f));

        if (GUILayout.Button("-", GUILayout.Width(20.0f))) // 버튼이 눌리면 true를 반환, 즉 클릭 시 실행 내용을 바로 작성
            isDelete = true;

        EditorGUILayout.EndHorizontal();
        return isDelete;
    }

    private void CountChange(ref TableState state, int count)
    {

    }

    private void OnCountChanged(ref TableState state, int count)
    {
        if (state.lastCount == count) return;

        state.property.arraySize = count;

        state.lastCount = count;
    }

    private void OnAddButtonClick(ref TableState state)
    {
        OnCountChanged(ref state, state.lastCount + 1);
    }
}
