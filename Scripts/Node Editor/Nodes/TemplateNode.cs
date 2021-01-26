using UnityEngine;
using UnityEditor;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class TemplateNode : BaseNode
    {
#if UNITY_EDITOR
        public override void Init(Vector2 position)
        {
            base.Init(position);
            
            m_title = "Узел";
            
            m_rectangle.width = 100;
            m_rectangle.height = 50;

            AddOutputPoint();
        }
        
        public override void Copy(BaseNode node, Vector2 position) => base.Copy(node, position);
        
        protected override void DrawNodeWindow(int id) => base.DrawNodeWindow(id);
#endif
    }
}