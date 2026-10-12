using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // Variáveis de rede que sincronizam do Servidor para os Clientes
    public NetworkVariable<int> scorePlayer1 = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> scorePlayer2 = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public event Action<int, int> OnScoreChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.LoadScene("MainMenu");
    }

    public override void OnNetworkSpawn()
    {
        // Quando qualquer valor mudar na rede, dispara o seu evento OnScoreChanged
        scorePlayer1.OnValueChanged += OnScoreValueChanged;
        scorePlayer2.OnValueChanged += OnScoreValueChanged;

        // Notifica a UI com os valores atuais assim que entra na partida
        OnScoreChanged?.Invoke(scorePlayer1.Value, scorePlayer2.Value);
    }

    public override void OnNetworkDespawn()
    {
        scorePlayer1.OnValueChanged -= OnScoreValueChanged;
        scorePlayer2.OnValueChanged -= OnScoreValueChanged;
    }

    private void OnScoreValueChanged(int previousValue, int newValue)
    {
        OnScoreChanged?.Invoke(scorePlayer1.Value, scorePlayer2.Value);
    }

    public void StartMatch()
    {
        SceneManager.LoadScene("GameplayUI", LoadSceneMode.Additive);
    }

    public void AddScore(int playerID)
    {
        // Apenas o Servidor altera a pontuação na rede
        if (!IsServer) return;

        if (playerID == 0) scorePlayer1.Value++;
        else if (playerID == 1) scorePlayer2.Value++;
    }
}