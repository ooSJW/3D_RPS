using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.Rendering.DebugUI.Table;

// � ��ũ��Ʈ�� Ŀ���� �ν��������� ���
[CustomEditor(typeof(PoolManager))]
public partial class PoolManagerEditor : Editor // Editor ���
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
    // ����Ƽ�� Inspector�� ��� reflection���� �۵���.
    // ����ȭ �� ������Ʈ�� ���� �� ����.
    private void OnEnable()
    {
        // ��� ���� �����ϴ� ��ܿ� ����� ������Ʈ�� ���� gameObject�� ����. (����� PoolManager�� ���� ��� ������Ʈ�� ��ȯ�ϴ� ���.)
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

        // ����� ������ ���� ������Ʈ�� ����.
        serializedObject.ApplyModifiedProperties();
    }


    private void DrawTable(ref TableState state)
    {
        #region Title/Button
        EditorGUILayout.BeginHorizontal();

        // FoldOut�� Ŭ���ϸ� �ݴ� ���·� �̵��ϱ� ������ �ݴ밪 ��ȯ.
        state.isOpen = EditorGUILayout.Foldout(state.isOpen, state.property.displayName);

        // EditorGUILayout.IntField : �ٲٴ� ���� �ٷ� ����.
        // EditorGUILayout.DelayedIntField : EnterŰ�� �����ų� �ٸ� ���� ��Ŀ���� �� ����.
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

    // �Լ��� ���� �ٴ� ��
    /// <summary>
    /// �ϳ��� ���� �׸��� �Լ�
    /// </summary>
    /// <param name="row">�ش� �࿡ ǥ���� ������Ƽ</param>
    /// <returns>������ ��� true �ƴ� ��� false</returns>
    private bool DrawRow(SerializedProperty row)
    {
        bool isDelete = false;

        SerializedProperty wantType = row.FindPropertyRelative("wantType");
        SerializedProperty amount = row.FindPropertyRelative("amount");

        EditorGUILayout.BeginHorizontal();

        // wantType�� enum�� �ƴ� �ٸ� ������ ���͵� ����� �� �ֵ��� ����.
        EditorGUILayout.PropertyField(wantType);

        amount.intValue = EditorGUILayout.DelayedIntField(amount.intValue, GUILayout.Width(40.0f));

        if (GUILayout.Button("-", GUILayout.Width(20.0f))) // ��ư�� ������ true�� ��ȯ, �� Ŭ�� �� ���� ������ �ٷ� �ۼ�
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
