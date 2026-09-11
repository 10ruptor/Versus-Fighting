using System;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MeshPositionComputer : MonoBehaviour
{
    private SkinnedMeshRenderer characterMeshRenderer;
    private PlayerGameplay owner;

    public void Initialize(PlayerGameplay player)
    {
        this.owner = player;
    }

    private void Awake()
    {
        characterMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    private float playerRelativeCharacterTop =>
        owner.transform.InverseTransformPoint(characterMeshRenderer.bounds.max).y;

    private float playerRelativeCharacterBottom =>
        owner.transform.InverseTransformPoint(characterMeshRenderer.bounds.min).y;

    public float CharacterMeshHeight => playerRelativeCharacterTop - playerRelativeCharacterBottom;
    public float CharacterMeshCenter => (playerRelativeCharacterTop + playerRelativeCharacterBottom) / 2f;
}