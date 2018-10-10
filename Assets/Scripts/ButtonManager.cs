using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
   public PlayerController controller;

    public Button buttton_red;
    public Button buttton_blue;
    public Button buttton_green;
    public Button buttton_yellow;

    public Button button_ready;

    Color color_select;


    public void SetColor(Color color)
    {
        if (color_select != null)
        {
            if(button_ready.interactable != true)
            button_ready.interactable = true;

            SetInterctableButtonsColor(color_select, true);
        }
        color_select = color;
        SetInterctableButtonsColor(color_select, false);

    }

    private void SetInterctableButtonsColor(Color color, bool interactable)
    {
        if (color == Color.red)
        {
            buttton_red.interactable = interactable;
        }

        if (color == Color.blue)
        {
            buttton_blue.interactable = interactable;
        }

        if (color == Color.green)
        {
            buttton_green.interactable = interactable;
        }

        if (color == Color.yellow)
        {
            buttton_yellow.interactable = interactable;
        }
    }

    public void SetPlayerController(PlayerController _controller)
    {
        controller = _controller;
    }
    
    //public void SendColor()
    //{
    //    controller.RecieveColorPlayer(color_select);
    //    Destroy(this.gameObject, 0.5f);
    //}


    public void ButttonSelectColor(string color)
    {
        if (color == "Red")
            SetColor(Color.red);

        if (color == "Blue")
            SetColor(Color.blue);

        if (color == "Green")
            SetColor(Color.green);

        if (color == "Yellow")
            SetColor(Color.yellow);
    }

}