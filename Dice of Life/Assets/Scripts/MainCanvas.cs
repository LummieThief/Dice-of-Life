using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainCanvas : MonoBehaviour
{
	public static MainCanvas instance;
	public GameObject gamePanel;
	public GameObject restartPrompt;
	public Text rollText;
	public GameObject winPanel;
	public Animator dieagramAnimator;
	public AudioMixer musicMixer, effectsMixer;
	public Slider musicSlider, effectsSlider;
	public GameObject optionsPanel;
	public GameObject menuPanel;
	public GameObject pausePanel;
	public GameObject levelPanel;
	public GameObject creditsPanel;

	public bool paused;


	private Die die;

	public bool dieagramState { get; private set; } = true;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		die = FindObjectOfType<Die>();
	}

	private void Start()
	{
		if (SceneManager.GetActiveScene().buildIndex != 0)
			return;

		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
		}
		if (PlayerPrefs.HasKey("EffectVolume"))
		{
			effectsSlider.value = PlayerPrefs.GetFloat("EffectVolume");
		}

		int p = 1;
		if (PlayerPrefs.HasKey("Progress"))
		{
			p = PlayerPrefs.GetInt("Progress");
			if (p <= 0)
			{
				p = 1;
			}
		}
		Button[] levelButtons = levelPanel.GetComponentsInChildren<Button>();
		for (int i = 1; i < levelButtons.Length; i++)
		{
			levelButtons[i].gameObject.SetActive(i <= p);
		}
		Debug.Log(p);

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			OnDieagramToggleClicked();
		}

		if (Input.GetKeyDown(KeyCode.F12))
		{
			OnNextLevel();
		}

		if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
		{
			if (!paused)
			{
				OnPause();
			}
			else
			{
				OnResume();
			}
			
		}

		if (die != null && die.running)
		{
			ChangeRollText("Reset");
		}
		else
		{
			ChangeRollText("Roll");
		}
	}

	public void OnDieagramToggleClicked()
	{
		dieagramAnimator.SetTrigger("Slide");
		dieagramState = !dieagramState;
	}

	public void OnWin()
	{
		dieagramAnimator.gameObject.transform.Translate(Vector3.up * 5000);
		gamePanel.SetActive(false);
		winPanel.SetActive(true);
		PlayerPrefs.SetInt("Progress", SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void OnUnwin()
	{
		dieagramAnimator.gameObject.transform.Translate(Vector3.down * 5000);
		gamePanel.SetActive(true);
		winPanel.SetActive(false);
		FindObjectOfType<Die>().Return();
	}

	public void OnFallOut()
	{
		restartPrompt.SetActive(true);
	}

	public void OnFallIn()
	{
		restartPrompt.SetActive(false);
	}

	public void OnNextLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void OnLevelSelect()
	{
		levelPanel.SetActive(true);
		menuPanel.SetActive(false);
	}

	public void OnBackFromLevelSelect()
	{
		levelPanel.SetActive(false);
		menuPanel.SetActive(true);
	}

	public void OnRollReset()
	{
		if (die != null)
		{
			if (die.running)
			{
				die.Return();
			}
			else
			{
				die.TryRoll();
			}
		}
	}

	public void ChangeRollText(string s)
	{
		rollText.text = s;
	}

	public void ChangeMusicVolume(float newVol)
	{
		musicMixer.SetFloat("Volume", Mathf.Log10(newVol) * 20);
		PlayerPrefs.SetFloat("MusicVolume", newVol);
	}

	public void ChangeEffectsVolume(float newVol)
	{
		effectsMixer.SetFloat("Volume", Mathf.Log10(newVol) * 20);
		PlayerPrefs.SetFloat("EffectVolume", newVol);
	}

	public void OnOptionsFromMenu()
	{
		optionsPanel.SetActive(true);
		menuPanel.SetActive(false);
	}

	public void OnBackFromOptions()
	{
		optionsPanel.SetActive(false);
		if (paused)
		{
			pausePanel.SetActive(true);
		}
		else
		{
			menuPanel.SetActive(true);
		}
		
	}

	public void OnOptionsFromPause()
	{
		optionsPanel.SetActive(true);
		pausePanel.SetActive(false);
	}

	public void OnExit()
	{
		Application.Quit();
	}

	public void OnMainMenu()
	{
		Time.timeScale = 1;
		paused = false;
		SceneManager.LoadScene(0);
	}

	public void OnResume()
	{
		pausePanel.SetActive(false);
		gamePanel.SetActive(true);
		Time.timeScale = 1;
		paused = false;
		dieagramState = true;
	}

	public void OnPause()
	{
		pausePanel.SetActive(true);
		gamePanel.SetActive(false);
		Time.timeScale = 0;
		paused = true;
	}

	public void OnCredits()
	{
		creditsPanel.SetActive(true);
		menuPanel.SetActive(false);
	}

	public void OnBackFromCredits()
	{
		creditsPanel.SetActive(false);
		menuPanel.SetActive(true);
	}

	public void MouseEnter()
	{
		AudioController.instance.GetSound(5).Play();
	}

	public void MouseDown()
	{
		AudioController.instance.GetSound(6).Play();
	}

	public void GoToScene(int index)
	{
		SceneManager.LoadScene(index);
	} 
}
