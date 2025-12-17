# Project Brief: HyperConnectX

## Overview
HyperConnectX (also called ConnectX) is a customizable Connect Four-style board game implemented in C#/.NET. The game allows players to configure board dimensions, win conditions, and supports multiple game modes including player vs player, player vs AI, and AI vs AI.

## Core Requirements
- Configurable board size (3-20 columns/rows)
- Configurable win condition ("Connect How Many" - from 2 to board minimum dimension)
- Multiple game modes: PvP, PvC (Player vs Computer), CvC (Computer vs Computer)
- Optional cylindrical board mode (edges wrap around)
- Save/load game states
- Save/load game configurations
- AI opponent using Minimax algorithm with alpha-beta pruning

## Target Platform
- Console application for .NET 10.0
- Cross-platform (uses user home directory for data storage)

## Project Context
This appears to be an educational/academic project (indicated by the `icd0008-25f` in the path, likely a course code at TalTech - Tallinn University of Technology).

## Notes
- The project uses Entity Framework Core with SQLite for database persistence
- JSON file storage is also supported as an alternative repository implementation
