using Boo.Lang.Environments;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class openFileBlock : MonoBehaviour
{
    public openfile sc;
    public Color on;
    public Color off;
    public Image bg;

    public bool folder;
    public int id;
    public string mypath;

    public void Press()
	{
        if (!(id == -1))
        {
            if (sc.state.select == mypath)
            {
                sc.CheckSelect();
            }
            else
            {
                sc.state.select = mypath;
                sc.state.folder = folder;
                sc.browser.selectid = id;
            }
        } else
		{
            sc.MoveUp();
        }      
	}

	public void FixedUpdate()
	{
        if (!(id == -1))
        {
            if (sc.state.select == mypath)
                bg.color = on;
            else
                bg.color = off;
        }
    }
}
