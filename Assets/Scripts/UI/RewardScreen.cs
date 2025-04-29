using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class RewardScreen : MonoBehaviour
{
    public TextMeshProUGUI spellNameText;    // Drag your spell name TextMeshProUGUI here
    public TextMeshProUGUI descriptionText;  // Drag your description TextMeshProUGUI here
    public Image spellIcon;                  // Drag your spell icon Image here
    public Button acceptButton;              // Drag your accept button here

    private string[] baseSpells = { "arcane_bolt", "magic_missile", "arcane_blast", "arcane_spray" };
    private string[] modifierSpells = { "damage_amp", "speed_amp", "doubler", "splitter", "chaos", "homing" };
    private string generatedSpell;

    void Start()
    {
        // Hide the screen at start
        gameObject.SetActive(false);
        // Initialize the spell when the component starts
        GenerateAndDisplaySpell();
    }

    public void Show()
    {
        Debug.Log("RewardScreen Show() called");
        
        // Make sure UI elements are assigned
        if (spellNameText == null) Debug.LogError("spellNameText is not assigned!");
        if (descriptionText == null) Debug.LogError("descriptionText is not assigned!");
        if (spellIcon == null) Debug.LogError("spellIcon is not assigned!");
        
        GenerateAndDisplaySpell();
        
        // Force refresh UI elements
        if (spellNameText != null) spellNameText.enabled = false;
        if (descriptionText != null) descriptionText.enabled = false;
        if (spellIcon != null) spellIcon.enabled = false;
        
        if (spellNameText != null) spellNameText.enabled = true;
        if (descriptionText != null) descriptionText.enabled = true;
        if (spellIcon != null) spellIcon.enabled = true;
        
        gameObject.SetActive(true);
    }

    void GenerateAndDisplaySpell()
    {
        Debug.Log("Generating new spell...");  // Add debug log
        
        // Generate random spell
        string baseSpell = baseSpells[Random.Range(0, baseSpells.Length)];
        generatedSpell = baseSpell;

        Debug.Log($"Base spell selected: {baseSpell}");  // Add debug log

        // 50% chance to add modifier
        if (Random.value < 0.5f)
        {
            string modifier = modifierSpells[Random.Range(0, modifierSpells.Length)];
            generatedSpell = modifier + "_" + baseSpell;
            Debug.Log($"Added modifier: {modifier}");  // Add debug log
        }

        try
        {
            // Load spell data
            TextAsset spellsJson = Resources.Load<TextAsset>("spells");
            if (spellsJson == null)
            {
                Debug.LogError("Failed to load spells.json!");
                return;
            }

            JObject spells = JObject.Parse(spellsJson.text);
            
            // Get base spell data
            string[] spellParts = generatedSpell.Split('_');
            string baseSpellName = spellParts[spellParts.Length - 1];
            JObject baseSpellData = spells[baseSpellName] as JObject;

            if (baseSpellData == null)
            {
                Debug.LogError($"Could not find data for spell: {baseSpellName}");
                return;
            }

            // Set name and description
            string displayName = baseSpellData["name"].ToString();
            string description = baseSpellData["description"].ToString();

            // Add modifier info if present
            if (spellParts.Length > 1)
            {
                string modifierName = spellParts[0];
                JObject modifierData = spells[modifierName] as JObject;
                displayName = modifierData["name"].ToString() + " " + displayName;
                description += "\n" + modifierData["description"].ToString();
            }

            // Update UI elements
            if (spellNameText != null) spellNameText.text = displayName;
            if (descriptionText != null) descriptionText.text = description;
            if (spellIcon != null && GameManager.Instance.spellIconManager != null)
            {
                GameManager.Instance.spellIconManager.PlaceSprite(baseSpellData["icon"].Value<int>(), spellIcon);
            }

            Debug.Log($"Successfully generated spell: {generatedSpell}");  // Add debug log
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error generating spell: {e.Message}");
        }
    }

    public void OnAcceptClicked()
    {
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
        
        // Create and assign the new spell
        SpellBuilder builder = new SpellBuilder();
        Spell newSpell = builder.Build(player.spellcaster, generatedSpell);
        
        if (newSpell == null)
        {
            Debug.LogError("RewardScreen: Failed to create new spell!");
            return;
        }

        player.spellcaster.spell = newSpell;
        player.spellui.SetSpell(newSpell);

        // Hide the reward screen
        gameObject.SetActive(false);
    }
} 