﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class ConditionNode : BaseNode
    {
        [SerializeField] private ValueType m_valueType;
        
        [SerializeField] private Blackboard m_blackboardA;
        [SerializeField] private Blackboard m_blackboardB;
        
        [SerializeField] private string m_currentKeyA;
        [SerializeField] private string m_currentKeyB;
        [SerializeField] private int m_keySelectionA = 0;
        [SerializeField] private int m_keySelectionB = 0;

        private string[] m_sourceOptions = { "Значение", "Доска" };
        [SerializeField] private int m_sourceSelection = 0;

        private string[] m_booleanConditions = { "Равно " };
        private string[] m_floatConditions = { "Меньше, чем", "Равно", "Больше чем" };
        [SerializeField] private int m_booleanSelection = 0;
        [SerializeField] private int m_floatSelection = 1;

        [SerializeField] private bool m_booleanValue = false;
        [SerializeField] private float m_floatValue = 0;
        [SerializeField] private string m_stringValue = "";

        #region getters

        public ValueType GetValueType() => m_valueType;
        public Blackboard GetBlackboardA() => m_blackboardA; 
        public Blackboard GetBlackboardB() => m_blackboardB; 
        public string GetKeyA() => m_currentKeyA; 
        public string GetKeyB() => m_currentKeyB; 
        public int GetSourceSelection() => m_sourceSelection; 
        public int GetBooleanSelection() => m_booleanSelection; 
        public int GetFloatSelection() => m_floatSelection; 
        public bool GetBooleanValue() => m_booleanValue; 
        public float GetFloatValue() => m_floatValue; 
        public string GetStringValue() => m_stringValue; 

        #endregion

#if UNITY_EDITOR

        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Состояние";

            m_rectangle.width = 256;
            m_rectangle.height = 58;

            AddOutputPoint(); 
            AddOutputPoint(); 
        }

        protected override void DrawNodeWindow(int id)
        {
            EditorGUIUtility.labelWidth = 16;

            EditorGUILayout.BeginHorizontal();
            bool selectionA = DrawBlackboardVariableSelection(ref m_blackboardA, ref m_currentKeyA, ref m_keySelectionA);
            EditorGUILayout.EndHorizontal();

            if (selectionA)
            {
                m_valueType = m_blackboardA.GetValueType(m_currentKeyA);
                DrawConditionPopup();
                EditorGUILayout.BeginHorizontal();
                DrawSourcePopup();

                if (m_sourceSelection == 0)
                {
                    DrawValueField();
                    EditorGUILayout.EndHorizontal();
                }
                else if (m_sourceSelection == 1)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    DrawBlackboardVariableSelection(ref m_blackboardB, ref m_currentKeyB, ref m_keySelectionB, true);
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                m_rectangle.height = lastRect.y + lastRect.height + 4;
            }

            base.DrawNodeWindow(id);
        }
        
        private bool DrawBlackboardVariableSelection(ref Blackboard blackboard, ref string key, ref int keySelection, bool limitValueType = false)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Доска");
            blackboard = EditorGUILayout.ObjectField(blackboard, typeof(Blackboard), false) as Blackboard;
            EditorGUILayout.EndVertical();

            if (blackboard == null)
                return false;

            List<string> keys = null;
            if (limitValueType)
                keys = blackboard.GetAllOfValueType(m_valueType);
            else
                keys = blackboard.GetKeys();

            
            string[] variableNames = new string[keys.Count];
            for (int i = 0; i < keys.Count; i++)
                variableNames[i] = keys[i];
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Переменная");
            keySelection = EditorGUILayout.Popup(keySelection, variableNames);
            EditorGUILayout.EndVertical();

            if (keys.Count == 0)
                return false;
            
            keySelection = Mathf.Clamp(keySelection, 0, keys.Count - 1);
            key = keys[keySelection]; 
            return true;
        }
        
        private void DrawConditionPopup()
        {
            EditorGUILayout.LabelField("Состояние");

            switch (m_valueType)
            {
                case ValueType.Boolean:
                    m_booleanSelection = EditorGUILayout.Popup(m_booleanSelection, m_booleanConditions);
                    break;

                case ValueType.Float:
                    m_floatSelection = EditorGUILayout.Popup(m_floatSelection, m_floatConditions);
                    break;

                case ValueType.String:
                    m_booleanSelection = EditorGUILayout.Popup(m_booleanSelection, m_booleanConditions);
                    break;
            }
        }
        
        private void DrawSourcePopup()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("По сравнению с");
            m_sourceSelection = EditorGUILayout.Popup(m_sourceSelection, m_sourceOptions);
            EditorGUILayout.EndVertical();
        }
        
        private void DrawValueField()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Значение");

            switch (m_valueType)
            {
                case ValueType.Boolean:
                    m_booleanValue = EditorGUILayout.Toggle(m_booleanValue);
                    break;

                case ValueType.Float:
                    m_floatValue = EditorGUILayout.FloatField(m_floatValue);
                    break;

                case ValueType.String:
                    m_stringValue = EditorGUILayout.TextField(m_stringValue);
                    break;
            }

            EditorGUILayout.EndVertical();
        }
#endif
    }
}