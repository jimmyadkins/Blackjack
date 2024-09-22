using UnityEngine;
using UnityEngine.InputSystem; 

public class InputController : MonoBehaviour
{
    public BlackjackGameManager blackjackGameManager;
    public MenuManager menuManager;

    public void OnMenu()
    {
        Debug.Log("Menu button pressed");
        blackjackGameManager.OpenMenu();
    }

    //private void ReturnToMenu()
    //{
    //    blackjackGameManager.ResetGame();
    //    menuManager.ShowMainMenu();
    //}
}
