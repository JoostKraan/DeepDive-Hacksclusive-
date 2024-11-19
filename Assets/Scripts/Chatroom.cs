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

    private GameObject currentChatroom;
    private GameObject currentlySelectedChatItem;

    // Dictionary waarin elke gebruiker gekoppeld wordt aan een lijst met volledige interacties
    private Dictionary<string, List<ChatInteraction>> chatDictionary = new Dictionary<string, List<ChatInteraction>>();
    private Dictionary<string, List<GameObject>> chatHistory = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        InitializeChats();
        LoadChatList();
    }

    // Initialiseer de chatgegevens
    private void InitializeChats()
    {
        // Voeg voorbeeldinteracties toe voor StampLicker420
        chatDictionary.Add("StampLicker420", new List<ChatInteraction>
        {
            new ChatInteraction(
                "Hey, alles goed?",
                new List<ChatResponse>
                {
                    new ChatResponse("Ja, alles prima met jou?", () => RespondToChat("StampLicker420", "Goed om te horen! Ik doe rustig aan.")),
                    new ChatResponse("Niet zo goed, eerlijk gezegd.", () => RespondToChat("StampLicker420", "Ah, vervelend om te horen. Kan ik helpen?")),
                    new ChatResponse("Wat maakt het uit?", () => RespondToChat("StampLicker420", "Nou, ik dacht gewoon dat het aardig zou zijn om te vragen."))
                }
            ),
            new ChatInteraction(
                "Wat ben je aan het doen?",
                new List<ChatResponse>
                {
                    new ChatResponse("Niet veel, gewoon chillen.", () => RespondToChat("StampLicker420", "Klinkt relaxed.")),
                    new ChatResponse("Ik werk aan een project.", () => RespondToChat("StampLicker420", "Oh cool, wat voor project?")),
                    new ChatResponse("Niks waar jij je zorgen over moet maken.", () => RespondToChat("StampLicker420", "Oke, rustig maar!"))
                }
            )
        });

        // Voeg voorbeeldinteracties toe voor Hacker-man404
        chatDictionary.Add("Hacker-man404", new List<ChatInteraction>
        {
            new ChatInteraction(
                "Ik heb een nieuwe exploit gevonden.",
                new List<ChatResponse>
                {
                    new ChatResponse("Vertel me meer.", () => RespondToChat("Hacker-man404", "Het is een zero-day in het nieuwste systeem.")),
                    new ChatResponse("Dat klinkt illegaal...", () => RespondToChat("Hacker-man404", "Illegaal? Nee joh, dit is gewoon voor de lol!")),
                    new ChatResponse("Interessant, hoe werkt het?", () => RespondToChat("Hacker-man404", "Ik kan je de details later sturen."))
                }
            ),
            new ChatInteraction(
                "Ken je iemand die interesse heeft?",
                new List<ChatResponse>
                {
                    new ChatResponse("Misschien, wat is het precies?", () => RespondToChat("Hacker-man404", "Het is een manier om toegang te krijgen zonder sporen achter te laten.")),
                    new ChatResponse("Ik blijf liever buiten dit soort dingen.", () => RespondToChat("Hacker-man404", "Geen zorgen, ik vroeg het alleen maar.")),
                    new ChatResponse("Wat zit er voor mij in?", () => RespondToChat("Hacker-man404", "Laten we zeggen dat er zeker wat voor jou in zit."))
                }
            )
        });

        // Voeg voorbeeldinteracties toe voor TotallyNotTheFBI
        chatDictionary.Add("TotallyNotTheFBI", new List<ChatInteraction>
        {
            new ChatInteraction(
                "Wij willen alleen maar praten.",
                new List<ChatResponse>
                {
                    new ChatResponse("Ik heb niets te zeggen.", () => RespondToChat("TotallyNotTheFBI", "Dat zeggen ze allemaal.")),
                    new ChatResponse("Waarover dan?", () => RespondToChat("TotallyNotTheFBI", "Over je recente activiteiten.")),
                    new ChatResponse("Ik wil mijn advocaat spreken.", () => RespondToChat("TotallyNotTheFBI", "Dat kun je later doen."))
                }
            ),
            new ChatInteraction(
                "Vertel ons alles.",
                new List<ChatResponse>
                {
                    new ChatResponse("Ik weet van niks!", () => RespondToChat("TotallyNotTheFBI", "Hm, dat zeggen er veel.")),
                    new ChatResponse("Waarom zou ik dat doen?", () => RespondToChat("TotallyNotTheFBI", "Omdat wij je op heterdaad betrapt hebben.")),
                    new ChatResponse("Wat krijg ik ervoor terug?", () => RespondToChat("TotallyNotTheFBI", "Misschien strafvermindering."))
                }
            )
        });
    }

    private void LoadChatList()
    {
        foreach (var chat in chatDictionary.Keys)
        {
            // Instantieer het chat-item
            GameObject Chater = Instantiate(ChatPrefab, ChatListStack.transform);

            // Stel de naam en tekst in
            var textComponent = Chater.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = chat;
            textComponent.color = defaultTextColor; // Standaard kleur
            Chater.name = $"User: {chat}";

            // Voeg een klik-event toe
            var button = Chater.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                string selectedChat = chat; // Maak een lokale kopie voor de lambda
                button.onClick.AddListener(() => SelectChat(Chater, selectedChat));
            }
        }
    }

    private void SelectChat(GameObject chatItem, string chatName)
    {
        // Reset de kleur van de vorige geselecteerde chat
        if (currentlySelectedChatItem != null)
        {
            var previousText = currentlySelectedChatItem.GetComponentInChildren<TextMeshProUGUI>();
            previousText.color = defaultTextColor;
        }

        // Update de geselecteerde chat
        currentlySelectedChatItem = chatItem;
        SelectedChat = chatName;

        // Verander de kleur van de nieuwe geselecteerde chat
        var currentText = chatItem.GetComponentInChildren<TextMeshProUGUI>();
        currentText.color = selectedTextColor;

        print($"Geselecteerde chat: {SelectedChat}");

        // Clear de huidige chatbox en laad de opgeslagen geschiedenis
        ClearChatBox();
        if (chatHistory.ContainsKey(chatName))
        {
            foreach (var message in chatHistory[chatName])
            {
                Instantiate(message, ChatBoxContent.transform);
            }
        }
        else
        {
            chatHistory[chatName] = new List<GameObject>();
            ShowChatroom(chatName, 0);
        }
    }

    private void ShowChatroom(string chatName, int interactionIndex)
    {
        if (chatDictionary.TryGetValue(chatName, out List<ChatInteraction> chatInteractions))
        {
            if (interactionIndex < chatInteractions.Count)
            {
                ChatInteraction interaction = chatInteractions[interactionIndex];

                // Render het bericht in de chatbox
                GameObject chatMessage = Instantiate(ChatMessagePrefab, ChatBoxContent.transform);
                var messageText = chatMessage.GetComponentInChildren<TextMeshProUGUI>();
                messageText.text = $"{chatName}: {interaction.Message}";
                chatHistory[chatName].Add(chatMessage);

                // Verwijder vorige responsopties
                foreach (Transform child in ChatBoxContent.transform)
                {
                    if (child.gameObject.CompareTag("ResponseButton"))
                    {
                        Destroy(child.gameObject);
                    }
                }

                // Voeg responsopties toe aan de chatbox
                HorizontalLayoutGroup layoutGroup = ChatBoxContent.GetComponent<HorizontalLayoutGroup>();
                if (layoutGroup != null)
                {
                    layoutGroup.childControlHeight = true;
                    layoutGroup.childForceExpandHeight = true;
                    layoutGroup.childControlWidth = true;
                    layoutGroup.childForceExpandWidth = true;
                }

                for (int i = 0; i < interaction.Responses.Count; i++)
                {
                    ChatResponse response = interaction.Responses[i];
                    GameObject responseButton = Instantiate(ResponseButtonPrefab, ChatBoxContent.transform);
                    responseButton.tag = "ResponseButton";
                    var buttonText = responseButton.GetComponentInChildren<TextMeshProUGUI>();
                    buttonText.text = response.ResponseText;

                    var buttonComponent = responseButton.GetComponent<Button>();
                    if (buttonComponent != null)
                    {
                        buttonComponent.onClick.AddListener(() => StartCoroutine(HandleResponse(chatName, response.ResponseText, response.ResponseAction)));
                    }

                    // Zorg ervoor dat de knop automatisch schaalt met de tekst
                    LayoutElement layoutElement = responseButton.GetComponent<LayoutElement>();
                    if (layoutElement != null)
                    {
                        layoutElement.minHeight = 50;
                        layoutElement.preferredHeight = -1;
                        layoutElement.flexibleHeight = 1;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Geen verdere interacties beschikbaar voor {chatName}");
            }
        }
        else
        {
            Debug.LogWarning($"Geen chatgegevens gevonden voor {chatName}");
        }
    }

    private IEnumerator HandleResponse(string chatName, string responseText, System.Action responseAction)
    {
        // Render de reactie in de chatbox en verwijder de knoppen
        ClearResponseButtons();
        GameObject responseMessageObject = Instantiate(ChatMessagePrefab, ChatBoxContent.transform);
        var responseMessageText = responseMessageObject.GetComponentInChildren<TextMeshProUGUI>();
        responseMessageText.text = $"Jij: {responseText}";
        chatHistory[chatName].Add(responseMessageObject);

        // Wacht een willekeurig aantal seconden tussen 1 en 4
        yield return new WaitForSeconds(Random.Range(1f, 4f));

        // Voer de reactieactie uit en spawn de nieuwe responsopties
        responseAction.Invoke();
    }

    private void ClearResponseButtons()
    {
        foreach (Transform child in ChatBoxContent.transform)
        {
            if (child.gameObject.CompareTag("ResponseButton"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void RespondToChat(string chatName, string responseMessage)
    {
        // Render de reactie in de chatbox
        GameObject responseMessageObject = Instantiate(ChatMessagePrefab, ChatBoxContent.transform);
        var responseText = responseMessageObject.GetComponentInChildren<TextMeshProUGUI>();
        responseText.text = $"Jij: {responseMessage}";
        chatHistory[chatName].Add(responseMessageObject);

        Debug.Log($"Reactie aan {chatName}: {responseMessage}");
        // Hier kun je ook verdere interactie- of gamelogica toevoegen, zoals statistieken aanpassen of andere functies aanroepen
    }

    private void ClearChatBox()
    {
        foreach (Transform child in ChatBoxContent.transform)
        {
            Destroy(child.gameObject);
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
        Responses = responses;
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
