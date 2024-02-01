using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AutoLangSLywnow;
using UnityEngine.Events;
using SLywnow;

[RequireComponent(typeof(Dropdown))]
public class ALSLSelectTranslate : MonoBehaviour {

	public bool UpdateLanguages;
	public bool UpdateWithoutRestart = true;

	public UnityEvent onUpdateLanguage;

	bool upds;
	bool inUpdate;

	static Dropdown dropdw;

	void Start () {
		dropdw = GetComponent<Dropdown>();
		UpdLg();
		dropdw.onValueChanged.AddListener((int i) => ValueCh());
	}
	void Update () {
		if (upds==ALSL_Main.forseupdateall)
		{
			//Debug.Log(upds);
			UpdLg();
			UpdateLanguages = false;
			upds = !upds;
		}
		if (UpdateLanguages) { UpdLg(); UpdateLanguages = false; }
	}

	public void UpdLg()
	{
		inUpdate = true;
		dropdw.ClearOptions();
		List<Dropdown.OptionData> dp = new List<Dropdown.OptionData>();
		for (int i=0;i<ALSL_Main.alllangs.Count;i++)
		{
			dp.Add(new Dropdown.OptionData());
			dp[i].text = ALSL_Main.langsvis[i];
		}
		dropdw.AddOptions(dp);
		dropdw.value = ALSL_Main.currentlang;
		inUpdate = false;
	}

	public void ValueCh()
	{
		if (!inUpdate)
		{
			int ss = SaveSystemAlt.IsIndex();
			SaveSystemAlt.StopWorkAndClose();
			SaveSystemAlt.StartWork(StartingSLAL.SSALevel);
			SaveSystemAlt.SetInt("currentlang", dropdw.value);
			SaveSystemAlt.StopWorkAndClose();
			SaveSystemAlt.StartWork(ss);

			//Debug.Log("ok");

			if (UpdateWithoutRestart)
			{
				ALSL_Main.SetLanguage(ALSL_Main.alllangs[dropdw.value]);
				ALSL_Main.currentlang = dropdw.value;
			}

			onUpdateLanguage.Invoke();
		}
	}
}
