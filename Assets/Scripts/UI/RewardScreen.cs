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

    private string[] baseSpells = { "arcane_bolt", "magic_missile", "arcane_blast", "arcane_spray" };
    private string[] modifierSpells = { "damage_amp", "speed_amp", "doubler", "splitter", "chaos", "homing" };
    private string generatedSpell;

    void Start()
    {
        // Only hide the screen at start, don't generate spell yet
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("RewardScreen Show() called");
        
        // Generate and display spell first
        GenerateAndDisplaySpell();
        
        // Make sure UI elements are enabled
        if (spellNameText != null) spellNameText.enabled = true;
        if (descriptionText != null) descriptionText.enabled = true;
        if (spellIcon != null) spellIcon.enabled = true;
        
        // Activate the GameObject
        gameObject.SetActive(true);
        
        // Force one more UI update
        Canvas.ForceUpdateCanvases();
    }

    void GenerateAndDisplaySpell()
    {
        Debug.Log("GenerateAndDisplaySpell called");
        
        try
        {
            // Force UI elements to be enabled first
            if (spellNameText != null) spellNameText.enabled = true;
            if (descriptionText != null) descriptionText.enabled = true;
            if (spellIcon != null) spellIcon.enabled = true;

            // Generate base spell first and ENSURE it's set
            string baseSpell = baseSpells[Random.Range(0, baseSpells.Length)];
            generatedSpell = baseSpell;
            Debug.Log($"Selected base spell: {baseSpell}");

            // Load spell data
            TextAsset spellsJson = Resources.Load<TextAsset>("spells");
            if (spellsJson == null)
            {
                Debug.LogError("Failed to load spells.json!");
                return;
            }

            JObject spells = JObject.Parse(spellsJson.text);
            JObject baseSpellData = spells[baseSpell] as JObject;

            // Immediately update UI with base spell info
            string displayName = baseSpellData["name"].ToString();
            string description = baseSpellData["description"].ToString();

            // 50% chance to add modifier
            if (Random.value < 0.5f)
            {
                string modifier = modifierSpells[Random.Range(0, modifierSpells.Length)];
                generatedSpell = modifier + "_" + baseSpell;
                
                // Add modifier info to display
                JObject modifierData = spells[modifier] as JObject;
                displayName = modifierData["name"].ToString() + " " + displayName;
                description += "\n" + modifierData["description"].ToString();
                Debug.Log($"Added modifier: {modifier}");
            }

            // Force update UI elements and verify
            Debug.Log($"Setting name to: {displayName}");
            if (spellNameText != null)
            {
                spellNameText.text = displayName;
                spellNameText.ForceMeshUpdate(); // Force text update
                Debug.Log($"Spell name text component text is now: {spellNameText.text}");
            }
            else
            {
                Debug.LogError("spellNameText is null!");
            }

            Debug.Log($"Setting description to: {description}");
            if (descriptionText != null)
            {
                descriptionText.text = description;
                descriptionText.ForceMeshUpdate(); // Force text update
                Debug.Log($"Description text component text is now: {descriptionText.text}");
            }
            else
            {
                Debug.LogError("descriptionText is null!");
            }

            if (spellIcon != null)
            {
                SetSpellIcon(baseSpell);
            }
            else
            {
                Debug.LogError("spellIcon Image component is null!");
            }

            // Force the UI to update
            Canvas.ForceUpdateCanvases();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in GenerateAndDisplaySpell: {e.Message}\n{e.StackTrace}");
        }
    }

    void SetSpellIcon(string baseSpell)
    {
        // Get the Image component from the appropriate spell GameObject based on the spell type
        Image sourceImage = null;
        switch (baseSpell)
        {
            case "arcane_bolt":
                sourceImage = spell1.GetComponent<Image>();
                break;
            case "magic_missile":
                sourceImage = spell2.GetComponent<Image>();
                break;
            case "arcane_blast":
                sourceImage = spell3.GetComponent<Image>();
                break;
            case "arcane_spray":
                sourceImage = spell4.GetComponent<Image>();
                break;
        }

        if (sourceImage != null && sourceImage.sprite != null)
        {
            spellIcon.sprite = sourceImage.sprite;
            Debug.Log($"Set spell icon from {baseSpell} GameObject");
        }
        else
        {
            Debug.LogError($"Could not find sprite for {baseSpell}");
        }
    }

    public void OnAcceptClicked()
    {
        Debug.Log("Accept button clicked");
        
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

        // Store the old spell for comparison
        string oldSpellName = player.spellcaster.spell.GetName();
        
        // Assign the new spell to the player's spellcaster
        player.spellcaster.spell = newSpell;
        
        // Update the spell UI
        player.spellui.SetSpell(newSpell);

        Debug.Log($"Changed spell from {oldSpellName} to {newSpell.GetName()}");

        // Don't hide the reward screen here - let the wave manager handle that
        // Don't start next wave here - let the wave manager handle that
    }
} 