using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterStatData characterStatData;
    public CharacterStatData CharacterStatData => characterStatData;
}
