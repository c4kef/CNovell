using UnityEngine;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class StartNode : BaseNode
    {
#if UNITY_EDITOR
        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Старт";

            m_rectangle.position = new Vector2(4, 32);
            m_rectangle.width = 100;
            m_rectangle.height = 50;

            AddOutputPoint(); 
        }

        protected override void ProcessContextMenu()
        {
            return;
        }
#endif
    }
}