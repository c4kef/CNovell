using UnityEngine;
using UnityEngine.Audio;

namespace CNovell.SceneManagement
{
	public class SettingsManager : MonoBehaviour
	{
		private static SettingsManager m_instance;

		private SceneManager m_sceneManager;
		
		[Header("Настройки диалога")]
		[Range(0, 1)]
		[SerializeField] private float m_textSpeed = 0.5f;
		[Range(0, 1)]
		[SerializeField] private float m_autoSpeed = 1;
		
		[Header("Настройки аудио")]
		[Range(0.0001f, 1)]
		[SerializeField] private float m_masterVolume = 1;
		[Range(0.0001f, 1)]
		[SerializeField] private float m_bgmVolume = 1;
		[Range(0.0001f, 1)]
		[SerializeField] private float m_ambienceVolume = 1;
		[Range(0.0001f, 1)]
		[SerializeField] private float m_sfxVolume = 1;
		[Range(0.0001f, 1)]
		[SerializeField] private float m_voiceVolume = 1;

		#region getters

		public static SettingsManager GetInstance() => m_instance; 
		public float GetTextSpeed() => m_textSpeed; 
		public float GetMasterVolume() => m_masterVolume; 
		public float GetBGMVolume() => m_bgmVolume; 
		public float GetAmbientVolume() => m_ambienceVolume; 
		public float GetSFXVolume() => m_sfxVolume; 
		public float GetVoiceVolume() => m_voiceVolume; 

		#endregion

		#region setters

		public void SetTextSpeed(float textSpeed) => m_textSpeed = textSpeed; 
		public void SetAutoSpeed(float autoSpeed) => m_autoSpeed = autoSpeed; 
		public void SetMasterVolume(float masterVolume) => m_masterVolume = masterVolume; 
		public void SetBGMVolume(float bgmVolume) => m_bgmVolume = bgmVolume; 
		public void SetAmbienceVolume(float ambienceVolume) => m_ambienceVolume = ambienceVolume; 
		public void SetSFXVolume(float sfxVolume) => m_sfxVolume = sfxVolume; 
		public void SetVoiceVolume(float voiceVolume) => m_voiceVolume = voiceVolume; 

		#endregion

        private void Awake()
		{
			m_instance = this; 

			m_sceneManager = SceneManager.GetInstance();
			Debug.Assert(m_sceneManager != null, "CNovell: Экземпляр SceneManager не существует!");
		}

		void Update()
		{
			m_sceneManager.GetDialogueManager().SetDialogueSpeed(m_textSpeed);
			m_sceneManager.GetDialogueManager().SetAutoSpeed(m_autoSpeed);

			AudioMixer audioMixer = m_sceneManager.GetAudioManager().GetAudioMixer();

			audioMixer.SetFloat("volumeMaster", Mathf.Log10(m_masterVolume) * 20); 
			audioMixer.SetFloat("volumeBGM", Mathf.Log10(m_bgmVolume) * 20); 
			audioMixer.SetFloat("volumeAmbience", Mathf.Log10(m_ambienceVolume) * 20); 
			audioMixer.SetFloat("volumeSFX", Mathf.Log10(m_sfxVolume) * 20); 
			audioMixer.SetFloat("volumeVoice", Mathf.Log10(m_voiceVolume) * 20); 
		}
	}
}