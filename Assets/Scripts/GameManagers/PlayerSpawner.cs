using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerSpawner : MonoBehaviour
{
    private PlayerInputManager playerInputController;
    [SerializeField] private MainCamera mainCamera;
    [SerializeField] private GameObject UIParent;
    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputManager>();
        playerInputController.onPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log($"Player joined : {player.playerIndex}");

        PlayerGameplay gameplay = player.GetComponent<PlayerGameplay>();
    
        gameplay.Initialize(player.playerIndex);
        InitializeUI(gameplay);
        mainCamera.AddTrackingTarget(gameplay.transform);
        
       
    }

    private void InitializeUI(PlayerGameplay gameplay)
    {
        TextMeshProUGUI percentText = Instantiate(gameplay.PlayerUIPrefab, UIParent.transform).GetComponent<PlayerUIArea>().PercentText;
        gameplay.DamageController.Initialize(percentText);
    }
}
