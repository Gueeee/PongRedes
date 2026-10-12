using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas locais
using Unity.Netcode;

public class MainMenuBehaviour : MonoBehaviour {
    public void StartHost() {
        NetworkManager.Singleton.StartHost();
    
        NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        SceneManager.LoadScene("GameplayUI", LoadSceneMode.Additive);
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
    }
    
    // public void OnPlayButtonClicked() {
    //     GameManager.Instance.StartLocalMatch();
    // }
}
