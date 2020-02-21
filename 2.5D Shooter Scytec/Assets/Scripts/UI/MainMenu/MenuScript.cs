using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public InputField ipInputField;

    public void PlayPressed()
    {
        NetworkManager.singleton.StartHost();
        SceneManager.LoadScene("Game");
    }

    public void ConnectPressed()
    {
        if(ipInputField.text != string.Empty)
            NetworkManager.singleton.networkAddress = ipInputField.text;
        NetworkManager.singleton.StartClient();
    }

    public void ExitPressed()
    {        
        Application.Quit();
    }

}
