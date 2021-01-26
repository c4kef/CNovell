using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class ModifyNode : BaseNode
    {
        [SerializeField] private ValueType m_valueType;

        
        [SerializeField] private Blackboard m_blackboard;

        
        [SerializeField] private string m_currentKey;
        [SerializeField] private int m_variableSelection = 0;

        
        [SerializeField] private bool m_booleanValue = false;
        [SerializeField] private int m_booleanSelection = 0;

        
        [SerializeField] private float m_floatValue = 0;
        [SerializeField] private int m_floatSelection = 0;

        
        [SerializeField] private string m_stringValue = "";

        #region getters

        public ValueType GetValueType() => m_valueType; 
        public Blackboard GetBlackboard() => m_blackboard; 
        public string GetKey() => m_currentKey; 
        public bool GetBooleanValue() => m_booleanValue; 
        public int GetBooleanSelection() => m_booleanSelection; 
        public float GetFloatValue() => m_floatValue; 
        public int GetFloatSelection() => m_floatSelection; 
        public string GetStringValue() => m_stringValue; 

        #endregion

#if UNITY_EDITOR

        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Изменение";

            m_rectangle.width = 190;
            m_rectangle.height = 95;

            AddOutputPoint(); 
        }
        
        protected override void DrawNodeWindow(int id)
        {
            DrawBlackboardPopup();

            if (m_blackboard)
            {
                if (DrawVariablePopup())
                    DrawContent();
            }

            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                m_rectangle.height = lastRect.y + lastRect.height + 4;
            }

            base.DrawNodeWindow(id);
        }
        
        private void DrawBlackboardPopup()
        {
            
            EditorGUILayout.LabelField("Доска");
            m_blackboard = EditorGUILayout.ObjectField(m_blackboard, typeof(Blackboard), false) as Blackboard;
        }
        
        private bool DrawVariablePopup()
        {
            List<string> keys = m_blackboard.GetKeys();
            
            string[] variableNames = new string[keys.Count];
            for (int i = 0; i < keys.Count; i++)
                variableNames[i] = keys[i];
            
            EditorGUILayout.LabelField("Переменная");
            m_variableSelection = EditorGUILayout.Popup(m_variableSelection, variableNames);
            
            if (keys.Count == 0)
                return false;

            m_variableSelection = Mathf.Clamp(m_variableSelection, 0, keys.Count - 1);
            m_currentKey = keys[m_variableSelection]; 
            return true;
        }
        
        private void DrawContent()
        {
            m_valueType = m_blackboard.GetValueType(m_currentKey);
            EditorGUIUtility.labelWidth = 48;

            switch (m_valueType)
            {
                case ValueType.Boolean:
                    DrawBoolean();
                    break;

                case ValueType.Float:
                    DrawFloat();
                    break;

                case ValueType.String:
                    DrawString();
                    break;
            }
        }
        
        private void DrawBoolean()
        {
            EditorGUILayout.LabelField("Действие");
            
            string[] boolAction = { "Установить", "Выбрать" };
            m_booleanSelection = EditorGUILayout.Popup(m_booleanSelection, boolAction);

            if (m_booleanSelection == 0)
            {
                EditorGUILayout.LabelField("Значение:");
                
                Rect toggleRect = GUILayoutUtility.GetLastRect();
                toggleRect.x = m_rectangle.width - 20;
                m_booleanValue = EditorGUI.Toggle(toggleRect, m_booleanValue);
            }
        }
        
        private void DrawFloat()
        {
            EditorGUILayout.LabelField("Действие");
            
            string[] floatAction = { "Установить", "Прибавить" };
            m_floatSelection = EditorGUILayout.Popup(m_floatSelection, floatAction);
            EditorGUILayout.LabelField("Значение");
            m_floatValue = EditorGUILayout.FloatField("", m_floatValue);
        }
        
        private void DrawString() => m_stringValue = EditorGUILayout.TextField("Установить: ", m_stringValue);
#endif
    }
}