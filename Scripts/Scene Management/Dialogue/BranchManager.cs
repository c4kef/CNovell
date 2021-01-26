using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CNovell.Components;

namespace CNovell.SceneManagement
{
	public class BranchManager
	{
		private readonly GameObject m_branchPrefab;
		private readonly GameObject m_branchPanel;
		private readonly GameObject m_sliderTimer;
		private readonly Transform m_branchContent;
		private BranchComponent m_branchComponent;

		public BranchManager(BranchComponent branchComponent)
		{
			
			m_branchPrefab = branchComponent.GetBranchPrefab();
			m_branchPanel = branchComponent.GetBranches();
			m_sliderTimer = branchComponent.GetTimerSlider();
			m_branchContent = branchComponent.GetBranchContent();
			m_branchComponent = branchComponent;
		}
		
		public void DisplayChoices(List<string> branches, float timerWait, bool nextNode = false)
		{
			List<int> branchIndexes = new List<int>();
			for (int i = 0; i < branches.Count; i++)
			{
				GameObject branch = Object.Instantiate(m_branchPrefab, m_branchContent); 
				Button branchButton = branch.GetComponent<Button>(); 

				Text branchText = branch.GetComponentInChildren<Text>();
				branchText.text = branches[i];
				
				int branchIndex = i;
				
				branchButton.onClick.AddListener(() => { ClearChoices(); });
				branchButton.onClick.AddListener(() => { m_branchComponent.DeInitTimer(); });

				SceneManager sceneManager = SceneManager.GetInstance();
				if (sceneManager != null && nextNode)
				{
					branchIndexes.Add(branchIndex);
					branchButton.onClick.AddListener(() => sceneManager.NextNode(branchIndex));
				}
			}

			m_branchPanel.SetActive(true);
			m_branchComponent.InitTimer(timerWait, branchIndexes);
		}

		public void ClearChoices()
		{
			Button[] branches = m_branchContent.GetComponentsInChildren<Button>();

			for (int i = 0; i < branches.Length; i++)
				Object.Destroy(branches[i].gameObject);

			m_branchPanel.SetActive(false);
		}
	}
}