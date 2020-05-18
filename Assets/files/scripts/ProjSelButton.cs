using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjSelButton : MonoBehaviour
{
    public int i;
    public MakePreset sc;
    public void Run()
    {
        sc.GetProject(i);
    }
}
