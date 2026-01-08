# Product: HyperConnectX

## Purpose
HyperConnectX is a highly customizable Connect Four game designed to provide flexibility beyond the traditional 7x6 board with 4-in-a-row win condition. It serves as both an entertaining game and an educational project demonstrating software architecture patterns.

## Problems It Solves
1. **Limited Customization**: Traditional Connect Four games have fixed rules. HyperConnectX allows varying board sizes (3-20 columns/rows) and win conditions (2 to min board dimension).
2. **Single Player Experience**: Provides intelligent AI opponent using Minimax algorithm with alpha-beta pruning for challenging gameplay.
3. **Game Persistence**: Players can save and resume games, as well as save custom configurations for reuse.
4. **Exploring Different Strategies**: Cylindrical board mode changes strategy dynamics by allowing pieces to wrap around edges.

## How It Works

### Game Flow
1. User launches the console or web application
2. Main menu offers: New Game, Settings, Save/Load Configuration, Load/Delete Saved Games
3. Settings allow customization of board rules (size, connect requirement, cylindrical mode)
4. When starting a new game, user enters player names and selects game mode
5. Game starts with selected configuration
6. Players take turns dropping pieces into columns
7. First to connect the required number of pieces wins (or draw if board fills)
8. Game can be saved at any point

### User Experience Goals
- **Intuitive Interface**: Arrow key navigation in console; click-based in web
- **Visual Feedback**: Animated piece dropping effect when placing pieces (console app)
- **Clear Game State**: Board displayed with row/column numbers, current player indicator
- **Persistent Data**: Games and configurations survive between sessions
- **Flexible Storage**: Supports both JSON files and SQLite database backends

## Game Modes
| Mode | Description |
|------|-------------|
| PvP | Two human players take turns |
| PvC | Human player vs AI (AI is always O/Player 2) |
| CvC | Two AI opponents compete (for demonstration/testing) |

## Configuration Options (saved in GameConfiguration)
| Setting | Range | Default |
|---------|-------|---------|
| Board Width | 3-20 | 7 |
| Board Height | 3-20 | 6 |
| Connect How Many | 2 to min(width,height) | 4 |
| Cylindrical Mode | On/Off | Off |

## Game Options (set at game creation, saved in GameState)
| Setting | Range | Default |
|---------|-------|---------|
| Player 1 Name | Any string (1-50 chars) | "Player 1" |
| Player 2 Name | Any string (1-50 chars) | "Player 2" |
| Game Mode | PvP/PvC/CvC | PvP |
| AI Difficulty | Easy/Medium/Hard | Hard |
