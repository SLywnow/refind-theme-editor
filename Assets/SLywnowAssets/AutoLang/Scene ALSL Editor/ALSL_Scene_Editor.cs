using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ALSL_Scene_Editor : MonoBehaviour
{
	public void Awake()
	{
		if (SceneManager.GetActiveScene().name == "ALSL Editor")
			gameObject.SetActive(false);
	}

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	public void found(string inp)
	{

	}
}
