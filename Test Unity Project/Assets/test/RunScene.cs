using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunScene : MonoBehaviour
{
	public string sceneName = "";
	
	void Start()
	{
		if ( sceneName == null ) return;
		if ( sceneName == "" ) return;
		
		
		if ( sceneName != null )
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}
