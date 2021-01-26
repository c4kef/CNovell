using UnityEngine;
using UnityEditor;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class DelayNode : BaseNode
    {
        [SerializeField] private float m_delayTime = 0; 

        #region getters

        public float GetDelayTime() => m_delayTime; 

        #endregion

#if UNITY_EDITOR
        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Задержка";

            m_rectangle.width = 150;
            m_rectangle.height = 65;

            AddOutputPoint(); 
        }
        
        public override void Copy(BaseNode node, Vector2 position)
        {
            base.Copy(node, position);

            DelayNode delayNode = node as DelayNode;
            
            m_delayTime = delayNode.m_delayTime;
        }
        
        protected override void DrawNodeWindow(int id)
        {
            EditorGUILayout.LabelField("Время задержки (сек.)");
            m_delayTime = EditorGUILayout.FloatField(m_delayTime);

            base.DrawNodeWindow(id);
        }

#endif
    }
}