using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spellui;
    public SpellUI spellui2;
    public SpellUI spellui3;
    public SpellUI spellui4;

    public int speed;

    public Unit unit;

    public Transform spellContainer; // Reference to the UI container for spells

    private Vector3 lastPosition;
    private bool isMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;
        lastPosition = transform.position;
    }

    public void StartLevel()
    {
        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());
        
        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        Debug.Log($"spellcasters spell is: {spellcaster.spell.GetName()} and icon num is {spellcaster.spell.GetIcon()}");
        spellui.SetSpell(spellcaster.spell);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug log for points
        if (StatManager.Instance != null)
        {
            //Debug.Log($"Current Points: {StatManager.Instance.totalPoints}, Enemy Count: {GameManager.Instance.enemy_count}");
        }

        // Handle player input
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            CheckMovement();
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
            CheckMovement();
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            CheckMovement();
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            CheckMovement();
        }

        // Handle shooting
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Get mouse position and call OnAttack
            Vector2 mouseScreen = Mouse.current.position.value;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0;
            StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
            if (RelicManager.Instance != null)
            {
                RelicManager.Instance.OnPlayerCastSpell();
            }
        }

        // Update the player's position in the GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.player = gameObject;
        }
    }

    private void CheckMovement()
    {
        if (transform.position != lastPosition)
        {
            if (!isMoving)
            {
                isMoving = true;
                if (RelicManager.Instance != null)
                {
                    RelicManager.Instance.OnPlayerMove();
                }
            }
            lastPosition = transform.position;
        }
        else
        {
            isMoving = false;
        }
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.OnPlayerCastSpell();
        }
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) 
        {
            Debug.Log("Movement blocked due to game state");
            return;
        }
        unit.movement = value.Get<Vector2>()*speed;
        CheckMovement();
    }

    void Die()
    {
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        Debug.Log("You Lost");
    }

    public void AddSpell(Spell spell)
    {
        // Instantiate a new SpellUI
        GameObject spellUIObject = Instantiate(spellui.gameObject, spellContainer);
        SpellUI newSpellUI = spellUIObject.GetComponent<SpellUI>();
        newSpellUI.SetSpell(spell);
    }

    public int RPN(string input)
    {
        // CONVERTS WITH ONLY "wave"
        int val = 0;
        Stack<string> numStack = new Stack<string>();
        string[] tokens = input.Split(' ');
        if (tokens.Count() == 1)
        {
            return System.Int32.Parse(tokens[0]); 
        }

        for (int i = 0; i < tokens.Count(); i++) 
        {
            if (tokens[i] == "+" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = GameManager.Instance.currentWave;
                    numStack.Pop();
                }
                else
                {
                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = GameManager.Instance.currentWave;
                    numStack.Pop();
                }
                else
                {
                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 + val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "-" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = GameManager.Instance.currentWave;
                    numStack.Pop();
                }
                else
                {
                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = GameManager.Instance.currentWave;
                    numStack.Pop();
                }
                else
                {
                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 - val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "*" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = GameManager.Instance.currentWave;
                    numStack.Pop();
                }
                else
                {
                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = GameManager.Instance.currentWave;
                    numStack.Pop();
                }
                else
                {
                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 * val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "/" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = GameManager.Instance.currentWave;
                    numStack.Pop();
                }
                else
                {
                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = GameManager.Instance.currentWave + 1;
                    numStack.Pop();
                }
                else
                {
                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 / val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "%" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = GameManager.Instance.currentWave + 1;
                    numStack.Pop();
                }
                else
                {
                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = GameManager.Instance.currentWave + 1;
                    numStack.Pop();
                }
                else
                {
                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 % val2;
                numStack.Push(val.ToString());
                continue;
            }
            numStack.Push(tokens[i]);
        }
        return val;
    }
}
