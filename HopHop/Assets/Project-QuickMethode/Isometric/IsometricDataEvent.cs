using System;
using System.Collections.Generic;

[Serializable]
public class IsometricDataEvent
{
    public List<string> Identity = new List<string>();
    public List<string> IdentityGet = new List<string>();

    public bool DataExist => Identity == null ? false : Identity.Count == 0 ? false : true;
    public bool DataGetExist => IdentityGet == null ? false : IdentityGet.Count == 0 ? false : true;
}