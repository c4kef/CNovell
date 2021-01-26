using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CNovell.SceneManagement;
using CNovell.ScriptableObjects;

public class StartCanvas : MonoBehaviour
{
    public void LoadScene(Scene scene) => StartCoroutine(SceneManager.GetInstance().LoadScene(scene));

    public void Start() => SceneManager.GetInstance().GetNodeEvalutor().event_NodeEvaluted += StartCanvas_event_NodeEvaluted;

    private void StartCanvas_event_NodeEvaluted(TypesNode node)
    {
        if (node == TypesNode.EndNode)
            Debug.Log("CNovell: ну а тут типо действие при окончание новеллы");
    }
}
