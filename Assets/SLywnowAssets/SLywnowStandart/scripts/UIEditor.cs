using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SLywnow
{
	public class UIEditor : MonoBehaviour
	{
		/// <summary>
		/// Create Sprite with single Color
		/// </summary>
		/// <param name="color">Color what you need</param>
		/// <param name="width">Width what you need, default 1</param>
		/// <param name="height">Height what you need, default 1</param>
		/// <param name="pivotX">Pivot's X cord what you need, default 0.5</param>
		/// <param name="pivotY">Pivot's Y cord what you need, default 0.5</param>
		/// <returns></returns>
		public static Sprite GetSpriteWithColor(Color color, int width = 1, int height = 1, float pivotX = 0.5f, float pivotY = 0.5f)
		{
			Texture2D imgcol = new Texture2D(width, height, TextureFormat.RGBA32, false);
			for (int x = 0; x < width; x++) for (int y = 0; y < height; y++)
					imgcol.SetPixel(x, y, color);
			imgcol.Apply();
			return Sprite.Create(imgcol, new Rect(0.0f, 0.0f, imgcol.width, imgcol.height), new Vector2(pivotX, pivotY));
		}

		/// <summary>
		/// Create Texture2D with single Color
		/// </summary>
		/// <param name="color">Color what you need</param>
		/// <param name="width">Width what you need, default 1</param>
		/// <param name="height">Height what you need, default 1</param>
		/// <returns></returns>
		public static Texture2D GetTextureWithColor(Color color, int width = 1, int height = 1)
		{
			Texture2D imgcol = new Texture2D(width, height, TextureFormat.RGBA32, false);
			for (int x = 0; x < width; x++) for (int y = 0; y < height; y++)
					imgcol.SetPixel(x, y, color);
			imgcol.Apply();
			return imgcol;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gradient">Gradient</param>
		/// <param name="Xdirection">Is gradient will be in X dir?</param>
		/// <param name="width">Width what you need, default 1</param>
		/// <param name="height">Height what you need, default 1</param>
		/// <param name="pivotX">Pivot's X cord what you need, default 0.5</param>
		/// <param name="pivotY">Pivot's Y cord what you need, default 0.5</param>
		/// <returns></returns>
		public static Sprite GetSpriteWithGradient(Gradient gradient, bool Xdirection, int width = 1, int height = 1, float pivotX = 0.5f, float pivotY = 0.5f)
		{
			Texture2D imgcol = new Texture2D(width, height, TextureFormat.RGBA32, false);
			for (int x = 0; x < width; x++) for (int y = 0; y < height; y++)
					imgcol.SetPixel(x, y, Xdirection ? gradient.Evaluate((float)x / width) : gradient.Evaluate((float)y / height));
			imgcol.Apply();
			return Sprite.Create(imgcol, new Rect(0.0f, 0.0f, imgcol.width, imgcol.height), new Vector2(pivotX, pivotY));
		}

		public static Sprite GetSpriteWithGradient2D(Gradient gradientX, Gradient gradientY, int width = 1, int height = 1, float pivotX = 0.5f, float pivotY = 0.5f)
		{
			Texture2D imgcol = new Texture2D(width, height, TextureFormat.RGBA32, false);
			for (int x = 0; x < width; x++) for (int y = 0; y < height; y++) 
				{
					Color cX = gradientX.Evaluate((float)x / width);
					Color cY = gradientY.Evaluate((float)y / height);
					Color c = (cX + cY) / 2;
					
					imgcol.SetPixel(x, y, c);
				}
			imgcol.Apply();
			return Sprite.Create(imgcol, new Rect(0.0f, 0.0f, imgcol.width, imgcol.height), new Vector2(pivotX, pivotY));
		}

		/// <summary>
		/// Replacing entering DropDown optionsList to your text list
		/// </summary>
		/// <param name="dropdown">DropDown to replace</param>
		/// <param name="strings">String list to replace</param>
		/// <param name="enter">DropDown with starting parameters</param>
		public static void FillDropDownByTextList(out Dropdown dropdown, List<string> strings, Dropdown enter)
		{
			dropdown = enter;
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int i = 0; i < strings.Count; i++)
			{
				options.Add(new Dropdown.OptionData());
				options[i].text = strings[i];
			}
			dropdown.options = options;
		}

		/// <summary>
		/// Replacing entering DropDown optionsList to your sprite list
		/// </summary>
		/// <param name="dropdown">DropDown to replace</param>
		/// <param name="sprites">Sprite list to replace</param>
		/// <param name="enter">DropDown with starting parameters</param>
		public static void FillDropDownBySpriteList(out Dropdown dropdown, List<Sprite> sprites, Dropdown enter)
		{
			dropdown = enter;
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int i = 0; i < sprites.Count; i++)
			{
				options.Add(new Dropdown.OptionData());
				options[i].image = sprites[i];
			}
			dropdown.options = options;
		}

		/// <summary>
		/// Replacing entering DropDown optionsList to your sprite and string list (index to index)
		/// </summary>
		/// <param name="dropdown">DropDown to replace</param>
		/// <param name="sprites">Sprite list to replace</param>
		/// <param name="strings">String list to replace</param>
		/// <param name="enter">DropDown with starting parameters</param>
		public static void FillDropDownBySpriteAndTextList(out Dropdown dropdown, List<Sprite> sprites, List<string> strings, Dropdown enter)
		{
			dropdown = enter;
			if (sprites.Count != strings.Count) return;

			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int i = 0; i < sprites.Count; i++)
			{
				options.Add(new Dropdown.OptionData());
				options[i].image = sprites[i];
				options[i].text = strings[i];
			}
			dropdown.options = options;
		}

		/// <summary>
		/// Replacing entering DropDown optionsList to your sprite list
		/// </summary>
		/// <param name="dropdown">DropDown to replace</param>
		/// <param name="colors">Colors list to replace</param>
		/// <param name="enter">DropDown with starting parameters</param>
		public static void FillDropDownByColorList(out Dropdown dropdown, List<Color> colors, Dropdown enter)
		{
			dropdown = enter;
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int i = 0; i < colors.Count; i++)
			{
				Texture2D imgcol = new Texture2D(1, 1, TextureFormat.RGBA32, false);
				imgcol.SetPixel(1, 1, colors[i]);
				imgcol.Apply();
				options.Add(new Dropdown.OptionData());
				options[i].image = Sprite.Create(imgcol, new Rect(0.0f, 0.0f, imgcol.width, imgcol.height), new Vector2(0.5f, 0.5f));
			}
			dropdown.options = options;
		}

		/// <summary>
		/// Replacing entering DropDown optionsList to your sprite list
		/// </summary>
		/// <param name="dropdown">DropDown to replace</param>
		/// <param name="colors">Colors list to replace</param>
		/// <param name="enter">DropDown with starting parameters</param>
		public static void FillDropDownByColorAndTextList(out Dropdown dropdown, List<Color> colors, List<string> strings, Dropdown enter)
		{
			dropdown = enter;
			if (colors.Count != strings.Count) return;

			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int i = 0; i < colors.Count; i++)
			{
				Texture2D imgcol = new Texture2D(1, 1, TextureFormat.RGBA32, false);
				imgcol.SetPixel(1, 1, colors[i]);
				imgcol.Apply();
				options.Add(new Dropdown.OptionData());
				options[i].image = Sprite.Create(imgcol, new Rect(0.0f, 0.0f, imgcol.width, imgcol.height), new Vector2(0.5f, 0.5f));
				options[i].text = strings[i];
			}
			dropdown.options = options;
		}

		/// <summary>
		/// Returns string value from UIEvent input
		/// </summary>
		/// <param name="input">input string</param>
		/// <param name="pos">position</param>
		/// <param name="space">char for space in input</param>
		/// <param name="def">default value</param>
		/// <returns></returns>
		public static string GetStringUIEvent(string input, int pos, char space, string def = null)
		{
			string output = def;

			string[] inp = input.Split(space);
			if (inp.Length > pos)
				output = inp[pos];

			return output;
		}

		/// <summary>
		/// Returns int value from UIEvent input
		/// </summary>
		/// <param name="input">input string</param>
		/// <param name="pos">position</param>
		/// <param name="space">char for space in input</param>
		/// /// <param name="def">default value</param>
		/// <returns></returns>
		public static int GetIntUIEvent(string input, int pos, char space, int def = 0)
		{
			int output = def;

			string[] inp = input.Split(space);
			if (inp.Length > pos)
				if (int.TryParse(inp[pos], out output))
					output = int.Parse(inp[pos]);

			return output;
		}

		/// <summary>
		/// Returns float value from UIEvent input
		/// </summary>
		/// <param name="input">input string</param>
		/// <param name="pos">position</param>
		/// <param name="space">char for space in input</param>
		/// /// <param name="def">default value</param>
		/// <returns></returns>
		public static float GetFloatUIEvent(string input, int pos, char space, float def = 0)
		{
			float output = def;

			string[] inp = input.Split(space);
			if (inp.Length > pos)
				if (float.TryParse(inp[pos], out output))
					output = float.Parse(inp[pos]);

			return output;
		}

		/// <summary>
		/// Returns bool value from UIEvent input
		/// </summary>
		/// <param name="input">input string</param>
		/// <param name="pos">position</param>
		/// <param name="space">char for space in input</param>
		/// /// <param name="def">default value</param>
		/// <returns></returns>
		public static bool GetBoolUIEvent(string input, int pos, char space, bool def = false)
		{
			bool output = def;

			string[] inp = input.Split(space);
			if (inp.Length > pos)
				if (bool.TryParse(inp[pos], out output))
					output = bool.Parse(inp[pos]);

			return output;
		}
	}
}
