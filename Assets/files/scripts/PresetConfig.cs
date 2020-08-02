using SLywnow;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PresetConfig : MonoBehaviour
{
    public MakePreset sc;
	public List<string> text;
	/*
	 * <<bi>> - big_icon_size
	 * <<si>> - small_icon_size
	 * <<st>> - showtools
	 */

	public void Open()
	{
		sc.config.configobj.SetActive(true);
		int bi= SaveSystemAlt.GetInt("Gconf_bi", 172);
		int si = SaveSystemAlt.GetInt("Gconf_si", 72);
		bool st = SaveSystemAlt.GetBool("Gconf_shd", true);
		bool rb = SaveSystemAlt.GetBool("Gconf_rb", true);
		bool ex = SaveSystemAlt.GetBool("Gconf_ex", false);
		if (FilesSet.CheckFile(sc.curProj.dir + "/theme.conf", false))
		{
			List<string> conf = FilesSet.LoadStream(sc.curProj.dir + "/theme.conf", false).ToList();
			for (int c = 0; c < conf.Count; c++)
			{
				if (!((conf[c].IndexOf("#")) >= 0) && (conf[c].IndexOf("big_icon_size ") >= 0) && int.Parse(conf[c].Replace("big_icon_size ", ""))>0) 
					bi = int.Parse(conf[c].Replace("big_icon_size ", ""));
				if (!((conf[c].IndexOf("#")) >= 0) && (conf[c].IndexOf("small_icon_size ") >= 0)&& int.Parse(conf[c].Replace("small_icon_size ", ""))>0) 
					si = int.Parse(conf[c].Replace("small_icon_size ", ""));
				if (!((conf[c].IndexOf("#")) >= 0) && (conf[c].IndexOf("showtools ")>=0)) 
				{
					string work = conf[c].Replace("showtools ", "");
					string[] works = work.Split(',');
					bool stch = false; bool rbch = false; bool exch = false;
					for (int i=0;i<works.Length;i++)
					{
						if (works[i] == "shutdown") { st = true; stch = true; }
						if (works[i] == "reboot") { rb = true; rbch = true; }
						if (works[i] == "exit") { ex = true; exch = true; }
					}
					if (!stch) st = false;
					if (!rbch) rb = false;
					if (!exch) ex = false;
				}
				else if (conf[c].IndexOf("#no showtools") >= 0) 
				{
					st = false;
					rb = false;
					ex = false;
				}
			}
		}

		sc.config.bigicon.text = bi + "";
		sc.config.smallicon.text = si + "";
		sc.config.shutdown.isOn = st;
		sc.config.reboot.isOn = rb;
		sc.config.exit.isOn = ex;
	}

		public void Save()
	{
		List<string> tosave = new List<string>();
		for (int i=0;i< text.Count;i++)
		{
			string save="";
			if (text[i] == "<<bi>>") save = "big_icon_size " + sc.config.bigicon.text;
			else if (text[i] == "<<si>>") save = "small_icon_size " + sc.config.smallicon.text;
			else if (text[i] == "<<st>>")
			{
				if (sc.config.shutdown.isOn || sc.config.reboot.isOn || sc.config.exit.isOn)
				{
					save = "showtools ";
					bool firsthave=false;
					if (sc.config.shutdown.isOn)
					{
						save += "shutdown";
						firsthave = true;
					}
					if (sc.config.reboot.isOn)
					{
						if (firsthave)
							save += ",";
						save += "reboot";
						firsthave = true;
					}
					if (sc.config.exit.isOn)
					{
						if (firsthave)
							save += ",";
						save += "exit";
						firsthave = true;
					}
				}
				else
					save = "#no showtools";
			}
			else
				save = text[i];
			tosave.Add(save);
		}

		FilesSet.SaveStream(sc.curProj.dir + "/theme.conf", tosave.ToArray());
		sc.config.configobj.SetActive(false);
	}

	public void Cancel()
	{
		sc.config.configobj.SetActive(false);
	}
}

