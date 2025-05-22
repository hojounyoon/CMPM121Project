using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public class RewardScreen : MonoBehaviour
{
    public TextMeshProUGUI spellNameText;    // Drag your spell name TextMeshProUGUI here
    public TextMeshProUGUI descriptionText;  // Drag your description TextMeshProUGUI here
    public Image spellIcon;                  // Drag your spell icon Image here
    public Button acceptButton;              // Drag your accept button here

    // Relic reward UI elements
    public GameObject relicRewardPanel;
    public Image[] relicIcons = new Image[3];        // Array of 3 relic icons
    public TextMeshProUGUI[] relicDescriptions = new TextMeshProUGUI[3];  // Array of 3 relic descriptions
    public Button[] takeButtons = new Button[3];     // Array of 3 take buttons

    private string generatedSpell;
    private bool hasGeneratedSpell = false;
    private List<Relic> availableRelics = new List<Relic>();
    private List<Relic> offeredRelics = new List<Relic>();

    void Start()
    {
        // Hide the screen initially
        gameObject.SetActive(false);
        if (relicRewardPanel != null)
        {
            relicRewardPanel.SetActive(false);
        }

        // Set up relic button listeners
        for (int i = 0; i < takeButtons.Length; i++)
        {
            int index = i;
            if (takeButtons[i] != null)
            {
                takeButtons[i].onClick.AddListener(() => OnRelicSelected(index));
            }
        }

        // Debug: List all resources in the Resources folder
        Object[] allResources = Resources.LoadAll("");
        Debug.Log("All resources in Resources folder:");
        foreach (Object resource in allResources)
        {
        }
    }

    public void Show()
    {
        // Don't show if already active
        if (gameObject.activeSelf)
        {
            return;
        }

        Debug.Log("RewardScreen Show() called");

        // Always show spell rewards
        ShowSpellRewards();

        // If it's time for relic rewards (every third wave starting from wave 3), show them too
        if (GameManager.Instance.currentWave % 3 == 0)
        {
            ShowRelicRewards();
        }
        else
        {
            // Hide relic screen if it's not a relic wave
            if (relicRewardPanel != null)
            {
                relicRewardPanel.SetActive(false);
            }
        }

        // Activate the screen and update the canvas
        gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    private void ShowSpellRewards()
    {
        // Show spell reward UI elements
        if (spellNameText != null) spellNameText.gameObject.SetActive(true);
        if (descriptionText != null) descriptionText.gameObject.SetActive(true);
        if (spellIcon != null) spellIcon.gameObject.SetActive(true);
        if (acceptButton != null) acceptButton.gameObject.SetActive(true);

        // Generate and display spell
        if (!hasGeneratedSpell)
        {
            SpellBuilder builder = new SpellBuilder();
            builder.LoadSpells();
            generatedSpell = builder.GenerateRandomSpell();
            hasGeneratedSpell = true;
        }
        DisplaySpell(generatedSpell);
    }

    private void ShowRelicRewards()
    {
        // Show relic panel
        if (relicRewardPanel != null)
        {
            relicRewardPanel.SetActive(true);
        }

        LoadAvailableRelics();
        SelectRandomRelics();
        UpdateRelicUI();
    }

    private void LoadAvailableRelics()
    {
        availableRelics.Clear();
        availableRelics = RelicManager.Instance.GetAvailableRelics();
        Debug.Log($"Total available relics: {availableRelics.Count}");
    }

    private void SelectRandomRelics()
    {
        offeredRelics.Clear();
        var availableRelics = RelicManager.Instance.GetAvailableRelics();
        
        if (availableRelics.Count == 0)
        {
            Debug.Log("No relics available to offer");
            return;
        }

        // Shuffle the available relics
        for (int i = availableRelics.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = availableRelics[i];
            availableRelics[i] = availableRelics[j];
            availableRelics[j] = temp;
        }

        // Take the first 3 relics (or less if there aren't enough)
        int count = Mathf.Min(3, availableRelics.Count);
        for (int i = 0; i < count; i++)
        {
            offeredRelics.Add(availableRelics[i]);
        }
    }

    private void UpdateRelicUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < offeredRelics.Count)
            {
                Relic relic = offeredRelics[i];
                relicIcons[i].gameObject.SetActive(true);
                relicDescriptions[i].gameObject.SetActive(true);
                takeButtons[i].gameObject.SetActive(true);

                relicIcons[i].sprite = GameManager.Instance.relicIconManager.GetSprite(relic.spriteIndex);
                relicDescriptions[i].text = $"{relic.name}\n{relic.trigger.description}\n{relic.effect.description}";
            }
            else
            {
                relicIcons[i].gameObject.SetActive(false);
                relicDescriptions[i].gameObject.SetActive(false);
                takeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Public methods for each button
    public void OnTakeRelic1()
    {
        OnRelicSelected(0);
    }

    public void OnTakeRelic2()
    {
        OnRelicSelected(1);
    }

    public void OnTakeRelic3()
    {
        OnRelicSelected(2);
    }

    public void OnRelicSelected(int index)
    {
        if (index < offeredRelics.Count)
        {
            Relic selectedRelic = offeredRelics[index];
            RelicManager.Instance.AddRelic(selectedRelic);
            gameObject.SetActive(false);
        }
    }

    public void DisplaySpell(string spellType)
    {
        Debug.Log($"DisplaySpell called with spellType: {spellType}");

        // Load spell data from spells_copy.json
        TextAsset spellsJson = Resources.Load<TextAsset>("spells_copy");
        if (spellsJson == null)
        {
            Debug.LogError("Failed to load spells_copy.json!");
            return;
        }

        // Parse as JArray instead of JObject
        JArray spells = JArray.Parse(spellsJson.text);

        // Split the spellType to get base and modifiers
        string[] parts = spellType.Split(' ');
        string baseSpell = parts[parts.Length - 1];

        Debug.Log($"Parsed base spell: {baseSpell}");

        // Find the base spell in the array
        JObject baseSpellData = spells.FirstOrDefault(s => 
            s["name"].ToString() == baseSpell && 
            s["type"].ToString() == "base") as JObject;

        if (baseSpellData == null)
        {
            Debug.LogError($"Base spell data not found for {baseSpell} or it's not a base spell. Make sure it exists in spells.json and has type 'base'");
            return;
        }

        string displayName = baseSpellData["name"].ToString();
        string description = baseSpellData["description"].ToString();

        // --- Get the icon index from the JSON ---
        int iconIndex = 0;
        if (baseSpellData.ContainsKey("icon"))
        {
            iconIndex = baseSpellData["icon"].ToObject<int>();
        }
        else
        {
            Debug.LogWarning($"No icon index found for {baseSpell}, defaulting to 0.");
        }
        Debug.Log($"Icon index for {baseSpell}: {iconIndex}");

        // --- Set the spell icon using the icon index ---
        GameManager.Instance.spellIconManager.PlaceSprite(iconIndex, spellIcon.GetComponent<Image>());

        // Add modifiers to display if present
        for (int i = 0; i < parts.Length - 1; i++)
        {
            string modifier = parts[i];
            JObject modifierData = spells.FirstOrDefault(s => 
                s["name"].ToString() == modifier && 
                s["type"].ToString() == "modifier") as JObject;

            if (modifierData == null)
            {
                Debug.LogError($"Modifier data not found for {modifier} or it's not a modifier");
                continue;
            }
            displayName = modifierData["name"].ToString() + " " + displayName;
            description += "\n" + modifierData["description"].ToString();
        }

        // Set UI
        if (spellNameText != null) spellNameText.text = displayName;
        if (descriptionText != null) descriptionText.text = description;
        

        Canvas.ForceUpdateCanvases();
    }

    private void SetSpellIcon(string baseSpell)
    {
        Debug.Log($"SetSpellIcon called for spell: {baseSpell}");
        
        // Try different possible paths for the sprite
        string[] possiblePaths = new string[]
        {
            $"SpellIcons/{baseSpell}",           // SpellIcons/arcane_spray
            $"UI/SpellIcons/{baseSpell}",        // UI/SpellIcons/arcane_spray
            $"Icons/{baseSpell}",                // Icons/arcane_spray
            $"Spells/{baseSpell}",               // Spells/arcane_spray
            baseSpell                            // arcane_spray
        };

        Sprite spellSprite = null;
        foreach (string path in possiblePaths)
        {
            // Try loading as Sprite first
            spellSprite = Resources.Load<Sprite>(path);
            
            // If that fails, try loading as Texture2D and converting to Sprite
            if (spellSprite == null)
            {
                Texture2D texture = Resources.Load<Texture2D>(path);
                if (texture != null)
                {
                    spellSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    Debug.Log($"Found texture at path: {path} and converted to sprite");
                    break;
                }
            }
            else
            {
                Debug.Log($"Found sprite at path: {path}");
                break;
            }
        }
        
        if (spellSprite != null)
        {
            spellIcon.sprite = spellSprite;
            Debug.Log($"Successfully set spell icon for {baseSpell}");
        }
        else
        {
            Debug.LogError($"Could not find sprite or texture for {baseSpell}. Tried paths: {string.Join(", ", possiblePaths)}");
        }
    }

    public void OnAcceptClicked()
    {
        Debug.Log("Accept button clicked");
        Debug.Log($"Generated spell type: {generatedSpell}"); // Log the spell we're trying to create
        
        PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.LogError("RewardScreen: Player is null!");
            return;
        }

        if (player.spellcaster == null)
        {
            Debug.LogError("RewardScreen: Player's spellcaster is null!");
            return;
        }

        if (string.IsNullOrEmpty(generatedSpell))
        {
            Debug.LogError("RewardScreen: No spell has been generated!");
            return;
        }

        Debug.Log($"Creating spell: {generatedSpell}");
        
        // Create the new spell
        SpellBuilder builder = new SpellBuilder();
        builder.LoadSpells(); // Make sure spells are loaded
        Spell newSpell = builder.Build(player.spellcaster, generatedSpell);
        
        if (newSpell == null)
        {
            Debug.LogError("RewardScreen: Failed to create new spell!");
            return;
        }

        // Log more details about the new spell
        Debug.Log($"New spell created: Type = {newSpell.GetType().Name}, Name = {newSpell.GetName()}");
        Debug.Log($"Previous spell: Type = {player.spellcaster.spell.GetType().Name}, Name = {player.spellcaster.spell.GetName()}");

        // Store the old spell for comparison
        string oldSpellName = player.spellcaster.spell.GetName();
        
        // Assign the new spell to the player's spellcaster
        if(player.spellcaster.spell == null && player.spellui != null)
        {
            Debug.Log("spell added to position 1");
            player.spellcaster.spell = newSpell;
            player.spellui.SetSpell(newSpell);
        }
        else if(player.spellcaster.spell2 == null && player.spellui != null)
        {
            Debug.Log("spell added to position 2");   
            player.spellcaster.spell2 = newSpell;
            player.spellui2.SetSpell(newSpell);
        }
        else if(player.spellcaster.spell3 == null && player.spellui != null)
        {
            Debug.Log("spell added to position 3");
            player.spellcaster.spell3 = newSpell;
            player.spellui3.SetSpell(newSpell);
        }
        else if(player.spellcaster.spell4 == null && player.spellui != null)
        {
            Debug.Log("spell added to position 4");
            player.spellcaster.spell4 = newSpell;
            player.spellui4.SetSpell(newSpell);
        }
        else
        {
            Debug.Log("tried to equip another spell but already at max spells");
        }

        
        // else
        // {
        //     Debug.LogError("RewardScreen: Player's spellui is null!");
        // }

        // Verify the spell was actually changed
        Debug.Log($"Final spell after assignment: Type = {player.spellcaster.spell.GetType().Name}, Name = {player.spellcaster.spell.GetName()}");
        Debug.Log($"Changed spell from {oldSpellName} to {newSpell.GetName()}");

        // Reset the generated spell flag so next time we show the screen we'll generate a new spell
        hasGeneratedSpell = false;
        
        // Hide the screen
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Only show the screen if we're at wave end AND the screen is not active
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND && !gameObject.activeSelf)
        {
            Show();
        }
    }
} 