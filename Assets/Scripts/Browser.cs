using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Browser : MonoBehaviour
{
    [SerializeField] private GameObject[] windows;
    [SerializeField] private TMP_InputField inputfield;
    [SerializeField] private string temptext;
    [SerializeField] private RectTransform suggestionsPanel;
    [SerializeField] private GameObject suggestionPrefab;

    private List<string> websiteSuggestions = new List<string>
    {
        "https://www.google.com",
        "https://noorderportal.com",
        "https://www.asnbank.nl",
        "https://www.mijnnduo.nl"
    };
    private List<GameObject> suggestionObjects = new List<GameObject>();

    private string lastSuggestion = "";

    [Header("Variables")] 
    public bool IsOnValidTab = false;

    void Start()
    {
        inputfield.onValueChanged.AddListener(OnInputChanged);
        suggestionsPanel.gameObject.SetActive(false);

    }

    private void SetWindow(string activeWindow)
    {
        foreach (var window in windows)
        {
            if (window.name == activeWindow)
            {
                window.SetActive(true);
                IsOnValidTab = true;
            }
            else
            {
                window.SetActive(false); 
            }
        }
    }

    void Update()
    {
        // Check for the Enter key to process the input
        if (Input.GetKeyDown(KeyCode.Return))
        {
            temptext = inputfield.text;

            switch (temptext)
            {
                case "https://www.google.com":
                    Debug.Log("Google selected");
                    SetWindow("googletab");
                    break;
                case "https://noorderportal.com":
                    Debug.Log("NP selected");
                    SetWindow("nptab");
                    break;
                case "https://www.asnbank.nl":
                    Debug.Log("ASN selected");
                    SetWindow("asntab");
                    break;
                case "https://www.mijnnduo.nl":
                    Debug.Log("DUO selected");
                    SetWindow("duotab");
                    break;

            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!string.IsNullOrEmpty(lastSuggestion))
            {
                inputfield.text = lastSuggestion;
                inputfield.caretPosition = lastSuggestion.Length;
                suggestionsPanel.gameObject.SetActive(false);     
            }
        }
    }
    private void OnInputChanged(string userInput)
    {
        foreach (var suggestionObject in suggestionObjects)
        {
            Destroy(suggestionObject);
        }
        suggestionObjects.Clear();
        lastSuggestion = "";
        if (userInput.Length >= 3)
        {
            var filteredSuggestions = websiteSuggestions
                .Where(website => website.IndexOf(userInput, System.StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (filteredSuggestions.Count == 1)
            {
                lastSuggestion = filteredSuggestions[0];
            }

            if (filteredSuggestions.Count > 0)
            {
                suggestionsPanel.gameObject.SetActive(true);

                foreach (var suggestion in filteredSuggestions)
                {
                    var suggestionObject = Instantiate(suggestionPrefab, suggestionsPanel);
                    suggestionObject.GetComponentInChildren<TextMeshProUGUI>().text = suggestion;
                    suggestionObject.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        inputfield.text = suggestion;
                        suggestionsPanel.gameObject.SetActive(false);
                    });

                    suggestionObjects.Add(suggestionObject);
                }
            }
            else
            {
                suggestionsPanel.gameObject.SetActive(false);
            }
        }
        else
        {
            suggestionsPanel.gameObject.SetActive(false);
        }
    }
}