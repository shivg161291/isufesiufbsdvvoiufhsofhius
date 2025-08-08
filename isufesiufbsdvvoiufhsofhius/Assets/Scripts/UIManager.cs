using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI turnsText;
    public TextMeshProUGUI matchesText;

    public void UpdateScore(int score, int combo, int best, int turns, int matches)
    {
    if (scoreText) scoreText.text = $"Score: {score}";
    if (comboText) comboText.text = combo > 0 ? $"Combo x{combo}" : "";
    if (bestScoreText) bestScoreText.text = $"Best: {best}";
    if (turnsText) turnsText.text = $"Turns: {turns}";
    if (matchesText) matchesText.text = $"Matches: {matches}";
    }

    public void ShowGameOver(int score, int best)
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (gameOverScoreText) gameOverScoreText.text = $"Score: {score}";
        if (bestScoreText) bestScoreText.text = $"Best: {best}";
    }
}
