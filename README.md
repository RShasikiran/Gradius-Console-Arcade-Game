# Gradius-Console-Arcade-Game
A C# terminal-based arcade engine implementing OOP design patterns, double-buffering for flicker-free rendering, and persistent data management.

## 🚀 Project Overview
Gradius Elite is a terminal-based side-scrolling shooter developed in C#. The project demonstrates the application of Object-Oriented Programming (OOP) principles, efficient memory management, and real-time game loop architecture in a console environment.

## 🛠 Technical Features
* **OOP Architecture:** Implements Inheritance and Polymorphism for game entities (Player, Enemies, Bonuses).
* **State Management:** Finite State Machine (FSM) to handle Main Menu, Gameplay, and Game Over states.
* **Rendering Engine:** Uses a Custom Double-Buffering technique with `StringBuilder` to eliminate screen flickering.
* **Collision Detection:** Proximity-based AABB collision algorithms.
* **Data Persistence:** File-based High Score system to track player performance.

## 🕹 How to Run
1. Ensure you have the **.NET SDK** installed.
2. Clone this repository: `git clone [YOUR_REPO_LINK]`
3. Navigate to the folder and run: `dotnet run`
4. Controls: 
   - `W / Up Arrow`: Move Up
   - `S / Down Arrow`: Move Down
   - `Spacebar`: Fire Lasers

## 📊 System Architecture
The project follows a modular design where the `GradiusElite_Project` namespace separates logic from data.
