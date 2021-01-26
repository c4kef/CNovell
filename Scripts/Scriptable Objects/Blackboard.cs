using System.Collections.Generic;
using UnityEngine;

namespace CNovell.ScriptableObjects
{
    public enum ValueType
    {
        Boolean,
        Float,
        String
    }
    
    [System.Serializable]
    public class KeyValue
    {
        public string m_key; 
        public Value m_value; 
        public ValueType m_valueType; 
        public KeyValue(string key, ValueType valueType)
        {
            m_key = key;
            m_valueType = valueType;
            
            m_value.m_boolean = false;
            m_value.m_float = 0.0f;
            m_value.m_string = "";
        }

        public KeyValue(KeyValue keyValue)
        {
            m_key = keyValue.m_key;
            m_valueType = keyValue.m_valueType;
            m_value = keyValue.m_value;
        }
    }
    
    [System.Serializable]
    public struct Value
    {
        public bool m_boolean;
        public float m_float;
        public string m_string;
    }

    [CreateAssetMenu(fileName = "Новая доска", menuName = "CNovell/Доска")]
    [System.Serializable]
    public class Blackboard : ScriptableObject
    {
        [HideInInspector]
        [SerializeField] private int m_blackboardID;
        [SerializeField] private List<KeyValue> m_booleans = new List<KeyValue>();
        [SerializeField] private List<KeyValue> m_floats = new List<KeyValue>();
        [SerializeField] private List<KeyValue> m_strings = new List<KeyValue>();
        [HideInInspector]
        [SerializeField] private int m_booleanID = 0;
        [HideInInspector]
        [SerializeField] private int m_floatID = 0;
        [HideInInspector]
        [SerializeField] private int m_stringID = 0;

        #region getters

        public int GetBlackboardID() => m_blackboardID; 
        public List<KeyValue> GetBooleans() => m_booleans; 
        public List<KeyValue> GetFloats() => m_floats; 
        public List<KeyValue> GetStrings() => m_strings; 
        public int NewBooleanID() => m_booleanID++; 
        public int NewFloatID() => m_floatID++; 
        public int NewStringID() => m_stringID++; 

        #endregion
        
        public void Copy(Blackboard blackboard)
        {
            m_blackboardID = blackboard.GetInstanceID();

            for (int i = 0; i < blackboard.m_booleans.Count; i++)
                m_booleans.Add(new KeyValue(blackboard.m_booleans[i]));

            for (int i = 0; i < blackboard.m_floats.Count; i++)
                m_floats.Add(new KeyValue(blackboard.m_floats[i]));

            for (int i = 0; i < blackboard.m_strings.Count; i++)
                m_strings.Add(new KeyValue(blackboard.m_strings[i]));

            m_booleanID = blackboard.m_booleanID;
            m_floatID = blackboard.m_floatID;
            m_stringID = blackboard.m_stringID;
        }
        
        public void AddKey(string key, ValueType valueType)
        {
            KeyValue keyValue = new KeyValue(key, valueType);

            switch (valueType)
            {
                case ValueType.Boolean:
                    m_booleans.Add(keyValue);
                    break;

                case ValueType.Float:
                    m_floats.Add(keyValue);
                    break;

                case ValueType.String:
                    m_strings.Add(keyValue);
                    break;
            }
        }
        
        public void SetKey(string target, string newKey, ValueType valueType)
        {
            if (target != newKey && IsKeyTaken(newKey))
            {
                Debug.LogWarning("CNovell: Попытка создать две переменные с одинаковым именем!");
                return;
            }

            switch (valueType)
            {
                case ValueType.Boolean:
                    for (int i = 0; i < m_booleans.Count; i++)
                    {
                        if (m_booleans[i].m_key == target)
                            m_booleans[i].m_key = newKey;
                    }
                    break;

                case ValueType.Float:
                    for (int i = 0; i < m_floats.Count; i++)
                    {
                        if (m_floats[i].m_key == target)
                            m_floats[i].m_key = newKey;
                    }
                    break;

                case ValueType.String:
                    for (int i = 0; i < m_strings.Count; i++)
                    {
                        if (m_strings[i].m_key == target)
                            m_strings[i].m_key = newKey;
                    }
                    break;
            }
        }

        public bool IsKeyTaken(string key)
        {
            for (int i = 0; i < m_booleans.Count; i++)
            {
                if (m_booleans[i].m_key == key)
                    return true;
            }

            for (int i = 0; i < m_floats.Count; i++)
            {
                if (m_floats[i].m_key == key)
                    return true;
            }

            for (int i = 0; i < m_strings.Count; i++)
            {
                if (m_strings[i].m_key == key)
                    return true;
            }

            return false;
        }

        public void SetValue(string key, bool value)
        {
            for (int i = 0; i < m_booleans.Count; i++)
            {
                if (m_booleans[i].m_key == key)
                {
                    KeyValue keyValue = new KeyValue(key, ValueType.Boolean);
                    keyValue.m_value.m_boolean = value;

                    m_booleans[i] = keyValue; 
                }
            }
        }

        public void SetValue(string key, float value)
        {
            for (int i = 0; i < m_floats.Count; i++)
            {
                if (m_floats[i].m_key == key)
                {
                    KeyValue keyValue = new KeyValue(key, ValueType.Float);
                    keyValue.m_value.m_float = value;

                    m_floats[i] = keyValue; 
                }
            }
        }
        
        public void SetValue(string key, string value)
        {
            for (int i = 0; i < m_strings.Count; i++)
            {
                if (m_strings[i].m_key == key)
                {
                    KeyValue keyValue = new KeyValue(key, ValueType.String);
                    keyValue.m_value.m_string = value;

                    m_strings[i] = keyValue; 
                }
            }
        }

        public Value GetValue(string key)
        {
            Value value = new Value();

            for (int i = 0; i < m_booleans.Count; i++)
            {
                if (m_booleans[i].m_key == key)
                    value = m_booleans[i].m_value; 
            }

            for (int i = 0; i < m_floats.Count; i++)
            {
                if (m_floats[i].m_key == key)
                    value = m_floats[i].m_value; 
            }

            for (int i = 0; i < m_strings.Count; i++)
            {
                if (m_strings[i].m_key == key)
                    value = m_strings[i].m_value; 
            }

            return value;
        }

        public ValueType GetValueType(string key)
        {
            ValueType valueType = ValueType.String;

            for (int i = 0; i < m_booleans.Count; i++)
            {
                if (m_booleans[i].m_key == key)
                    valueType = m_booleans[i].m_valueType; 
            }

            for (int i = 0; i < m_floats.Count; i++)
            {
                if (m_floats[i].m_key == key)
                    valueType = m_floats[i].m_valueType; 
            }

            return valueType;
        }

        public void RemoveKey(string key, ValueType valueType)
        {
            List<KeyValue> listToSearch = null;

            switch (valueType)
            {
                case ValueType.Boolean:
                    listToSearch = m_booleans;
                    break;

                case ValueType.Float:
                    listToSearch = m_floats;
                    break;

                case ValueType.String:
                    listToSearch = m_strings;
                    break;
            }

            if (listToSearch == null)
                return;

            for (int i = 0; i < listToSearch.Count; i++)
            {
                if (listToSearch[i].m_key == key)
                {
                    listToSearch.Remove(listToSearch[i]);
                    return;
                }
            }
        }
        
        public List<string> GetKeys()
        {
            List<string> outputKeys = new List<string>();

            for (int i = 0; i < m_booleans.Count; i++)
                outputKeys.Add(m_booleans[i].m_key);

            for (int i = 0; i < m_floats.Count; i++)
                outputKeys.Add(m_floats[i].m_key);

            for (int i = 0; i < m_strings.Count; i++)
                outputKeys.Add(m_strings[i].m_key);

            return outputKeys;
        }
        
        public List<string> GetAllOfValueType(ValueType valueType)
        {
            List<string> outputKeys = new List<string>();
            List<KeyValue> listToGet = null;

            switch (valueType)
            {
                case ValueType.Boolean:
                    listToGet = m_booleans;
                    break;

                case ValueType.Float:
                    listToGet = m_floats;
                    break;

                case ValueType.String:
                    listToGet = m_strings;
                    break;
            }

            if (listToGet != null)
            {
                for (int i = 0; i < listToGet.Count; i++)
                    outputKeys.Add(listToGet[i].m_key);
            }

            return outputKeys;
        }
    }
}