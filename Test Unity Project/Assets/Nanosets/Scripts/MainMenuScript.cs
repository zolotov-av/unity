using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

public class MainMenuScript: WindowBehaviour
{
	
	public bool inMenu = true;
	public GameObject mainPanel;
	public GameObject introPanel;
	public GameObject loadingPanel;
	public GameObject backgroundPanel;
	private GameObject currentPanel = null;
	
	public void showMainMenu()
	{
		Debug.Log("showMainMenu");
		inMenu = true;
		gameObject.SetActive(true);
		if ( currentPanel != null )
		{
			currentPanel.SetActive(false);
		}
		currentPanel = mainPanel;
		mainPanel.SetActive(true);
	}
	
	public void showIntro()
	{
		Debug.Log("showIntro");
		inMenu = true;
		gameObject.SetActive(true);
		if ( currentPanel != null )
		{
			currentPanel.SetActive(false);
		}
		currentPanel = introPanel;
		introPanel.SetActive(true);
	}
	
	public void ShowLoading()
	{
		Debug.Log("ShowLoading");
		inMenu = true;
		gameObject.SetActive(true);
		if ( currentPanel != null )
		{
			currentPanel.SetActive(false);
		}
		currentPanel = loadingPanel;
		loadingPanel.SetActive(true);
		backgroundPanel.SetActive(true);
	}
	
	public void EndLoading()
	{
		Debug.Log("EndLoading");
		inMenu = false;
		gameObject.SetActive(false);
	}
	
	public void HandleInput()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			if ( currentPanel != mainPanel )
			{
				showMainMenu();
				return;
			}
		}
	}
	
	public void OnNewGameButton()
	{
		//ShowLoading();
		//SceneManager.LoadScene(1);
		TabletController.LoadScene(1);
		/*
		SoundManager.instance.FadePlay("town");
		gameObject.SetActive(false);
		inMenu = false;
		Time.timeScale = 1f;
		*/
	}
	
	public void OnLoadGameButton()
	{
	}
	
	public void OnOptionsButton()
	{
	}
	
	public void OnIntroButton()
	{
		showIntro();
	}
	
	public void OnStaffButton()
	{
	}
	
	public void OnExitButton()
	{
		Application.Quit();
	}
	
}

} // namespace Nanosoft
