using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 5)]
public class CharacterSO : ScriptableObject
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField] 
    private Vector2 spritePos, spriteScale = Vector2.one;
    [SerializeField] 
    private Vector2 abilitySpritePos, abilitySpriteScale = Vector2.one;
    [SerializeField]
    private SpellSO ability;

    public SpellSO GetAbility
    {
        get { return ability; }
    }
    public Vector2 GetSpritePosition
    {
        get { return spritePos; }
    }
    public Vector2 GetSpriteScale
    {
        get { return spriteScale; }
    }
    public Vector2 GetAbilitySpritePosition
    {
        get { return abilitySpritePos; }
    }
    public Vector2 GetAbilitySpriteScale
    {
        get { return abilitySpriteScale; }
    }
    public Sprite GetSprite
    {
        get { return sprite; }
    }
}
