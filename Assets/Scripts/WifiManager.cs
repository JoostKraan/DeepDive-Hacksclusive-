using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiManager : MonoBehaviour
{
    public List<GameObject> Networks = new List<GameObject>();
    public GameObject NetworkPrefab; 
    public Transform NetworkContainer; 

    void Start()
    {
      
    }
    public void RefreshNetworks()
    {
        foreach (var network in Networks)
        {
            Destroy(network);
        }
        Networks.Clear();

       
        for (int i = 0; i < 5; i++)
        {
            GameObject newNetwork = Instantiate(NetworkPrefab, NetworkContainer);
            newNetwork.name = $"Network {i + 1}";
            newNetwork.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Network {i + 1}"; 
            newNetwork.GetComponent<RefreshButton>().Setup(OnNetworkClicked, $"Network {i + 1}");
            Networks.Add(newNetwork);
        }
    }


    private void OnNetworkClicked(string networkName)
    {
        Debug.Log($"You clicked on {networkName}");
      
    }
}
