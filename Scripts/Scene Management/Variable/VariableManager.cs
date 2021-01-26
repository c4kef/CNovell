using System.Collections.Generic;
using CNovell.ScriptableObjects;

namespace CNovell.SceneManagement
{
    public class VariableManager
    {
        public enum BoolComparison
        {
            EqualTo
        }

        public enum FloatComparison
        {
            LessThan,
            EqualTo,
            GreaterThan
        }

        public enum StringComparison
        {
            EqualTo
        }
        
        public void PerformModify(Blackboard blackboard, string key, bool value)
        {
            Blackboard sceneBlackboard = FindBlackboard(blackboard);
            sceneBlackboard.SetValue(key, value);
        }

        public void PerformModify(Blackboard blackboard, string key, float value)
        {
            Blackboard sceneBlackboard = FindBlackboard(blackboard);
            sceneBlackboard.SetValue(key, value);
        }
        
        public void PerformModify(Blackboard blackboard, string key, string value)
        {
            Blackboard sceneBlackboard = FindBlackboard(blackboard);
            sceneBlackboard.SetValue(key, value);
        }
        
        public bool EvaluateBlackboardVsValue(Blackboard blackboard, string key, bool value, BoolComparison boolComparison)
        {
            Blackboard sceneBlackboard = FindBlackboard(blackboard);
            Value valueA = sceneBlackboard.GetValue(key);

            return CompareBoolean(valueA.m_boolean, value, boolComparison);
        }
        
        public bool EvaluateBlackboardVsValue(Blackboard blackboard, string key, float value, FloatComparison floatComparison)
        {
            Blackboard sceneBlackboard = FindBlackboard(blackboard);
            Value valueA = sceneBlackboard.GetValue(key);

            return CompareFloat(valueA.m_float, value, floatComparison);
        }
        
        public bool EvaluateBlackboardVsValue(Blackboard blackboard, string key, string value, StringComparison stringComparison)
        {
            Blackboard sceneBlackboard = FindBlackboard(blackboard);
            Value valueA = sceneBlackboard.GetValue(key);

            return CompareString(valueA.m_string, value, stringComparison);
        }
        
        public bool EvaluateBlackboardVsBlackboard(Blackboard blackboardA, string keyA, Blackboard blackboardB, string keyB, BoolComparison boolComparison)
        {
            Blackboard sceneBlackboardA = FindBlackboard(blackboardA);
            Value valueA = sceneBlackboardA.GetValue(keyA);
            Blackboard sceneBlackboardB = FindBlackboard(blackboardB);
            Value valueB = sceneBlackboardB.GetValue(keyB);

            return CompareBoolean(valueA.m_boolean, valueB.m_boolean, boolComparison);
        }

        public bool EvaluateBlackboardVsBlackboard(Blackboard blackboardA, string keyA, Blackboard blackboardB, string keyB, FloatComparison floatComparison)
        {
            Blackboard sceneBlackboardA = FindBlackboard(blackboardA);
            Value valueA = sceneBlackboardA.GetValue(keyA);
            Blackboard sceneBlackboardB = FindBlackboard(blackboardB);
            Value valueB = sceneBlackboardB.GetValue(keyB);

            return CompareFloat(valueA.m_float, valueB.m_float, floatComparison);
        }

        public bool EvaluateBlackboardVsBlackboard(Blackboard blackboardA, string keyA, Blackboard blackboardB, string keyB, StringComparison stringComparison)
        {
            Blackboard sceneBlackboardA = FindBlackboard(blackboardA);
            Value valueA = sceneBlackboardA.GetValue(keyA);
            Blackboard sceneBlackboardB = FindBlackboard(blackboardB);
            Value valueB = sceneBlackboardB.GetValue(keyB);

            return CompareString(valueA.m_string, valueB.m_string, stringComparison);
        }

        private bool CompareBoolean(bool valueA, bool valueB, BoolComparison boolComparison)
        {
            switch (boolComparison)
            {
                default:
                    return valueA == valueB;
            }
        }

        private bool CompareFloat(float valueA, float valueB, FloatComparison floatComparison)
        {
            switch (floatComparison)
            {
                case FloatComparison.LessThan:
                    return valueA < valueB; 

                case FloatComparison.EqualTo:
                    return valueA == valueB; 

                case FloatComparison.GreaterThan:
                    return valueA > valueB; 
            }

            return false;
        }

        private bool CompareString(string valueA, string valueB, StringComparison stringComparison)
        {
            switch (stringComparison)
            {
                default:
                    return valueA == valueB;
            }
        }

        private Blackboard FindBlackboard(Blackboard blackboard)
        {
            SceneManager sceneManager = SceneManager.GetInstance();
            List<Blackboard> blackboards = sceneManager.GetBlackboards();

            for (int i = 0; i < blackboards.Count; i++)
            {
                if (blackboard.GetInstanceID() == blackboards[i].GetBlackboardID())
                    return blackboards[i]; 
            }

            return null; 
        }
    }
}