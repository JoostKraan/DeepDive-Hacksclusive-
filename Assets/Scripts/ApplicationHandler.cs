using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ApplicationHandler : MonoBehaviour {
    [SerializeField] private List<PanelInfo> Panels;
    [SerializeField] private Transform ElementParent;
    [SerializeField] private GameObject Element;

    [SerializeField] private Dictionary<string, GameObject> ElementHistory = new Dictionary<string, GameObject>();
    [SerializeField] private Canvas canvas;
    private bool IsDragging = false;
    

    public void InteractApplication(string ApplicationName) {
        foreach (PanelInfo panelInfo in Panels) {
            if (panelInfo.Name != ApplicationName) continue;
            panelInfo.Panel.SetActive(!panelInfo.Panel.active);
            if (panelInfo.Minimized || false) {
                panelInfo.Minimized = false;
            } else {
                if (panelInfo.Panel.active == false) {
                    Destroy(ElementHistory[panelInfo.Name]);
                    ElementHistory.Remove(panelInfo.Name);
                } else {
                    panelInfo.Panel.transform.parent.SetAsLastSibling();
                    ElementHistory.Add(panelInfo.Name, Instantiate(Element));
                    ElementHistory[panelInfo.Name].transform.SetParent(ElementParent);
                    ElementHistory[panelInfo.Name].GetComponent<Button>().onClick.AddListener((() => HandleTaskbarElement(ApplicationName)));
                    ElementHistory[panelInfo.Name].transform.Find("Icon").GetComponent<Image>().sprite = panelInfo.TrayIcon;
                }
            }
        }
    }
    public void MinimizeApplication(string ApplicationName) {
        foreach (PanelInfo panelInfo in Panels) {
            if (panelInfo.Name != ApplicationName) continue;
            panelInfo.Panel.SetActive(false);
            panelInfo.Minimized = true;
        }
    }

    public void HandleTaskbarElement(string ApplicationName) {
        foreach (PanelInfo panelInfo in Panels) {
            if (panelInfo.Name != ApplicationName) continue;
            panelInfo.Panel.SetActive(!panelInfo.Panel.active);
            panelInfo.Minimized = !panelInfo.Minimized;
        }
    }

    public void CloseApplication(string ApplicationName) {
        foreach (PanelInfo panelInfo in Panels) {
            if (panelInfo.Name != ApplicationName) continue;
            panelInfo.Panel.SetActive(false);
            Destroy(ElementHistory[panelInfo.Name]);
            ElementHistory.Remove(panelInfo.Name);
        }
    }
}
