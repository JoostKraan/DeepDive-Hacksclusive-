using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Chatroom : MonoBehaviour
{
    [SerializeField] private string SelectedChat;
    [SerializeField] private GameObject ChatListStack;
    [SerializeField] private GameObject ChatPrefab;
    [SerializeField] private GameObject ChatBoxContent;
    [SerializeField] private GameObject ChatMessagePrefab;
    [SerializeField] private GameObject ResponseButtonPrefab;
    [SerializeField] private Color selectedTextColor = Color.yellow;
    [SerializeField] private Color defaultTextColor = Color.green;
    [SerializeField] private AudioSource notificationSound;

    private GameObject currentChatroom;
    private GameObject currentlySelectedChatItem;

    // Dictionary waarin elke gebruiker gekoppeld wordt aan een lijst met volledige interacties
    private Dictionary<string, List<ChatInteraction>> chatDictionary = new Dictionary<string, List<ChatInteraction>>();
    private Dictionary<string, List<ChatHistoryEntry>> chatHistory = new Dictionary<string, List<ChatHistoryEntry>>();
    private Dictionary<string, int> unreadMessages = new Dictionary<string, int>();
    private Dictionary<string, List<ChatInteraction>> backupDictionary = new Dictionary<string, List<ChatInteraction>>();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // M-toets om een bericht te simuleren
        {
            string randomChat = GetRandomChat();
            ReceiveMessage(randomChat);
        }
    }

    private void Awake()
    {
        InitializeChats(); // Chatgegevens initialiseren
        StartCoroutine(SimulateIncomingMessages());
    }
    private IEnumerator SimulateIncomingMessages()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(45f, 120f));

            // Kies een willekeurige gebruiker uit het chatDictionary
            List<string> chatNames = new List<string>(chatDictionary.Keys);
            if (chatNames.Count > 0)
            {
                string randomChat = chatNames[Random.Range(0, chatNames.Count)];
                ReceiveMessage(randomChat);
            }
            else
            {
                Debug.LogWarning("Geen gebruikers meer met beschikbare interacties.");
            }
        }
    }



    private string GetRandomChat()
    {
        List<string> chatNames = new List<string>(chatDictionary.Keys);

        if (chatNames.Count == 0)
        {
            Debug.LogWarning("Geen chats beschikbaar in chatDictionary!");
            return null; // Geen beschikbare chats
        }

        int randomIndex = Random.Range(0, chatNames.Count);
        return chatNames[randomIndex];
    }


    // Initialiseer de chatgegevens
    private void InitializeChats()
    {
        // Voeg voorbeeldinteracties toe voor Hacker-man404
        var hackerManInteractions = new List<ChatInteraction>
        {
            new ChatInteraction(
                "Ik heb een nieuwe exploit gevonden.",
                new List<ChatResponse>
                {
                    new ChatResponse("Vertel me meer.", () => RespondToChat("Hacker-man404", "Het is een zero-day in het nieuwste systeem.")),
                    new ChatResponse("Dat klinkt illegaal...", () => RespondToChat("Hacker-man404", "Illegaal? Nee joh, dit is gewoon voor de lol!")),
                    new ChatResponse("Interessant, hoe werkt het?", () => RespondToChat("Hacker-man404", "Ik kan je de details later sturen."))
                }
            )
        };

        // Voeg voorbeeldinteracties toe voor LaZuR3s
        var laZur3sInteractions = new List<ChatInteraction>
        {
            new ChatInteraction(
                "Hey, alles goed?",
                new List<ChatResponse>
                {
                    new ChatResponse("Ja, alles prima met jou?", () => RespondToChat("LaZuR3s", "Goed om te horen! Ik doe rustig aan.")),
                    new ChatResponse("Niet zo goed, eerlijk gezegd.", () => RespondToChat("LaZuR3s", "Ah, vervelend om te horen. Kan ik iets doen?")),
                    new ChatResponse("Wat maakt het uit?", () => RespondToChat("LaZuR3s", "Nou, ik dacht gewoon dat het aardig zou zijn om te vragen."))
                }
            )
        };

        // Voeg voorbeeldinteracties toe voor TotallyNotTheFBI
        var totallyNotTheFBIInteractions = new List<ChatInteraction>
        {
            new ChatInteraction(
                "Wij willen alleen maar praten.",
                new List<ChatResponse>
                {
                    new ChatResponse("Ik heb niets te zeggen.", () => RespondToChat("TotallyNotTheFBI", "Dat zeggen ze allemaal...")),
                    new ChatResponse("Waarover dan?", () => RespondToChat("TotallyNotTheFBI", "Over je recente activiteiten.")),
                    new ChatResponse("Ik wil mijn advocaat spreken.", () => RespondToChat("TotallyNotTheFBI", "Dat kun je regelen, maar we weten al genoeg."))
                }
            )
        };

        // Voeg entries toe aan chatDictionary
        chatDictionary.Add("Hacker-man404", hackerManInteractions);
        chatDictionary.Add("LaZuR3s", laZur3sInteractions);
        chatDictionary.Add("TotallyNotTheFBI", totallyNotTheFBIInteractions);
        Debug.Log($"Chat TotallyNotTheFBI toegevoegd aan chatDictionary met {chatDictionary["TotallyNotTheFBI"].Count} interacties.");

        // Maak een kopie van alle interacties voor de backupDictionary
        foreach (var entry in chatDictionary)
        {
            backupDictionary.Add(entry.Key, new List<ChatInteraction>(entry.Value));
        }

        Debug.Log($"Chats geïnitialiseerd. Aantal chats: {chatDictionary.Count}");
    }


    private void LoadChatList()
    {
        // Wis de huidige lijst
        foreach (Transform child in ChatListStack.transform)
        {
            Destroy(child.gameObject);
        }

        // Itereer door de gebruikers met berichten in de chatHistory
        foreach (var chat in chatHistory.Keys)
        {
            GameObject Chater = Instantiate(ChatPrefab, ChatListStack.transform);
            var textComponent = Chater.GetComponentInChildren<TextMeshProUGUI>();

            // Controleer of er ongelezen berichten zijn
            int unreadCount = unreadMessages.ContainsKey(chat) ? unreadMessages[chat] : 0;
            textComponent.text = unreadCount > 0
                ? $"{chat} ({unreadCount})"
                : chat;

            textComponent.color = defaultTextColor;
            Chater.name = $"User: {chat}";

            // Voeg klikfunctionaliteit toe
            var button = Chater.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                string selectedChat = chat;
                button.onClick.AddListener(() => SelectChat(Chater, selectedChat));
            }
        }
    }



    public void ReceiveMessage(string chatName)
    {
        // Controleer of de chat bestaat in het dictionary
        if (chatDictionary.ContainsKey(chatName))
        {
            // Herstel interacties als de lijst leeg is en een back-up beschikbaar is
            if (chatDictionary[chatName].Count == 0 && backupDictionary.ContainsKey(chatName))
            {
                Debug.Log($"Herstellen van interacties voor {chatName} vanuit back-up.");
                chatDictionary[chatName] = new List<ChatInteraction>(backupDictionary[chatName]);
            }


// Controleer opnieuw of er interacties zijn nadat het herstel is geprobeerd
            if (chatDictionary[chatName].Count > 0)
            {
                ChatInteraction interaction = chatDictionary[chatName][0];
                Debug.Log($"Interactie gekozen voor {chatName}: {interaction.Message}");

                // Voeg de interactie toe aan de geschiedenis
                if (!chatHistory.ContainsKey(chatName))
                {
                    chatHistory[chatName] = new List<ChatHistoryEntry>();
                }

                chatHistory[chatName].Add(new ChatHistoryEntry(interaction.Message, Color.yellow));

                // Verwijder de interactie uit de lijst
                chatDictionary[chatName].RemoveAt(0);
                Debug.Log(
                    $"Interactie verwerkt voor {chatName}. Overgebleven interacties: {chatDictionary[chatName].Count}");

                // Werk de chatlijst bij
                LoadChatList();
            }
            else
            {
                Debug.LogWarning($"Geen interacties beschikbaar voor {chatName} zelfs na herladen.");
            }
        }
    }


public void SelectChat(GameObject chatItem, string chatName)
{
    if (unreadMessages.ContainsKey(chatName))
    {
        unreadMessages[chatName] = 0;
    }

    currentlySelectedChatItem = chatItem;
    SelectedChat = chatName;

    ClearChatBox();  // Verwijder chatberichten
    ClearResponseButtons();  // Verwijder alle response knoppen

    // Voeg chatgeschiedenis toe aan de chatbox
    if (chatHistory.ContainsKey(chatName))
    {
        Debug.Log($"Laden van de geschiedenis voor {chatName}, aantal berichten: {chatHistory[chatName].Count}");
        foreach (var historyEntry in chatHistory[chatName])
        {
            AddMessageToChatBox(chatName, historyEntry.MessageText, historyEntry.MessageColor);
        }
    }
    else
    {
        Debug.LogWarning($"Geen geschiedenis beschikbaar voor {chatName}");
    }

    // Controleer of er interacties beschikbaar zijn in de chatDictionary
    if (chatDictionary.ContainsKey(chatName) && chatDictionary[chatName].Count > 0)
    {
        ChatInteraction interaction = chatDictionary[chatName][0];
        Debug.Log($"Interactie geladen voor {chatName}: {interaction.Message}");

        AddMessageToChatBox(chatName, interaction.Message, Color.yellow); // Voeg het bericht toe aan de chatbox

        Transform responseButtonContainer = GetResponseButtonContainer();
        foreach (var response in interaction.Responses)
        {
            Debug.Log($"Knop maken voor reactie: {response.ResponseText}"); // Debug om te controleren of responses worden toegevoegd

            GameObject responseButton = Instantiate(ResponseButtonPrefab, responseButtonContainer);
            var buttonText = responseButton.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = response.ResponseText; // Voeg de juiste response tekst toe
            }
            else
            {
                Debug.LogError("TextMeshProUGUI niet gevonden in ResponseButtonPrefab.");
            }

            var buttonComponent = responseButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => HandleResponse(chatName, response)); // Stel de actie van de knop in
            }
            else
            {
                Debug.LogError("Button-component niet gevonden in ResponseButtonPrefab.");
            }
        }
    }
    else
    {
        Debug.LogWarning($"Geen interacties beschikbaar voor {chatName} of chat bestaat niet in chatDictionary.");
    }

    LoadChatList();
}

    // ReSharper disable Unity.PerformanceAnalysis
    private Transform GetResponseButtonContainer()
    {
        Transform responseButtonContainer = ChatBoxContent.transform.Find("ResponseButtonContainer");
        if (responseButtonContainer == null)
        {
            Debug.LogError("ResponseButtonContainer niet gevonden in ChatBoxContent.");
        }
        else
        {
            Debug.Log($"ResponseButtonContainer gevonden: {responseButtonContainer.name}");
        }
        return responseButtonContainer;
    }

private void HandleResponse(string chatName, ChatResponse response)
{
    Debug.Log($"Reactie gekozen voor {chatName}: {response.ResponseText}");

    if (!chatHistory.ContainsKey(chatName))
    {
        chatHistory[chatName] = new List<ChatHistoryEntry>();
    }
    chatHistory[chatName].Add(new ChatHistoryEntry($"Jij:\n{response.ResponseText}", Color.blue));
    AddMessageToChatBox("Jij", response.ResponseText, Color.blue);

    // Roep de bijbehorende actie van de response aan
    response.ResponseAction?.Invoke();

    // Werk de UI bij zonder knoppen direct te verwijderen
    UpdateChatUI(chatName);
}

private void UpdateChatUI(string chatName)
{
    ClearResponseButtons(); // Verwijder alleen de bestaande response knoppen
    if (chatDictionary.ContainsKey(chatName) && chatDictionary[chatName].Count > 0)
    {
        var interaction = chatDictionary[chatName][0];
        var responseButtonContainer = GetResponseButtonContainer();
        if (responseButtonContainer != null)
        {
            foreach (var response in interaction.Responses)
            {
                var responseButton = Instantiate(ResponseButtonPrefab, responseButtonContainer);
                var buttonText = responseButton.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = response.ResponseText; // Stel de tekst van de response knop in
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI niet gevonden in ResponseButtonPrefab.");
                }

                var buttonComponent = responseButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => HandleResponse(chatName, response));
                }
                else
                {
                    Debug.LogError("Button-component niet gevonden in ResponseButtonPrefab.");
                }
            }
        }
        else
        {
            Debug.LogError("ResponseButtonContainer niet gevonden.");
        }
    }
}


private void ClearResponseButtons()
{
    Transform responseButtonContainer = GetResponseButtonContainer();
    if (responseButtonContainer == null)
    {
        Debug.LogError("ResponseButtonContainer niet gevonden in ChatBoxContent.");
        return;
    }

    Debug.Log("Verwijder alle knoppen binnen ResponseButtonContainer.");
    foreach (Transform child in responseButtonContainer)
    {
        Destroy(child.gameObject);
        Debug.Log($"Knop '{child.name}' verwijderd.");
    }
}

    private void RespondToChat(string chatName, string responseMessage)
    {
        // Render de reactie in de chatbox
        GameObject responseMessageObject = Instantiate(ChatMessagePrefab, ChatBoxContent.transform);
        var responseText = responseMessageObject.GetComponentInChildren<TextMeshProUGUI>();
        notificationSound.Play();
        responseText.text = $"{chatName}:\n{responseMessage}";
        responseText.color = Color.yellow;

        // Voeg het reactiebericht toe aan de geschiedenis
        if (!chatHistory.ContainsKey(chatName))
        {
            chatHistory[chatName] = new List<ChatHistoryEntry>();
        }

        chatHistory[chatName].Add(new ChatHistoryEntry(responseMessage, Color.yellow));

        EnsureButtonContainerIsLast();
        Debug.Log($"Reactie aan {chatName}: {responseMessage}");
    }
    private void AddMessageToChatBox(string userName, string message, Color color)
    {
        GameObject newMessage = Instantiate(ChatMessagePrefab, ChatBoxContent.transform);
        var textComponent = newMessage.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = $"{userName}: {message}"; // Voeg de gebruikersnaam toe aan het bericht
            textComponent.color = color;
            Debug.Log($"Bericht toegevoegd aan chatbox: {userName}: {message}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI niet gevonden in ChatMessagePrefab.");
        }
    }

    private IEnumerator ReloadChatContent(string chatName)
    {
        yield return new WaitForEndOfFrame(); // Wacht tot het einde van de huidige frame
        SelectChat(currentlySelectedChatItem, chatName);
    }

    public void ReloadChat(string chatName)
    {
        StartCoroutine(ReloadChatContent(chatName));
    }

    private void ClearChatBox()
    {
        foreach (Transform child in ChatBoxContent.transform)
        {
            // Zorg ervoor dat de `ResponseButtonContainer` zelf niet wordt verwijderd.
            if (child.name != "ResponseButtonContainer")
            {
                Destroy(child.gameObject);
            }
        }
    }



    private void EnsureButtonContainerIsLast()
    {
        Transform responseButtonContainer = GetResponseButtonContainer();
        if (responseButtonContainer != null)
        {
            responseButtonContainer.SetAsLastSibling();
        }
    }



}

// Klasse voor een volledige interactie
public class ChatInteraction
{
    public string Message { get; private set; }
    public List<ChatResponse> Responses { get; private set; }

    public ChatInteraction(string message, List<ChatResponse> responses)
    {
        Message = message;
        Responses = responses ?? new List<ChatResponse>();
    }
}



// Klasse voor een mogelijke reactie
public class ChatResponse
{
    public string ResponseText { get; private set; }
    public System.Action ResponseAction { get; private set; }

    public ChatResponse(string responseText, System.Action responseAction)
    {
        ResponseText = responseText;
        ResponseAction = responseAction;
    }
}



public class ChatHistoryEntry
{
    public string MessageText { get; private set; }
    public Color MessageColor { get; private set; }
    public List<string> ResponseOptions { get; private set; }
    public bool IsProcessed { get; set; } // Nieuw veld om de status van de interactie bij te houden

    public ChatHistoryEntry(string messageText, Color messageColor, List<string> responseOptions = null, bool isProcessed = false)
    {
        MessageText = messageText;
        MessageColor = messageColor;
        ResponseOptions = responseOptions;
        IsProcessed = isProcessed;
    }
}



