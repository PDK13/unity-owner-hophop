using System;

[Serializable]
public class IsometricDataFollow
{
    public string Identity = "";
    public string IdentityGet = "";

    public bool DataExist => Identity == null ? false : Identity == "" ? false : true;
    public bool DataGetExist => IdentityGet == null ? false : IdentityGet == "" ? false : true;
}