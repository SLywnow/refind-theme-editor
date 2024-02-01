using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AutoLangSLywnow;

[RequireComponent(typeof(Text))]
public class ALSLAutoImage : MonoBehaviour
{

	public TpePer TypeOfContent;
	public float WaitForUpdate = 1f;
	public Tpe TypeOfUpdate;
	public string TextureProperty = "_MainTex";
	public List<ALSLAutoImage_Lang> langs;
	public enum Tpe { OnStart, OnUpdateWithTime };
	public enum TpePer { Image, Sprite, Texture };
	
	bool upds;
	List<string> langssting;

	SpriteRenderer spriteRenderer;
	Image image;
	Renderer renderer;

	void Start()
	{
		GetLangs();
		if (TypeOfContent == TpePer.Image)
			image = GetComponent<Image>();
		else if (TypeOfContent == TpePer.Sprite)
			spriteRenderer = GetComponent<SpriteRenderer>();
		else if (TypeOfContent == TpePer.Texture)
			renderer = GetComponent<Renderer>();
		UpdateImage();
	}

	float timer;
	void Update()
	{
		if (upds == ALSL_Main.forseupdateall)
		{
			GetLangs();
			UpdateImage();
			timer = 0;
			upds = !upds;
		}

		if (TypeOfUpdate == Tpe.OnUpdateWithTime)
		{
			if (timer < WaitForUpdate) timer += Time.deltaTime;
			else { UpdateImage(); timer = 0; }
		}
	}

	void UpdateImage()
	{
		if (langs.Count > 0)
		{
			int id = 0;

			if (langssting.Contains(ALSL_Main.alllangs[ALSL_Main.currentlang]))
				id = langssting.IndexOf(ALSL_Main.alllangs[ALSL_Main.currentlang]);
			else if (langssting.Contains(ALSL_Main.alllangs[ALSL_Main.deflang]))
				id = langssting.IndexOf(ALSL_Main.alllangs[ALSL_Main.deflang]);
			else id = 0;

			if (TypeOfContent == TpePer.Image)
			{
				image.sprite = langs[id].sprite;
			}
			else if (TypeOfContent == TpePer.Sprite)
			{
				spriteRenderer.sprite = langs[id].sprite;
			}
			else if (TypeOfContent == TpePer.Texture)
			{
				renderer.sharedMaterial.SetTexture(TextureProperty, langs[id].texture);
			}
		}
	}

	void GetLangs()
	{
		langssting = new List<string>();
		foreach (ALSLAutoImage_Lang l in langs)
			langssting.Add(l.lang);
	}
}

[System.Serializable]
public class ALSLAutoImage_Lang
{
	public string lang;
	public Sprite sprite;
	public Texture texture;
}