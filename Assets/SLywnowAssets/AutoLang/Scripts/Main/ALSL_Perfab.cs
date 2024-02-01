using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoLangSLywnow
{
	public class ALSL_Perfab : MonoBehaviour
	{
		public TextAsset keys;
		public TextAsset options;
		public List<ALSL_Perfab_Lang> langFiles;

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}
	}

	[System.Serializable]
	public class ALSL_Perfab_Lang
	{
		public string name;
		public TextAsset asset;
	}
}