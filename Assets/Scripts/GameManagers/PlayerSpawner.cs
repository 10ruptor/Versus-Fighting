using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerSpawner : MonoBehaviour
{
    private PlayerInputManager playerInputController;
    [SerializeField] private MainCamera mainCamera;
    [SerializeField] private GameObject uiParent;
    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputManager>();
        playerInputController.onPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log($"Player joined : {player.playerIndex}");

        PlayerGameplay gameplay = player.GetComponent<PlayerGameplay>();
    
        gameplay.Initialize(player.playerIndex,uiParent);
        mainCamera.AddTrackingTarget(gameplay.transform);
    }
    
}
