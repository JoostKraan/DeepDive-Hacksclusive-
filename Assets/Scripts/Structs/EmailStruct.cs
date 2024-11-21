using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EmailStruct {
    public string Sender;
    public string Company;
    public bool TrustWorthyCompany;
    public EmailHandler.Email EmailData;
    public EmailHandler.File FileType;
    public GameObject Prefab;

    public EmailStruct(string _Sender, string _Company, bool _TrustWorthyCompany, EmailHandler.Email _EmailData, EmailHandler.File _FileType, GameObject _Prefab) {
        this.Sender = _Sender;
        this.Company = _Company;
        this.TrustWorthyCompany = _TrustWorthyCompany;
        this.EmailData = _EmailData;
        this.FileType = _FileType;
        this.Prefab = _Prefab;
    }
}
