using UnityEngine;
using CNovell.Nodes;
using CNovell.ScriptableObjects;

namespace CNovell.SceneManagement
{
    public class NodeProcessor
    {
        private BaseNode m_currentNode;
        private Background m_currentBackground;

        private readonly SceneManager m_sceneManager;
        
        private readonly AudioManager m_audioManager;
        private readonly BackgroundManager m_backgroundManager;
        private readonly CharacterManager m_characterManager;
        private readonly DialogueManager m_dialogueManager;
        private readonly VariableManager m_variableManager;

        public NodeProcessor(SceneManager sceneManager)
        {
            m_sceneManager = sceneManager;
            
            m_audioManager = m_sceneManager.GetAudioManager();
            m_backgroundManager = m_sceneManager.GetBackgroundManager();
            m_characterManager = m_sceneManager.GetCharacterManager();
            m_dialogueManager = m_sceneManager.GetDialogueManager();
            m_variableManager = m_sceneManager.GetVariableManager();
        }

        public void Update(BaseNode currentNode) => m_currentNode = currentNode;

        #region node processing functions
        public void ProcessBackground(bool nextNode)
        {
            BackgroundNode backgroundNode = m_currentNode as BackgroundNode;
            Color fadeColour = backgroundNode.GetFadeColour();
            float fadeTime = backgroundNode.GetFadeTime();
            
            if (backgroundNode.GetToggleSelection() == 0) 
            {
                m_currentBackground = backgroundNode.GetBackground();
                m_sceneManager.SetCurrentBackground(m_currentBackground);
                Sprite background = m_currentBackground.GetBackground();
                bool waitForFinish = backgroundNode.GetWaitForFinish();
                m_backgroundManager.EnterBackground(background, fadeColour, fadeTime, nextNode, waitForFinish);
            }
            else 
                m_backgroundManager.ExitBackground(fadeColour, fadeTime, nextNode);
        }
        
        public void ProcessBGM()
        {
            BGMNode bgmNode = m_currentNode as BGMNode;

            switch (bgmNode.GetToggleSelection())
            {
                case 0:
                    m_audioManager.SetBGM(bgmNode.GetBGM(), bgmNode.GetAmbientAudio());
                    break;

                case 1:
                    m_audioManager.PauseBGM();
                    break;

                case 2:
                    m_audioManager.ResumeBGM();
                    break;
            }

            m_sceneManager.NextNode();
        }
        
        public void ProcessCharacter(bool nextNode)
        {
            CharacterNode characterNode = m_currentNode as CharacterNode;
            Character character = characterNode.GetCharacter();
            Sprite sprite = characterNode.GetSprite();
            float fadeTime = characterNode.GetFadeTime();
            bool waitForFinish = characterNode.GetWaitForFinish();
            
            if (characterNode.GetToggleSelection() == 0) 
            {
                float xPosition = characterNode.GetXPosition();
                bool isInvert = characterNode.GetIsInverted();
                m_characterManager.EnterCharacter(character, sprite, xPosition, fadeTime, nextNode, waitForFinish, isInvert);
            }
            else 
                m_characterManager.ExitCharacter(character, sprite, fadeTime, nextNode, waitForFinish);
        }
        
        public void ProcessCharacterTransform()
        {
            CharacterTransformer characterTransformer = m_characterManager.GetCharacterTransformer();
            Debug.Assert(characterTransformer != null, "CNovell: CharacterTransformer не найден!");

            if (m_currentNode is CharacterScaleNode)
                ScaleCharacter(characterTransformer);
            else if (m_currentNode is CharacterTranslateNode)
                TranslateCharacter(characterTransformer);
        }
        
        private void ScaleCharacter(CharacterTransformer characterTransformer)
        {
            CharacterScaleNode scaleNode = m_currentNode as CharacterScaleNode;
            GameObject character = m_characterManager.TryGetCharacter(scaleNode.GetCharacter());
            Vector2 scale = scaleNode.GetScale();

            if (scaleNode.GetIsLerp())
                m_sceneManager.StartCoroutine(characterTransformer.LerpCharacterScale(character, scale, scaleNode.GetLerpTime()));
            else
                characterTransformer.SetCharacterScale(character, scale);

            m_sceneManager.NextNode();
        }
        
        private void TranslateCharacter(CharacterTransformer characterTransformer)
        {
            CharacterTranslateNode translateNode = m_currentNode as CharacterTranslateNode;
            GameObject character = m_characterManager.TryGetCharacter(translateNode.GetCharacter());
            Vector2 position = new Vector2(translateNode.GetXPosition(), 0);

            if (translateNode.GetIsLerp())
                m_sceneManager.StartCoroutine(characterTransformer.LerpCharacterPosition(character, position, translateNode.GetLerpTime()));
            else
                characterTransformer.SetCharacterPosition(character, position);

            m_sceneManager.NextNode();
        }
        
        public void ProcessDialogue()
        {
            DialogueNode dialogueNode = m_currentNode as DialogueNode;

            Character character = dialogueNode.GetCharacter();

            m_dialogueManager.SetDialogue(character, dialogueNode.GetSprite(), dialogueNode.GetDialogue(),
                m_characterManager);
        }
        
        public void ProcessDialogueBox()
        {
            DialogueBoxNode dialogueBoxNode = m_currentNode as DialogueBoxNode;

            if (dialogueBoxNode.GetToggleSelection() == 0)
                m_dialogueManager.ToggleDialogueBox(true);
            else
                m_dialogueManager.ToggleDialogueBox(false);

            m_sceneManager.NextNode();
        }
        
        public void ProcessCondition()
        {
            ConditionNode conditionNode = m_currentNode as ConditionNode;
            Blackboard blackboardA = conditionNode.GetBlackboardA();
            string keyA = conditionNode.GetKeyA();
            bool output = true;

            if (conditionNode.GetSourceSelection() == 0)
            {
                switch (conditionNode.GetValueType())
                {
                    case ValueType.Boolean:
                        VariableManager.BoolComparison boolComparison = VariableManager.BoolComparison.EqualTo;
                        output = m_variableManager.EvaluateBlackboardVsValue(blackboardA, keyA, conditionNode.GetBooleanValue(), boolComparison);
                        break;

                    case ValueType.Float:
                        VariableManager.FloatComparison floatComparison = (VariableManager.FloatComparison)conditionNode.GetFloatSelection();
                        output = m_variableManager.EvaluateBlackboardVsValue(blackboardA, keyA, conditionNode.GetFloatValue(), floatComparison);
                        break;

                    case ValueType.String:
                        VariableManager.StringComparison stringComparison = VariableManager.StringComparison.EqualTo;
                        output = m_variableManager.EvaluateBlackboardVsValue(blackboardA, keyA, conditionNode.GetStringValue(), stringComparison);
                        break;
                }
            }
            else
            {
                Blackboard blackboardB = conditionNode.GetBlackboardB();
                string keyB = conditionNode.GetKeyB();

                switch (conditionNode.GetValueType())
                {
                    case ValueType.Boolean:
                        VariableManager.BoolComparison boolComparison = VariableManager.BoolComparison.EqualTo;
                        output = m_variableManager.EvaluateBlackboardVsBlackboard(blackboardA, keyA, blackboardB, keyB, boolComparison);
                        break;

                    case ValueType.Float:
                        VariableManager.FloatComparison floatComparison = (VariableManager.FloatComparison)conditionNode.GetFloatSelection();
                        output = m_variableManager.EvaluateBlackboardVsBlackboard(blackboardA, keyA, blackboardB, keyB, floatComparison);
                        break;

                    case ValueType.String:
                        VariableManager.StringComparison stringComparison = VariableManager.StringComparison.EqualTo;
                        output = m_variableManager.EvaluateBlackboardVsBlackboard(blackboardA, keyA, blackboardB, keyB, stringComparison);
                        break;
                }
            }

            m_sceneManager.NextNode(output ? 0 : 1);
        }
        
        public void ProcessModify()
        {
            ModifyNode modifyNode = m_currentNode as ModifyNode;
            Blackboard blackboard = modifyNode.GetBlackboard();
            string key = modifyNode.GetKey();
            Value value = blackboard.GetValue(key);

            switch (modifyNode.GetValueType())
            {
                case ValueType.Boolean:
                    bool booleanValue;

                    if (modifyNode.GetBooleanSelection() == 0)
                        booleanValue = modifyNode.GetBooleanValue(); 
                    else
                        booleanValue = !value.m_boolean; 

                    m_variableManager.PerformModify(blackboard, key, booleanValue); 
                    break;

                case ValueType.Float:
                    float floatValue;

                    if (modifyNode.GetFloatSelection() == 0)
                        floatValue = modifyNode.GetFloatValue(); 
                    else
                        floatValue = value.m_float + modifyNode.GetFloatValue(); 

                    m_variableManager.PerformModify(blackboard, key, floatValue); 
                    break;

                case ValueType.String:
                    m_variableManager.PerformModify(blackboard, key, modifyNode.GetStringValue()); 
                    break;
            }

            m_sceneManager.NextNode();
        }
        #endregion
    }
}