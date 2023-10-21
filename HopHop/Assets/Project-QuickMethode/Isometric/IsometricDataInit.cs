using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

[Serializable]
public class IsometricDataInit
{
    public List<string> Data = new List<string>();

    public bool DataExist => Data == null ? false : Data.Count == 0 ? false : true;
}