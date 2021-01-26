using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using CNovell.Nodes;
using CNovell.Components;
using CNovell.ScriptableObjects;

namespace CNovell.SceneManagement
{
    public class SceneManager : MonoBehaviour
    {
        private static SceneManager m_instance;

        [Header("Компоненты сцены")]
        [SerializeField] private AudioComponent m_audioComponent;
        [SerializeField] private BackgroundComponent m_backgroundComponent;
        [SerializeField] private BranchComponent m_branchComponent;
        [SerializeField] private CharacterComponent m_characterComponent;
        [SerializeField] private DialogueComponent m_dialogueComponent;
        [SerializeField] private SaveComponent m_saveComponent;

        private NodeEvaluator m_nodeEvaluator;
        private Background m_currentBackground;

        private Scene m_currentScene;
        private BaseNode m_currentNode;
        private List<BaseNode> m_sceneNodes;

        private List<Blackboard> m_blackboards;

        #region component managers

        private AudioManager m_audioManager;
        private BackgroundManager m_backgroundManager;
        private BranchManager m_branchManager;
        private CharacterManager m_characterManager;
        private DialogueManager m_dialogueManager;
        private SaveManager m_saveManager;
        private UtilityManager m_utilityManager;
        private VariableManager m_variableManager;

        #endregion

        #region getters

        public static SceneManager GetInstance() => m_instance; 
        public Scene GetCurrentScene() => m_currentScene; 
        public BaseNode GetCurrentNode() => m_currentNode; 
        public List<Blackboard> GetBlackboards() => m_blackboards; 
        public AudioManager GetAudioManager() => m_audioManager; 
        public BackgroundManager GetBackgroundManager() => m_backgroundManager; 
        public BranchManager GetBranchManager() => m_branchManager; 
        public CharacterManager GetCharacterManager() => m_characterManager; 
        public DialogueManager GetDialogueManager() => m_dialogueManager; 
        public SaveManager GetSaveManager() => m_saveManager; 
        public UtilityManager GetUtilityManager() => m_utilityManager; 
        public VariableManager GetVariableManager() => m_variableManager; 
        public Background GetCurrentBackground() => m_currentBackground; 
        public NodeEvaluator GetNodeEvalutor() => m_nodeEvaluator;

        #endregion

        public void SetBlackboards(List<Blackboard> blackboards) => m_blackboards = blackboards; 
        public void SetCurrentBackground(Background background) => m_currentBackground = background;

        private void Awake()
        {
            m_instance = this;

            Blackboard[] blackboards = Resources.FindObjectsOfTypeAll<Blackboard>();

            m_blackboards = new List<Blackboard>();
            for (int i = 0; i < blackboards.Length; i++)
            {
                Blackboard blackboard = ScriptableObject.CreateInstance<Blackboard>();
                blackboard.Copy(blackboards[i]);
                m_blackboards.Add(blackboard);
            }

            if (m_audioComponent != null)
                m_audioManager = m_audioComponent.GetAudioManager();
            if (m_backgroundComponent != null)
                m_backgroundManager = m_backgroundComponent.GetBackgroundManager();
            if (m_branchComponent != null)
                m_branchManager = m_branchComponent.GetBranchManager();
            if (m_characterComponent != null)
                m_characterManager = m_characterComponent.GetCharacterManager();
            if (m_dialogueComponent != null)
                m_dialogueManager = m_dialogueComponent.GetDialogueManager();
            if (m_saveComponent != null)
                m_saveManager = new SaveManager(this, m_saveComponent);

            m_utilityManager = new UtilityManager();
            m_variableManager = new VariableManager();

            m_nodeEvaluator = new NodeEvaluator(this);
        }
        
        public void NewScene(Scene scene)
        {
            m_currentScene = scene; 
            NewPage(0); 
        }
        
        public void NextNode(int outputIndex = 0)
        {
            
            if (outputIndex < m_currentNode.m_outputs.Count)
            {
                
                m_currentNode = GetNode(m_currentNode.m_outputs[outputIndex]);
                m_nodeEvaluator.Evaluate(m_currentNode, true);
            }
            else
                Debug.LogWarning("CNovell: Индекс слишком велик, невозможно перейти на следующий нод");
        }
        
        public void JumpToNode(BaseNode node)
        {
            m_currentNode = node;
            m_nodeEvaluator.Evaluate(m_currentNode, true);
        }
        
        public void LoadNode(BaseNode node)
        {
            m_currentNode = node;
            m_nodeEvaluator.Evaluate(m_currentNode, false);
        }
        
        public void NewPage(int pageNumber)
        {
            m_sceneNodes = m_currentScene.GetNodes(pageNumber); 
            m_currentNode = m_sceneNodes[0];
        }

        public void NewNode(BaseNode node)
        {
            m_currentNode = node;
            m_nodeEvaluator.Evaluate(node, true);
        }

        public BaseNode GetNode(int nodeID)
        {
            for (int i = 0; i < m_sceneNodes.Count; i++)
            {
                if (m_sceneNodes[i].GetNodeID() == nodeID)
                    return m_sceneNodes[i];
            }

            return null;
        }

        public IEnumerator LoadScene(string webPathToScene, string assetPathInEditor)
        {
            while (!Caching.ready)
                yield return null;

            WWW web = new WWW(webPathToScene);
            yield return web;

            if (!string.IsNullOrEmpty(web.error))
            {
                Debug.LogError($"CNovell: ошибка загрузки бандла {web.error}");
                yield break;
            }
            AssetBundle assetBundle = web.assetBundle;
            AssetBundleRequest scene = assetBundle.LoadAssetAsync(assetPathInEditor, typeof(Scene));
            yield return scene;
            m_currentScene = scene.asset as Scene;
            NewScene(m_currentScene);
        }

        public IEnumerator LoadScene(Scene scene)
        {
            NewScene(scene);
            NextNode();
            yield return null;
        }
    }
}
