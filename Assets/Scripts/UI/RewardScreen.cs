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

    [SerializeField] private Image homingSpellIcon; // Make sure this is assigned in the Inspector

    private string generatedSpell;
    private bool hasGeneratedSpell = false;  // Add this field

    void Start()
    {
        // Only hide the screen at start, don't generate spell yet
        gameObject.SetActive(false);
        
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

        // Only generate a new spell if we haven't generated one yet
        if (!hasGeneratedSpell)
        {
            // Generate the spell using SpellBuilder
            SpellBuilder builder = new SpellBuilder();
            builder.LoadSpells();
            generatedSpell = builder.GenerateRandomSpell();
            hasGeneratedSpell = true;  // Mark that we've generated a spell
        }

        // Display the existing spell
        DisplaySpell(generatedSpell);

        // Make sure UI elements are enabled
        if (spellNameText != null) spellNameText.enabled = true;
        if (descriptionText != null) descriptionText.enabled = true;
        if (spellIcon != null) spellIcon.enabled = true;
        
        // Activate the GameObject
        gameObject.SetActive(true);
        
        // Force one more UI update
        Canvas.ForceUpdateCanvases();
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