using System.Collections;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.UI;

public class HurtUI : MonoBehaviour
{
    public Image targetImage; // The Image component to change color
    public Color hurtColor = Color.red; // The color to change to when hurt
    public float hurtDuration = 0.5f; // Duration to keep the hurt color
    public float expressionDuration = 1f;

    private Color originalColor;
    public Character character;

    private void Start()
    {
        if (targetImage != null)
        {
            originalColor = targetImage.color;
        }
    }

    public void FlashHurtColor()
    {
        if (targetImage != null)
        {
            StartCoroutine(ChangeColor());
            StartCoroutine(Expression());
        }
    }

    private IEnumerator ChangeColor()
    {
        targetImage.color = hurtColor;
        yield return new WaitForSeconds(hurtDuration);
        targetImage.color = originalColor;
    }
    
    private IEnumerator Expression()
    {
        character.SetExpression("Angry");
        yield return new WaitForSeconds(expressionDuration);
        character.SetExpression("Default");
    }
}
