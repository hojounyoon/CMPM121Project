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

    // Add references to your existing spell GameObjects
    public GameObject spell1;  // Arcane Bolt
    public GameObject spell2;  // Magic Missile
    public GameObject spell3;  // Arcane Blast
    public GameObject spell4;  // Arcane Spray

    [SerializeField] private Image homingSpellIcon; // Make sure this is assigned in the Inspector

    private string generatedSpell;
    private bool hasGeneratedSpell = false;  // Add this field

    void Start()
    {
        // Only hide the screen at start, don't generate spell yet
        gameObject.SetActive(false);
        
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
        
        // Load spell data
        TextAsset spellsJson = Resources.Load<TextAsset>("spells_copy");
        if (spellsJson == null)
        {
            Debug.LogError("Failed to load spells.json!");
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
        if (spellIcon != null) SetSpellIcon(baseSpell);

        Canvas.ForceUpdateCanvases();
    }

    private void SetSpellIcon(string baseSpell)
    {
        Debug.Log($"SetSpellIcon called for spell: {baseSpell}");
        
        // Log the state of our spell GameObjects
        Debug.Log($"spell1 (Arcane Bolt) exists: {spell1 != null}");
        Debug.Log($"spell2 (Magic Missile) exists: {spell2 != null}");
        Debug.Log($"spell3 (Arcane Blast) exists: {spell3 != null}");
        Debug.Log($"spell4 (Arcane Spray) exists: {spell4 != null}");

        // Get the Image component from the appropriate spell GameObject based on the spell type
        Image sourceImage = null;
        switch (baseSpell)
        {
            case "arcane_bolt":
                if (spell1 != null)
                {
                    sourceImage = spell1.GetComponent<Image>();
                    Debug.Log("Found Arcane Bolt image component");
                }
                break;
            case "magic_missile":
                if (spell2 != null)
                {
                    sourceImage = spell2.GetComponent<Image>();
                    Debug.Log("Found Magic Missile image component");
                }
                break;
            case "arcane_blast":
                if (spell3 != null)
                {
                    sourceImage = spell3.GetComponent<Image>();
                    Debug.Log("Found Arcane Blast image component");
                }
                break;
            case "arcane_spray":
                if (spell4 != null)
                {
                    sourceImage = spell4.GetComponent<Image>();
                    Debug.Log("Found Arcane Spray image component");
                }
                break;
        }

        if (sourceImage != null)
        {
            if (sourceImage.sprite != null)
            {
                spellIcon.sprite = sourceImage.sprite;
                Debug.Log($"Successfully set spell icon from {baseSpell} GameObject");
            }
            else
            {
                Debug.LogError($"Found Image component for {baseSpell} but it has no sprite!");
            }
        }
        else
        {
            Debug.LogError($"Could not find Image component for {baseSpell}");
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
        player.spellcaster.spell = newSpell;
        
        // Update the spell UI
        if (player.spellui != null)
        {
            player.spellui.SetSpell(newSpell);
        }
        else
        {
            Debug.LogError("RewardScreen: Player's spellui is null!");
        }

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