using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjPrevToggle : MonoBehaviour
{
    public enum tp { os,func };
    public tp tpe;
    public Toggle toggle;
    public int i;
    public MakePreset sc;

    /*public void SetUp()
    {
        if (toggle.isOn)
        {
            if (tpe==tp.os)
            {
                if (sc.prevopt.ostog.Contains(i)) return;
                else sc.prevopt.ostog.Add(i);
            }
            if (tpe == tp.func)
            {
                if (sc.prevopt.functog.Contains(i)) return;
                else sc.prevopt.functog.Add(i);
            }
        }
        else
        {
            if (tpe == tp.os)
            {
                if (!sc.prevopt.ostog.Contains(i)) return;
                else sc.prevopt.ostog.Remove(sc.prevopt.ostog.IndexOf(i));
            }
            if (tpe == tp.func)
            {
                if (!sc.prevopt.functog.Contains(i)) return;
                else sc.prevopt.functog.Remove(sc.prevopt.functog.IndexOf(i));
            }
        }
    }
    public void Update()
    {
        if (tpe == tp.os)
        {
            if (sc.prevopt.ostog.Contains(i)) toggle.isOn = true;
            else toggle.isOn = false;
        }
        if (tpe == tp.func)
        {
            if (sc.prevopt.functog.Contains(i)) toggle.isOn = true;
            else toggle.isOn = false;
        }
    }*/
}
