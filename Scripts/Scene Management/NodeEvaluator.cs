using UnityEngine;
using CNovell.Nodes;

namespace CNovell.SceneManagement
{
    public enum TypesNode
    {
        BGMNode,
        SFXNode,
        BackgroundNode,
        BranchNode,
        CharacterNode,
        CharacterScaleNode,
        CharacterTranslateNode,
        DialogueNode,
        DialogueBoxNode,
        DelayNode,
        ConditionNode,
        ModifyNode,
        PageNode,
        EndNode
    }

    public class NodeEvaluator
    {
        private BaseNode m_currentNode;
        
        private readonly SceneManager m_sceneManager;
        private readonly NodeProcessor m_nodeProcessor;

        public delegate void DelegateEndNode(TypesNode node);
        public event DelegateEndNode event_NodeEvaluted;

        public NodeEvaluator(SceneManager sceneManager)
        {
            m_sceneManager = sceneManager;
            m_nodeProcessor = new NodeProcessor(sceneManager);
        }

        public void Evaluate(BaseNode currentNode, bool nextNode)
        {
            m_currentNode = currentNode;
            m_nodeProcessor.Update(currentNode);
            
            if (EvaluateAudioNode())
                return;
            else if (EvaluateBackgroundNode(nextNode))
                return;
            else if (EvaluateBranchNode())
                return;
            else if (EvaluateCharacterNode(nextNode))
                return;
            else if (EvaluateDialogueNode())
                return;
            else if (EvaluateUtilityNode())
                return;
            else if (EvaluateVariableNode())
                return;
            EvaluateTransitionNodes();
        }

        #region node evaluation functions
        private bool EvaluateAudioNode()
        {
            AudioManager audioManager = m_sceneManager.GetAudioManager();

            if (audioManager != null)
            {
                if (m_currentNode is BGMNode)
                {
                    m_nodeProcessor.ProcessBGM();
                    event_NodeEvaluted?.Invoke(TypesNode.BGMNode);
                    return true;
                }
                else if (m_currentNode is SFXNode)
                {
                    SFXNode sfxNode = m_currentNode as SFXNode;
                    m_sceneManager.StartCoroutine(audioManager.PlaySFX(sfxNode.GetSFX(), true, sfxNode.GetWaitForFinish()));
                    event_NodeEvaluted?.Invoke(TypesNode.SFXNode);
                    return true;
                }
            }
            else
            {
                if (m_currentNode is BGMNode || m_currentNode is SFXNode)
                    Debug.LogError("CNovell: SceneManager нуждается в AudioComponent, если ты используешь аудиоузлы!");
            }

            return false;
        }

        private bool EvaluateBackgroundNode(bool nextNode)
        {
            BackgroundManager backgroundManager = m_sceneManager.GetBackgroundManager();

            if (backgroundManager != null)
            {
                if (m_currentNode is BackgroundNode)
                {
                    m_nodeProcessor.ProcessBackground(nextNode);
                    event_NodeEvaluted?.Invoke(TypesNode.BackgroundNode);
                    return true;
                }
            }
            else
            {
                if (m_currentNode is BackgroundNode)
                    Debug.LogError("CNovell: SceneManager нужен BackgroundComponent, если ты используешь фоновые узлы!");
            }

            return false;
        }

        private bool EvaluateBranchNode()
        {
            BranchManager branchManager = m_sceneManager.GetBranchManager();

            if (branchManager != null)
            {
                if (m_currentNode is BranchNode)
                {
                    branchManager.DisplayChoices((m_currentNode as BranchNode).GetBranches(), (m_currentNode as BranchNode).GetTimeWait(), true);
                    event_NodeEvaluted?.Invoke(TypesNode.BranchNode);
                    return true;
                }
            }
            else
            {
                if (m_currentNode is BranchNode)
                    Debug.LogError("CNovell: SceneManager нуждается в BranchComponent!");
            }

            return false;
        }
        
        private bool EvaluateCharacterNode(bool nextNode)
        {
            CharacterManager characterManager = m_sceneManager.GetCharacterManager();

            if (characterManager != null)
            {
                if (m_currentNode is CharacterNode)
                {
                    m_nodeProcessor.ProcessCharacter(nextNode);
                    event_NodeEvaluted?.Invoke(TypesNode.CharacterNode);
                    return true;
                }
                else if (m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
                {
                    m_nodeProcessor.ProcessCharacterTransform();
                    if (m_currentNode is CharacterTranslateNode)
                        event_NodeEvaluted?.Invoke(TypesNode.CharacterTranslateNode);
                    else
                        event_NodeEvaluted?.Invoke(TypesNode.CharacterScaleNode);
                    return true;
                }
            }
            else
            {
                if (m_currentNode is CharacterNode || m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
                    Debug.LogError("CNovell: SceneManager требует CharacterComponent, если ты используешь узлы символов!");
            }

            return false;
        }
        
        private bool EvaluateDialogueNode()
        {
            DialogueManager dialogueManager = m_sceneManager.GetDialogueManager();

            if (dialogueManager != null)
            {
                if (m_currentNode is DialogueNode)
                {
                    m_nodeProcessor.ProcessDialogue();
                    event_NodeEvaluted?.Invoke(TypesNode.DialogueNode);
                    return true;
                }
                else if (m_currentNode is DialogueBoxNode)
                {
                    m_nodeProcessor.ProcessDialogueBox();
                    event_NodeEvaluted?.Invoke(TypesNode.DialogueBoxNode);
                    return true;
                }
            }
            else
            {
                if (m_currentNode is DialogueNode || m_currentNode is DialogueBoxNode)
                    Debug.LogError("CNovell: SceneManager нуждается в DialogueComponent, если ты используешь узлы диалога!");
            }

            return false;
        }

        private bool EvaluateUtilityNode()
        {
            UtilityManager utilityManager = m_sceneManager.GetUtilityManager();

            if (m_currentNode is DelayNode)
            {
                m_sceneManager.StartCoroutine(utilityManager.Delay((m_currentNode as DelayNode).GetDelayTime(), true));
                event_NodeEvaluted?.Invoke(TypesNode.DelayNode);
                return true;
            }

            return false;
        }

        private bool EvaluateVariableNode()
        {
            if (m_currentNode is ConditionNode)
            {
                m_nodeProcessor.ProcessCondition();
                event_NodeEvaluted?.Invoke(TypesNode.ConditionNode);
                return true;
            }
            else if (m_currentNode is ModifyNode)
            {
                m_nodeProcessor.ProcessModify();
                event_NodeEvaluted?.Invoke(TypesNode.ModifyNode);
                return true;
            }

            return false;
        }

        private void EvaluateTransitionNodes()
        {
            if (m_currentNode is PageNode)
            {
                PageNode pageNode = m_currentNode as PageNode;
                m_sceneManager.GetCurrentScene().SetCurrentPage((pageNode).GetPageNumber());
                m_sceneManager.NewPage((pageNode).GetPageNumber());
                m_sceneManager.NextNode();
                event_NodeEvaluted?.Invoke(TypesNode.PageNode);
            }
            else if (m_currentNode is EndNode)
                event_NodeEvaluted?.Invoke(TypesNode.EndNode);
        }
        #endregion
    }
}
