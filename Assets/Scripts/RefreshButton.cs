using UnityEngine;
using UnityEngine.UI;


public class RefreshButton : MonoBehaviour
{
    private System.Action<string> onClickAction;
    private string networkName;
    public void Setup(System.Action<string> onClick, string name)
    {
        onClickAction = onClick;
        networkName = name;

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        onClickAction?.Invoke(networkName);
    }
}
