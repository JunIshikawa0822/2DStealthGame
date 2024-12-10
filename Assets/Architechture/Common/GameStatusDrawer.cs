#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(GameStatus))]
public class GameStatusDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // プロパティの描画開始
        EditorGUI.BeginProperty(position, label, property);

        // 折りたたみ可能なヘッダー
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            label
        );

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            // 各フィールドを表示
            SerializedProperty medicineProp = property.FindPropertyRelative("medicineDataArray");
            SerializedProperty handgunProp = property.FindPropertyRelative("handgunDataArray");
            SerializedProperty shotgunProp = property.FindPropertyRelative("shotgunDataArray");

            float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            position.y += lineHeight;
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                medicineProp
            );

            position.y += lineHeight;
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                handgunProp
            );

            position.y += lineHeight;
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                shotgunProp
            );

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            // ヘッダー + 各フィールド分の高さを計算
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;
        }
        else
        {
            // ヘッダーの高さのみ
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif

