using UnityEngine;
using TMPro;

public class MenuSelectorController : MonoBehaviour
{
    public TextMeshProUGUI label;
    public string level;
    public string className;
    public EnemySpawner spawner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(string text)
    {
        level = text;
        label.text = text;
    }

    public void StartLevel()
    {
        spawner.StartLevel(level);
        GameManager.Instance.UpdatePlayerStats();
    }

    public void SetClass(string text)
    {
        className = text;
        label.text = text;
    }

    public void SelectClass()
    {
        GameManager.Instance.SetClass(className);
        spawner.class_selector.gameObject.SetActive(false);
        spawner.CreateLevelButtons();
    }
}
