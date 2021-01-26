using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.Nodes;
using CNovell.ScriptableObjects;

namespace CNovell.Editor
{
#if UNITY_EDITOR
    public class NodeManager
    {
        [SerializeField] private List<BaseNode> m_nodes;
        [SerializeField] private BaseNode m_clipboard;

        #region getters

        public List<BaseNode> GetNodes() => m_nodes; 
        public BaseNode GetLastNode() => m_nodes[m_nodes.Count - 1];
        public BaseNode GetClipboard() => m_clipboard;

        #endregion

        public void Update()
        {
            Scene currentScene = NodeEditor.GetScene(); 
            m_nodes = currentScene.GetNodes(currentScene.GetCurrentPageID()); 
        }
        
        public void DrawNodes()
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                BaseNode node = m_nodes[i];
                Undo.RecordObject(node, "Изменения узлов"); 
                node.Draw(); 
            }
        }

        public BaseNode FindNode(int nodeID)
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].GetNodeID() == nodeID)
                    return m_nodes[i]; 
            }

            return null; 
        }

        public BaseNode AddNode(System.Type type, BaseNode nodeToCopy = null)
        {
            Scene currentScene = NodeEditor.GetScene(); 
            
            BaseNode node = ScriptableObject.CreateInstance(type) as BaseNode;
            if (nodeToCopy != null)
                node.Copy(nodeToCopy, NodeEditor.GetMousePosition()); 
            else
                node.Init(NodeEditor.GetMousePosition()); 

            if (type != typeof(StartNode))
            {
                Undo.RecordObject(currentScene.GetCurrentPage(), "Новый узелок"); 
                m_nodes.Add(node); 

                Undo.RegisterCreatedObjectUndo(node, "Новый узелок"); 
            }
            
            ConnectionManager connectionManager = NodeEditor.GetConnectionManager();
            BaseNode selectedLeftNode = connectionManager.GetSelectedLeftNode();
            if (selectedLeftNode != null)
            {
                connectionManager.SetSelectedRightNode(node);
                connectionManager.CreateConnection(); 
                connectionManager.ClearConnectionSelection(); 
            }

            string path = AssetDatabase.GetAssetPath(currentScene);
            AssetDatabase.AddObjectToAsset(node, path);

            node.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.SaveAssets(); 

            return node;
        }

        public void CopyNode(BaseNode node) => m_clipboard = node;

        public void PasteNode() => AddNode(m_clipboard.GetType(), m_clipboard);
        
        public void RemoveNode(BaseNode node, bool saveAssets = true)
        {
            ConnectionManager connectionManager = NodeEditor.GetConnectionManager(); 
            
            int noOfInputs = node.m_inputs.Count;
            for (int i = 0; i < noOfInputs; i++)
            {
                BaseNode connectedNode = FindNode(node.m_inputs[0]);

                int indexOfThisNode = connectedNode.m_outputs.IndexOf(node.GetNodeID());
                connectionManager.RemoveConnection(connectedNode, indexOfThisNode);
            }
            
            for (int i = 0; i < node.m_outputs.Count; i++)
            {
                if (node.m_outputs[i] == -1)
                    continue; 
                
                connectionManager.RemoveConnection(node, i);
            }
            
            Page currentPage = NodeEditor.GetScene().GetCurrentPage();
            Undo.RecordObject(currentPage, "Удалить узелок");
            m_nodes.Remove(node); 
            
            Undo.DestroyObjectImmediate(node);

            if (saveAssets)
                AssetDatabase.SaveAssets(); 
        }
    }
#endif
}