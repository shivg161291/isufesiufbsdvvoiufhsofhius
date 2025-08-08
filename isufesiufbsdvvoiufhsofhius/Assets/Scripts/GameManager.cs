using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GridBuilder gridBuilder;
    
    public GameObject cardPrefab; // must have Card component + Image (UI)
    public RectTransform cardParent; // UI container with GridLayoutGroup
    public AudioManager Audio;
    public UIManager UI;

    [Header("Game Settings")]
    public int rows = 3;
    public int cols = 4;
    public Sprite backSprite;
    public List<Sprite> allPossibleSprites; // Large sprite pool
    private List<Sprite> gameSprites= new List<Sprite>();       
    private List<int> currentSpriteOrder = new List<int>();
    private bool isSuffle = false;
    List<Card> allCards = new List<Card>();
    Queue<Card> pendingFlips = new Queue<Card>();
    bool isRunning = false;

    // scoring
    public int score = 0;
    public int combo = 0;
    public int bestScore = 0;
    public int turns = 0;
    public int matchesFound = 0;
    public int matchBasePoints = 100;
    public float mismatchDelay = 0.8f;
    public float matchRevealDelay = 0.25f;
    public List<int> spriteOrder = new List<int>();
    // save file
    string saveFileName => Path.Combine(Application.persistentDataPath, "cardmatch_save.json");

    class SaveData
    {
        public int rows, cols;
        public List<int> cardSpriteIndexes; // sprite order
        public List<int> matchedIndices;    // matched card indexes
        public int score;
        public int bestScore;
        public int turns;
        public int matchesFound;
    }

    void Start()
    {
        Audio = Audio ?? FindObjectOfType<AudioManager>();
        UI = UI ?? FindObjectOfType<UIManager>();

        bool loaded = Load();
        if (!loaded)
        {
            SetupBoard(rows, cols);
        }

        isRunning = true;
        StartCoroutine(CompareLoop());
    }

    public void SetupBoard(int r, int c, List<int> savedOrder = null, List<int> savedMatched = null)
    {
        rows = r;
        cols = c;
        int totalCards = rows * cols;
        int pairs = totalCards / 2;

        // clear old
        foreach (Transform t in cardParent) Destroy(t.gameObject);
        allCards.Clear();

        gridBuilder.ApplyGrid(cardParent, rows, cols);

        if (savedOrder != null && savedOrder.Count == totalCards)
        {
            currentSpriteOrder = new List<int>(savedOrder);
        }
        else
        {
            currentSpriteOrder.Clear();
            List<int> indexes = new List<int>();
            for (int i = 0; i < pairs; i++)
            {
                indexes.Add(i);
                indexes.Add(i);
            }
            Shuffle(indexes);
            currentSpriteOrder = new List<int>(indexes);
        }

        gameSprites.Clear();
        foreach (var idx in currentSpriteOrder)
            gameSprites.Add(allPossibleSprites[idx]);

        for (int i = 0; i < totalCards; i++)
        {
            GameObject go = Instantiate(cardPrefab, cardParent);
            Card card = go.GetComponent<Card>();
            card.Initialize(i, gameSprites[i], backSprite, this);
            allCards.Add(card);
        }

        if (savedMatched != null)
        {
            foreach (int idx in savedMatched)
            {
                if (idx >= 0 && idx < allCards.Count)
                {
                    allCards[idx].ForceSetFace(true);
                    allCards[idx].MarkMatched();
                }
            }
        }
    }

    public void RequestFlip(Card card)
    {
        if (!isRunning) return;
        if (card.IsFaceUp || card.IsMatched) return;

        card.PlayFlipAnimation(() =>
        {
            lock (pendingFlips)
            {
                pendingFlips.Enqueue(card);
            }
        });
    }

    IEnumerator CompareLoop()
    {
        while (true)
        {
            yield return null;
            if (!isRunning) continue;

            Card a = null, b = null;
            lock (pendingFlips)
            {
                if (pendingFlips.Count >= 2)
                {
                    a = pendingFlips.Dequeue();
                    b = pendingFlips.Dequeue();
                }
            }

            if (a != null && b != null)
            {
                if (a.IsMatched || b.IsMatched) continue;

                if (a.FaceSprite == b.FaceSprite)
                {
                    a.MarkMatched();
                    b.MarkMatched();
                    Audio.PlayMatch();

                    combo++;
                    matchesFound++;

                    int points = Mathf.RoundToInt(matchBasePoints * (1 + combo * 0.1f));
                    score += points;

                    yield return new WaitForSeconds(matchRevealDelay);

                    if (CheckWin())
                        GameOver();
                }
                else
                {
                    Audio.PlayMismatch();
                    combo = 0;

                    yield return new WaitForSeconds(mismatchDelay);
                    if (!a.IsMatched) a.PlayFlipAnimation();
                    if (!b.IsMatched) b.PlayFlipAnimation();
                }

                turns++;
                UI?.UpdateScore(score, combo, bestScore, turns, matchesFound);

                // save after every turn
                Save();
            }
        }
    }

    bool CheckWin()
    {
        foreach (var c in allCards)
            if (!c.IsMatched) return false;
        return true;
    }

    void GameOver()
    {
        ClearSave();
        isRunning = false;
        Audio.PlayGameOver();

        if (score > bestScore) bestScore = score;
        UI?.ShowGameOver(score, bestScore);
    }

    public void Save()
    {
        SaveData s = new SaveData
        {
            rows = rows,
            cols = cols,
            cardSpriteIndexes = new List<int>(currentSpriteOrder),
            matchedIndices = new List<int>(),
            score = score,
            bestScore = bestScore,
            turns = turns,
            matchesFound = matchesFound
        };

        for (int i = 0; i < allCards.Count; i++)
        {
            if (allCards[i].IsMatched)
                s.matchedIndices.Add(i);
        }

        try
        {
            File.WriteAllText(saveFileName, JsonUtility.ToJson(s));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public bool Load()
    {
        if (!File.Exists(saveFileName))
        {
            UI?.UpdateScore(score, combo, bestScore, turns, matchesFound);
            return false;
        }

        try
        {
            SaveData s = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveFileName));
            rows = s.rows;
            cols = s.cols;
            score = s.score;
            bestScore = s.bestScore;
            turns = s.turns;
            matchesFound = s.matchesFound;

            SetupBoard(rows, cols, s.cardSpriteIndexes, s.matchedIndices);
            UI?.UpdateScore(score, combo, bestScore, turns, matchesFound);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Load failed: " + e.Message);
            return false;
        }
    }

    public void ClearSave()
    {
        int keepBestScore = bestScore;
        if (File.Exists(saveFileName)) File.Delete(saveFileName);
        bestScore = keepBestScore;
    }

    public void RestartGame()
    {
        ClearSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
