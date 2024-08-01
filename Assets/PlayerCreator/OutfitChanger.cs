using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutfitChanger : MonoBehaviour
{

    [Header("Sprite to Change")]
    public SpriteRenderer bodyPart;

    [Header("Sprites to Cycle")]
    public List<Sprite> options = new List<Sprite>();

    private int currentOption = 0;

    public void NextOption()
    {
        currentOption++;
        if(currentOption >= options.Count)
        {
            currentOption = 0;
        }
        bodyPart.sprite = options[currentOption];
    }
    public void PerviopusOption()
    {
        currentOption--;
        if(currentOption <= options.Count)
        {
            currentOption = options.Count - 1;
        }
        bodyPart.sprite = options[currentOption];
    }
}
