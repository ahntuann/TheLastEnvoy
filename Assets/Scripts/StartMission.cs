using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMission : MonoBehaviour
{

    public void OnStartMissionClicked()
    {
    
        SceneManager.LoadScene("Map4");
    }
}
