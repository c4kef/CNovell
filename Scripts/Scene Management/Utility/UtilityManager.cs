using System.Collections;
using UnityEngine;

namespace CNovell.SceneManagement
{
	public class UtilityManager
	{
		public IEnumerator Delay(float delayTime, bool nextNode = false)
		{
			yield return new WaitForSeconds(delayTime);

			SceneManager sceneManager = SceneManager.GetInstance();
			if (sceneManager != null && nextNode)
				sceneManager.NextNode();
		}
	}
}