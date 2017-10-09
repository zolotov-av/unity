using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestEvents: MonoBehaviour
{
	
	void Start()
	{
		Scene scene = SceneManager.GetActiveScene();
		Debug.Log(gameObject.name + ".Start(), scene[" + scene.name + "]");
	}
	
	void Awake()
	{
		Scene scene = SceneManager.GetActiveScene();
		Debug.Log(gameObject.name + ".Awake(), scene[" + scene.name + "]");
	}
	
	void OnDestroy()
	{
		Scene scene = SceneManager.GetActiveScene();
		Debug.Log(gameObject.name + ".OnDestroy(), scene[" + scene.name + "]");
	}
	
}
