
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class cambiardeintefaz : MonoBehaviour
{
    [Header("Botones del menu de pausa")]

    public Button[] buttons;

    private int selectedIndex = 0;


    void Start()
    {
        // Continuar hacia el juego
        buttons[0].onClick.AddListener(Continuar);

        // menu de pausa 
        buttons[1].onClick.AddListener(Volver);

        // menu de inicio 
        buttons[2].onClick.AddListener(VolverInicio);

        // menu de creditos
        buttons[3].onClick.AddListener(creditos);

        // Seleccionar el primer boton
        buttons[selectedIndex].Select();
    }


    void Update()
    {
        Navigation();
    }


    void Navigation()
    {
        // Flecha arriba
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex =
                (selectedIndex - 1 + buttons.Length) % buttons.Length;

            buttons[selectedIndex].Select();
        }

        // Flecha abajo
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex =
                (selectedIndex + 1) % buttons.Length;

            buttons[selectedIndex].Select();
        }

        // Enter
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            buttons[selectedIndex].onClick.Invoke();
        }
    }


        void Continuar()
    {
        SceneManager.LoadScene("juego_1");
    }

        void Volver()
    {
        SceneManager.LoadScene("pausa");
    }

        void VolverInicio()
    {
        SceneManager.LoadScene("inicio");
    }

        void creditos()
    {
        SceneManager.LoadScene("creditos");
    }

}
