using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class EndNode : BaseNode
    {
#if UNITY_EDITOR
        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Конец";

            m_rectangle.width = 150;
            m_rectangle.height = 68;
        }
        
        public override void Copy(BaseNode node, Vector2 position) => base.Copy(node, position);
#endif
    }
}