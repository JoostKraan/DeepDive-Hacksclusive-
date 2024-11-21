using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WifiManager : MonoBehaviour
{
    [SerializeField]private List<GameObject> networks = new List<GameObject>();
    [SerializeField]private GameObject networkPrefab;
    [SerializeField]private string[] networknames = { "eduroam", "noorderpoort", "NP_loT", "publicroam", "free robux" };
    [SerializeField]private Transform networkContainer;
    [SerializeField]private bool isConnected = false;
    [SerializeField]private bool vpn = false;
    [SerializeField]private TextMeshProUGUI currentConnection;
    [SerializeField]private GameObject connectedIcon, notConnectedIcon;
    [SerializeField]private GameObject wifiDisconnectedStat, wifiConnectedStat;
    [SerializeField]private GameObject activeVpnIcon, inactiveVpnIcon;

    void Start()
    {
        StartCoroutine(vpnCounter());
        StartCoroutine(DisconnectionCounter());
    }

    private void Update()
    {
        if (isConnected)
        {
            connectedIcon.SetActive(true);
            notConnectedIcon.SetActive(false);
            
            wifiDisconnectedStat.SetActive(false);
            wifiConnectedStat.SetActive(true);
        }
        else
        {
            connectedIcon.SetActive(false);
            notConnectedIcon.SetActive(true);
            wifiDisconnectedStat.SetActive(true);
            wifiConnectedStat.SetActive(false);
        }
    }

    public void RefreshNetworks()
    {
        foreach (var network in networks)
        {
            Destroy(network);
        }
        networks.Clear();
        Vector3 startPosition = Vector3.zero;
        for (int i = 0; i < networknames.Length; i++)
        {
            GameObject newNetwork = Instantiate(networkPrefab, networkContainer);

            newNetwork.transform.localPosition = startPosition + new Vector3(0, -100 * i, 0);
            string networkName = networknames[i];
            newNetwork.name = networkName;
            newNetwork.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = networkName;
            newNetwork.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnNetworkClicked(networkName));
            networks.Add(newNetwork);
        }
    }

    public void OnNetworkClicked(string networkName)
    {
        isConnected = true;

        if (currentConnection != null)
        {
            currentConnection.text = $"Connected to: {networkName}";
        }
    }

    public void Disconnect()
    {
        isConnected = false;
        currentConnection.text = "NO CONNECTION";
    }

    public void ActivateVPN()
    {
        vpn = !vpn;

        if (vpn)
        {
            activeVpnIcon.SetActive(true);
            inactiveVpnIcon.SetActive(false);
        }
        if (!vpn)
        {
            activeVpnIcon.SetActive(false);
            inactiveVpnIcon.SetActive(true);
        }
    }

    IEnumerator vpnCounter()
    {
        while (true)
        {
            if (vpn)
            {
                yield return new WaitForSeconds(5);
                vpn = false;
                activeVpnIcon.SetActive(false);
                inactiveVpnIcon.SetActive(true);
            }
            yield return null;
        }
    }
    IEnumerator DisconnectionCounter()
    {
        while (true)
        {
            if (isConnected)
            {
                yield return new WaitForSeconds(30);
                Disconnect();
                wifiConnectedStat.SetActive(false);
                wifiDisconnectedStat.SetActive(true);
            }
            yield return null;
        }
    }
}
