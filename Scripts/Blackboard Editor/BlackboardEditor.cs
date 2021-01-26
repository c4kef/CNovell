using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Editor
{
#if UNITY_EDITOR
    using UnityEditorInternal;
    public class BlackboardEditor : EditorWindow
    {
        private Blackboard m_blackboard; 
        
        private ReorderableList m_booleanList;
        private ReorderableList m_floatList;
        private ReorderableList m_stringList;

        private Vector2 m_scrollPosition = Vector2.zero; 

        [MenuItem("Window/CNovell/Доска")]
        public static void Init()
        {
            BlackboardEditor window = GetWindow<BlackboardEditor>();
            window.titleContent = new GUIContent("Доска");
        }
        
        private void OnEnable()
        {
            if (m_blackboard != null)
                InitialiseReorderableLists();
        }
        
        void OnGUI()
        {
            Blackboard previousBlackboard = m_blackboard;

            EditorGUILayout.Space();
            m_blackboard = EditorGUILayout.ObjectField("Текущая доска", m_blackboard, typeof(Blackboard), false)
                           as Blackboard; 
            if (m_blackboard != null)
            {
                
                if (previousBlackboard != m_blackboard)
                    InitialiseReorderableLists();

                DrawContent(); 
                DrawLogo(); 

                EditorUtility.SetDirty(m_blackboard); 
            }
        }

        private void DrawContent()
        {
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            
            DrawReorderableList(m_booleanList, ValueType.Boolean);
            DrawReorderableList(m_floatList, ValueType.Float);
            DrawReorderableList(m_stringList, ValueType.String);

            EditorGUILayout.EndScrollView();
        }

        private void InitialiseReorderableLists()
        {
            m_booleanList = new ReorderableList(m_blackboard.GetBooleans(), typeof(KeyValue), false, true, true, true);
            m_floatList = new ReorderableList(m_blackboard.GetFloats(), typeof(KeyValue), false, true, true, true);
            m_stringList = new ReorderableList(m_blackboard.GetStrings(), typeof(KeyValue), false, true, true, true);
        }

        private void DrawReorderableList(ReorderableList reorderableList, ValueType valueType)
        {
            List<string> keys = m_blackboard.GetAllOfValueType(valueType);

            reorderableList.drawHeaderCallback = rect =>
            {
                DrawHeader(rect, valueType);
            };
            
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                DrawElement(rect, index, keys[index], valueType);
            };
            
            reorderableList.onAddCallback = (ReorderableList list) =>
            {
                AddElement(valueType);
            };
            
            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                RemoveElement(list.index, valueType);
            };
            
            GUIStyle scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
            scrollViewStyle.margin = new RectOffset(8, 8, 8, 8);
            
            EditorGUILayout.BeginVertical(scrollViewStyle);
            reorderableList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader(Rect rect, ValueType valueType)
        {
            string label = valueType.ToString() + "s";
            EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
        }

        private void DrawElement(Rect rect, int index, string key, ValueType valueType)
        {
            
            Rect firstHalf = rect;
            firstHalf.width = firstHalf.width * 0.5f - 2;
            Rect secondHalf = firstHalf;
            secondHalf.x += secondHalf.width + 4;

            m_blackboard.SetKey(key, EditorGUI.TextField(firstHalf, key), valueType);
            
            switch (valueType)
            {
                case ValueType.Boolean: 
                    secondHalf.x = rect.width;
                    secondHalf.width = 16;
                    m_blackboard.SetValue(key, EditorGUI.Toggle(secondHalf, m_blackboard.GetValue(key).m_boolean));
                    break;

                case ValueType.Float: 
                    m_blackboard.SetValue(key, EditorGUI.FloatField(secondHalf, m_blackboard.GetValue(key).m_float));
                    break;

                case ValueType.String: 
                    m_blackboard.SetValue(key, EditorGUI.TextField(secondHalf, m_blackboard.GetValue(key).m_string));
                    break;
            }
        }
        
        private void AddElement(ValueType valueType)
        {
            string keyToAdd;

            switch (valueType)
            {
                default:
                    keyToAdd = "Boolean " + m_blackboard.NewBooleanID();
                    break;

                case ValueType.Float:
                    keyToAdd = "Float " + m_blackboard.NewFloatID();
                    break;

                case ValueType.String:
                    keyToAdd = "String " + m_blackboard.NewStringID();
                    break;
            }

            m_blackboard.AddKey(keyToAdd, valueType); 
        }
        
        private void RemoveElement(int index, ValueType valueType)
        {
            
            if (!EditorUtility.DisplayDialog("Постой!", "Ты уверен что хочешь удалить эту переменную", "Ну... да", "Конечно нет"))
                return;

            string keyToRemove;
            
            switch (valueType)
            {
                default:
                    keyToRemove = m_blackboard.GetBooleans()[index].m_key;
                    break;

                case ValueType.Float:
                    keyToRemove = m_blackboard.GetFloats()[index].m_key;
                    break;

                case ValueType.String:
                    keyToRemove = m_blackboard.GetStrings()[index].m_key;
                    break;
            }

            m_blackboard.RemoveKey(keyToRemove, valueType); 
        }

        private void DrawLogo()
        {
            float xPosLogo = GUILayoutUtility.GetLastRect().width - 80;
            float yPosLogo = GUILayoutUtility.GetLastRect().height - 36;
            float xPosText = xPosLogo - 118;
            float yPosText = yPosLogo + 40;

            GUI.color = new Color(1, 1, 1, 0.25f); 
            
            GUI.Label(new Rect(xPosText, yPosText, 300, 20), "Редактор диалогов для визуальных новелл");

            GUI.color = new Color(1, 1, 1, 1); 
        }
    }
#endif
}