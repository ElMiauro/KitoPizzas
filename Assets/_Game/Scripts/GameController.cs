using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController control;
    public int score = 0;
    public int initialLives = 3;
    public int lives = 3;

    
    public Dictionary<ToppingType, int> toppings = new Dictionary<ToppingType, int>();
    
    UIManager ui;
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
        ui = UIManager.control;
        ui.DrawUI();
        foreach (ToppingType topping in Enum.GetValues(typeof(ToppingType)))
        {
            toppings.Add(topping, 0);
        }
    }

	public void GetHit()
    {
        if (lives < 1)
        {
            Debug.Log("GameOver");
        }
        else
        {
            lives--;
            ui.DrawUI();
        }
    }

    public void GetTopping(ToppingType toppingType, int _score)
    {
        int currValue = toppings[toppingType];
        toppings[toppingType] = currValue + 1;
        score += _score;
        ui.DrawUI();
    }
}
