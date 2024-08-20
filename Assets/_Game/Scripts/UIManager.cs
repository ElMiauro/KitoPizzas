using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager control;
    public GameObject pizzaPrefab;
    public GameObject pizzaHolder;
    public TMP_Text scoreText;
    GameController controller;
    

    private void Awake()
    {
        if (!control)
        {
            control = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    void Start()
    {
        controller = GameController.control;
    }

    public void DrawUI()
    {

        int numChildren = pizzaHolder.transform.childCount;
        int lives = controller.lives;
        if (numChildren > lives)
        {
            // Remove children in inverse order
            int diff = numChildren - lives;
            for (int i = 0; i < diff; i++)
            {
                Debug.Log(numChildren - i);
                Destroy(pizzaHolder.transform.GetChild(numChildren - i - 1).gameObject);
            }
        }
        else if (numChildren < lives)
        {
            int diff = lives - numChildren;
            for (int i = 0; i < diff; i++)
            {
                GameObject pizza = Instantiate(pizzaPrefab);
                pizza.transform.SetParent(pizzaHolder.transform);
            }
            // We need to add more 
            
        }
        scoreText.text = controller.score.ToString();
    }

    
}
