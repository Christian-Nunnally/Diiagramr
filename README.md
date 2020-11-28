
# Diiagramr/Visual Drop
## Introduction 
Diiagramr is a basic diagram editor that allows you to create a diagram, add nodes to it, and connect nodes together using wires. Wires will transmit data between nodes. 

Visual Drop is the name I made up for the combination of Diiagramr and a few node libraries that together allow anyone to play with sound responsive graphic design. The libraries provide nodes that interface with the sound card, graphics card, and serial (USB), leaving exactly how all of these components get connected together up to the user.

tl;dr: Math is hard, music and lights are cool. This lets you do music and lights experimentially.

# Installation
- I'm currently not hosting a built version of this app, so you'll have to clone the repo and build the souce with Visual Studio if you want to mess around with it.

# Getting Started
- Making Your First Diagram
1. Run Diiagramr.
2. Click on 'New' to create a new project.
3. Right click to see list of libraries.
4. Mouse over library title to see nodes in that library.
5. Click on node title to add a new instance of that type of node to the diagram.
6. Click on the diagram to place the new node under the mouse cursur.
7. Click on a terminal to select it (compatible terminals will also highlight).
8. Click on a compatible terminal to wire the two terminals together.

# Build and Test
To build the project:
1. Open Diiagramr.sln with Visual Studio 2019 Community Edition (https://www.visualstudio.com/downloads/).
2. Set 'Diiagramr' as the start up project.
3. Build or start debugging.

To test the project:
1. Open Diiagramr.sln with Visual Studio 2019 Community Edition (https://www.visualstudio.com/downloads/).
2. Build 'DiiagramrIntegrationTest' and 'DiiagramrUnitTest'.
3. Run all tests with your favorite test runner.

# Contribute
If you would like to contribute to this project, please feel free to reach out to me and ask about any of the technologies used in the project or tell me about your idea.