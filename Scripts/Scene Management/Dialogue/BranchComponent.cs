using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CNovell.SceneManagement;

namespace CNovell.Components
{
	public class BranchComponent : MonoBehaviour
	{
		[Header("Выборный префаб")]
		[Tooltip("Помести \"Ветку выборов\" который будет находиться по пути \"CNovell\\Prefabs\"")]
		[SerializeField] private GameObject m_branchPrefab;
		
		[Header("Выборный(е) UI элемент(ы)")]
		[Tooltip("Подключи родительскую панель, она используется для включения/отключения ветвей при выборе.")]
		[SerializeField] private GameObject m_branchPanel;
		[Tooltip("Подключи слайдер, он используется для отображения времени на выбор ответа.")]
		[SerializeField] private GameObject m_sliderTimer;
		[Tooltip("Подключи панель содержимого с прокруткой, к ней будут добавлены префабы веток.")]
		[SerializeField] private Transform m_branchContent;

		private BranchManager m_branchManager;

		#region getters

		public GameObject GetBranchPrefab() => m_branchPrefab;
		public GameObject GetBranches() => m_branchPanel;
		public GameObject GetTimerSlider() => m_sliderTimer;
		public Transform GetBranchContent() => m_branchContent;

		#endregion

		public void InitTimer(float seconds, List<int> idsBranch)
		{
			m_sliderTimer.SetActive((seconds > 0.0f));
			if (seconds > 0.0f)
				StartCoroutine(Countdown(m_sliderTimer, seconds, idsBranch));
		}
		
		public void DeInitTimer() => StopAllCoroutines();
		
		private IEnumerator Countdown(GameObject m_sliderTimer, float seconds, List<int> idsBranch)
		{
			float animationTime = 0f;
			while (animationTime < seconds)
			{
				animationTime += Time.deltaTime;
				float lerpValue = animationTime / seconds;
				Image[] percent = m_sliderTimer.GetComponentsInChildren<Image>();
				percent[percent.Length - 1].fillAmount = Mathf.Lerp(0.0f, 1.0f, lerpValue);
				yield return null;
			}

			
			SceneManager.GetInstance().NextNode(idsBranch[Random.Range(0, idsBranch.Count - 1)]);
			SceneManager.GetInstance().GetBranchManager().ClearChoices();
		}

		public BranchManager GetBranchManager()
		{
			if (m_branchManager == null)
				m_branchManager = new BranchManager(this);

			return m_branchManager;
		}
	}
}