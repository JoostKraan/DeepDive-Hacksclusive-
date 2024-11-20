using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PanelInfo {
    public GameObject Panel;
    public string Name;
    public GameObject Element;
    public bool Minimized = false;
    public Sprite TrayIcon;

    public PanelInfo(GameObject _Panel, string _Name) {
        this.Panel = _Panel;
        this.Name = _Name;
    }
}
