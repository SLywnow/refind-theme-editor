using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AutoLangSLywnow;

[RequireComponent(typeof(Dropdown))]
public class ALSLAutoDropDown : MonoBehaviour {

	public float WaitForUpdate = 1f;
	public Tpe TypeOfUpdate;
	public List<Dropdown.OptionData> keys;

	public enum Tpe { OnStart, OnUpdateWithTime };
	Dropdown dd;
	bool upds;

	void Start()
	{
		dd = GetComponent<Dropdown>();
		UpdateWord();
	}

	float timer;
	void Update()
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
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		for (int i=0; i < keys.Count;i++)
		{
			string txt = ALSL_Main.GetWord(keys[i].text);
			txt = ALSL_Main.Replace(txt);
			options.Add(new Dropdown.OptionData());
			options[i].text = txt;
		}

		dd.options = options;

	}
}
