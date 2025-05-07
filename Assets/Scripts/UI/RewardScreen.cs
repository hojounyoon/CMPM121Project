using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

    private string generatedSpell;

    void Start()
    {
        // Only hide the screen at start, don't generate spell yet
        gameObject.SetActive(false);
        SpellBuilder builder = new SpellBuilder();
        builder.LoadSpells();
    }

    public void Show()
    {
        Debug.Log("RewardScreen Show() called");

        // Generate the spell using SpellBuilder
        SpellBuilder builder = new SpellBuilder();
        builder.LoadSpells();
        generatedSpell = builder.GenerateRandomSpell();

        // Now display it
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
        // Load spell data
        TextAsset spellsJson = Resources.Load<TextAsset>("spells");
        if (spellsJson == null)
        {
            Debug.LogError("Failed to load spells.json!");
            return;
        }

        JObject spells = JObject.Parse(spellsJson.text);

        // Split the spellType to get base and modifiers
        string[] parts = spellType.Split(' ');
        string baseSpell = parts[parts.Length - 1];

        JObject baseSpellData = spells[baseSpell] as JObject;
        string displayName = baseSpellData["name"].ToString();
        string description = baseSpellData["description"].ToString();

        // Add modifiers to display if present
        for (int i = 0; i < parts.Length - 1; i++)
        {
            string modifier = parts[i];
            JObject modifierData = spells[modifier] as JObject;
            displayName = modifierData["name"].ToString() + " " + displayName;
            description += "\n" + modifierData["description"].ToString();
        }

        // Set UI
        if (spellNameText != null) spellNameText.text = displayName;
        if (descriptionText != null) descriptionText.text = description;
        if (spellIcon != null) SetSpellIcon(baseSpell);

        Canvas.ForceUpdateCanvases();
    }

    void SetSpellIcon(string baseSpell)
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
        player.spellui.SetSpell(newSpell);

        // Verify the spell was actually changed
        Debug.Log($"Final spell after assignment: Type = {player.spellcaster.spell.GetType().Name}, Name = {player.spellcaster.spell.GetName()}");
        Debug.Log($"Changed spell from {oldSpellName} to {newSpell.GetName()}");
    }
} 