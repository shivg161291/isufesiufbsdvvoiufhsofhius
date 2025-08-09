# isufesiufbsdvvoiufhsofhius
A polished isufesiufbsdvvoiufhsofhius game built with Unity.  
It supports smooth animations, dynamic grid sizes, a save/load system, and works on both desktop and mobile.
---

## Features

1. **Smooth Gameplay**
   - Continuous card flipping without blocking while comparisons finish.
   - Flip, match, and mismatch animations.

2. **Dynamic Grid Support**
   - Adjustable rows/columns (2x2, 3x4, 5x6, etc.).
   - Automatic card scaling to fit the container using `GridBuilder`.

3. **Save/Load System**
   - Saves:
     - Current **sprite order** (`cardSpriteIndexes`)
     - Matched card indices (`matchedIndices`)
     - Score, turns, matches found, and best score
   - Loads exactly the same board layout and matched state after a restart or scene reload.
   - Prevents shuffle on reload unless starting a new game.

4. **Scoring System**
   - Base points per match
   - Combo bonus for consecutive matches
   - Tracks turns and matches found

5. **Sound Effects**
   - Flip
   - Match
   - Mismatch
   - Game Over

6. **Cross-Platform**
   - Runs on Desktop and Mobile (Android/iOS).

7. **Git Best Practices**
   - First commit is an empty Unity project.
   - Frequent commits with meaningful messages.

---

##  How to Run

1. Clone or download this repository.
2. Open in **Unity 2021+**.
3. Open:
Assets/Scenes/MainScene.unity
4. Press **Play**.

---

## Controls

- **Click/Tap** a card to flip it.
- Match two identical cards to score points.
- Game ends when all cards are matched.

---

##� Save/Load Logic

- Game saves automatically **after every turn**.
- Saves include:
- `cardSpriteIndexes` → exact order of sprites on the board
- `matchedIndices` → which cards are already matched
- `score`, `bestScore`, `turns`, `matchesFound`
- Saves stored at:
Application.persistentDataPath/cardmatch_save.json
- **On Reload:**  
- Board is rebuilt with same sprite order.  
- Matched cards are shown face-up.  
- Score and stats restored.
- **RestartGame()**:
- Deletes the save file
- Keeps `bestScore`
- Builds a fresh shuffled board

---

## Code Structure

- **GameManager.cs** — Main game logic: board setup, matching, scoring, saving/loading.
- **Card.cs** — Handles flip animations, matched state, and click handling.
- **GridBuilder.cs** — Configures `GridLayoutGroup` to scale cards to fit the board.
- **UIManager.cs** — Updates on-screen score, turns, and game over screen.
- **AudioManager.cs** — Plays sounds for flip, match, mismatch, and game over.

---

## Audio

Default sound effects are triggered on:
- Flip
- Match
- Mismatch
- Game Over

You can replace them in the **AudioManager** inspector.

---

## Customisation

- Change `rows` and `cols` in **GameManager** inspector for different grid sizes.
- Add more sprites to `allPossibleSprites` in the inspector.
- Tune difficulty with:
- `matchBasePoints`
- `mismatchDelay`
- `matchRevealDelay`

---

## Development Notes

- **Why save sprite order?**  
Without saving the order (`cardSpriteIndexes`), the game would reshuffle on reload, breaking the persistence of matched cards.
- **Why save after each turn?**  
Saves are updated after every turn to ensure progress is not lost if Unity stops unexpectedly.
- **Why use indices for sprites?**  
Saving indices instead of full sprite data keeps save files lightweight and avoids asset path issues.

---

## Platforms

- Windows / Mac / Linux
- Android / iOS

---

## License

This project is for educational purposes only.
