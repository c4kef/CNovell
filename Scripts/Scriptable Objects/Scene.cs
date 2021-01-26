using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.Nodes;
using CNovell.Editor;

namespace CNovell.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Новая сцена", menuName = "CNovell/Сцена")]
    [System.Serializable]
    public class Scene : ScriptableObject
    {
        [HideInInspector]
        [SerializeField] private int m_currentPage = 0;

        [HideInInspector]
        [SerializeField] private List<Page> m_pages = new List<Page>();

        #region getters

        public int GetCurrentNodeID() => m_pages[m_currentPage].GetCurrentNodeID(); 
        public int GetCurrentPageID() => m_currentPage; 
        public Page GetCurrentPage() => m_pages[m_currentPage]; 
        public List<Page> GetPages() => m_pages; 
        public List<BaseNode> GetNodes(int pageNumber) => m_pages[pageNumber].GetNodes(); 
        public List<DialogueNode> GetDialogueNodes(int pageNumber) 
        { 
            var list = new List<DialogueNode>();

            foreach (var node in m_pages[pageNumber].GetNodes())
                if (node as DialogueNode != null)
                    list.Add(node as DialogueNode); 

            return list;
        }

        #endregion

        #region setters

        public void SetCurrentNodeID(int currentID) => m_pages[m_currentPage].SetCurrentNodeID(currentID); 
        public void SetCurrentPage(int currentPage) => m_currentPage = currentPage; 

        #endregion

#if UNITY_EDITOR
        public void Init()
        {
            if (m_pages.Count == 0)
                NewPage();
        }
        
        public void NewPage()
        {
            Page newPage = CreateInstance<Page>(); 

            Undo.RegisterCreatedObjectUndo(newPage, "Новая страница");

            string path = AssetDatabase.GetAssetPath(this);
            AssetDatabase.AddObjectToAsset(newPage, path);
            
            newPage.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.SaveAssets();

            Undo.RecordObject(this, "Новый узелок");
            m_pages.Add(newPage); 
            m_currentPage = m_pages.Count - 1;
            newPage.Init(); 
        }

        public void DeletePage()
        {
            
            if (!EditorUtility.DisplayDialog("Подожди!", "Ты уверен что хочешь удалить столь прекрасную сцену?", "Угусь", "Не-не"))
                return;

            Page currentPage = m_pages[m_currentPage];
            List<BaseNode> nodes = currentPage.GetNodes();
            int nodeCount = nodes.Count;

            NodeManager nodeManager = NodeEditor.GetNodeManager();
            Undo.RegisterFullObjectHierarchyUndo(currentPage, "Удалить страницу");
            for (int i = nodeCount - 1; i >= 0; i--)
                nodeManager.RemoveNode(nodes[i], false);
            
            Undo.RecordObject(this, "Удалить страницу");
            m_pages.Remove(currentPage);
            
            m_currentPage = Mathf.Clamp(m_currentPage, 0, m_pages.Count - 1);

            Undo.DestroyObjectImmediate(currentPage);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}