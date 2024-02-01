using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AutoLangSLywnow;

[RequireComponent(typeof(Text))]
public class ALSLText : MonoBehaviour {

	public bool onlyReplacement;
	public string key;
	public float WaitForUpdate=1f;
	public Tpe TypeOfUpdate;
	
	public enum Tpe {OnStart, OnUpdateWithTime };
	Text text;
	bool upds;

	void Start ()
	{
		text = GetComponent<Text>();
		UpdateWord();
	}

	float timer;
	void Update ()
	{
		if (upds == ALSL_Main.forseupdateall)
		{
			UpdateWord();
			timer = 0;
			upds = !upds;
		}

		if (TypeOfUpdate == Tpe.OnUpdateWithTime)
		{
			if (timer < WaitForUpdate) timer += Time.deltaTime;
			else { UpdateWord(); timer = 0; }
		}
	}

	void UpdateWord()
	{
		string txt = key;

		if (!onlyReplacement)
			txt = ALSL_Main.GetWord(key);

		txt = ALSL_Main.Replace(txt);

		text.text = txt;

	}
}
