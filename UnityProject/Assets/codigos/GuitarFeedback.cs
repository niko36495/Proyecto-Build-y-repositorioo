
using UnityEngine;

public class GuitarFeedback : MonoBehaviour
{
    public SerialManager serialManager;

    public void CorrectNote()
    {
        serialManager.SendMessageToArduino("LED:GREEN");
        serialManager.SendMessageToArduino("BUZZER:ON");
    }

    public void WrongNote()
    {
        serialManager.SendMessageToArduino("LED:RED");
        serialManager.SendMessageToArduino("BUZZER:ON");
    }

    public void ComboAchieved()
    {
        serialManager.SendMessageToArduino("LED:GREEN");
        serialManager.SendMessageToArduino("BUZZER:ON");
    }

    public void StopFeedback()
    {
        serialManager.SendMessageToArduino("LED:OFF");
        serialManager.SendMessageToArduino("BUZZER:OFF");
    }
}
