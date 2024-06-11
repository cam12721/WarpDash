using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterDatabase : ScriptableObject
{
    public CharacterClass[] character;

    public int CharacterCount
    {
        get
        {
            return character.Length;
        }
    }

    public CharacterClass GetCharacter(int index)
    {
        return character[index];
    }
}
