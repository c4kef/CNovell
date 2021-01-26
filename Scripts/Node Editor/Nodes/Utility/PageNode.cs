using UnityEngine;
using UnityEditor;
using CNovell.Editor;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class PageNode : BaseNode
    {
        [SerializeField] private int m_pageNumber; 
        #region getter

        public int GetPageNumber() => m_pageNumber; 

        #endregion

#if UNITY_EDITOR

        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Страница";

            m_rectangle.width = 120;
            m_rectangle.height = 65;
        }
        
        public override void Copy(BaseNode node, Vector2 position)
        {
            base.Copy(node, position);

            PageNode pageNode = node as PageNode;
            
            m_pageNumber = pageNode.m_pageNumber;
        }

        protected override void DrawNodeWindow(int id)
        {
            EditorGUILayout.LabelField("Переход на:");
            
            string[] pages = new string[NodeEditor.GetScene().GetPages().Count];
            for (int i = 0; i < pages.Length; i++)
                pages[i] = $"Страница {(i + 1)}";
            
            m_pageNumber = EditorGUILayout.Popup(m_pageNumber, pages);

            m_pageNumber = Mathf.Clamp(m_pageNumber, 0, pages.Length - 1);

            base.DrawNodeWindow(id);
        }
#endif
    }
}