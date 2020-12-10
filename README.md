
# Diiagramr/Visual Drop
## Introduction 
Diiagramr is a basic diagram editor that allows you to create a diagram, add nodes to it, and connect nodes together using wires. Wires will transmit data between nodes. 

Visual Drop is the name I made up for the combination of Diiagramr and a few node libraries that together allow anyone to play with sound responsive graphic design. The libraries provide nodes that interface with the sound card, graphics card, and serial (USB), leaving exactly how all of these components get connected together up to the user.

tl;dr: Math is hard, music and lights are cool. This lets you do music and lights relatively easily and flexibly.

# Instructions
## 1. Opening the application ([video demonstration](https://www.youtube.com/watch?v=LhFYTrgOpNY))
1. Clone this repo
2. Open [Diiagramr.sln](https://github.com/Christian-Nunnally/Diiagramr/blob/master/Diiagramr.sln) in [Visual Studio](https://visualstudio.microsoft.com/downloads/)
3. Click run (F5)

## 2. Making a project
1. Click on 'New' to create a new project

## 3. Creating a node
1. Right click anywhere on the empty diagram to open the node menu
1. Select a node from the menu to create it
1. Click on the diagram to place the node

## 4. Wiring two nodes
1. Click on a terminal to select it (compatible terminals will highlight)
1. Click on a compatible terminal to wire the two terminals together

## Screenshots
![Start screen](/Images/visual-drop-start-screen.png) 
![A simple project with a spectrograph](/Images/visual-drop-spectrograph.png) 

## Tips
- Press 'Space' to open the node menu with a search bar to quickly add nodes
- Right click on a specific terminal to filter the node menu to only nodes that have compatible terminals
- Hold 'Alt' and drag a box around compatible nodes to automatically wire them from top to bottom
- Hold 'Shift' and click and drag to create space on the diagram
- Hold 'Ctrl' + 'Shift' and click and drag around nodes to create a new diagram containing those nodes, and place the diagram inside the current diagram.
- Hold 'Ctrl' + click and drag around nodes to select multiple nodes.
- Hold 'Ctrl' to prevent nodes from snapping to the grid when resized or moved.

# Contribute
If you would like to contribute to this project, please feel free to reach out to me and ask about any of the technologies used in the project or tell me about your idea.
