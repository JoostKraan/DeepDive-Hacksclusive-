using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WifiManager : MonoBehaviour
{
    public List<GameObject> networks = new List<GameObject>();
    public GameObject networkPrefab;
    private string[] networknames = { "eduroam", "noorderpoort", "NP_loT", "publicroam", "free robux" };
    public Transform networkContainer;
    public bool isConnected = false;
    public bool vpn = false;
    public TextMeshProUGUI currentConnection;
    public GameObject connectedIcon, notConnectedIcon;
    public GameObject activeVpnIcon, inactiveVpnIcon;

    void Start()
    {
        StartCoroutine(vpnCounter());
    }

    private void Update()
    {
        if (isConnected)
        {
            connectedIcon.SetActive(true);
            notConnectedIcon.SetActive(false);
        }
        else
        {
            connectedIcon.SetActive(false);
            notConnectedIcon.SetActive(true);
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
            if (vpn)
            {
                Debug.Log("Connect is safe");
            }
            else
            {
                Debug.Log("Connect is unsafe");
            }
            currentConnection.text = $"Connected to: {networkName}";
            Debug.Log($"Connected to: {networkName}");
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
}
