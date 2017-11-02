You are ready to start developing your own Diiagramr library.

This README will get you started.  For more advanced topics, refer to this wiki: https://christiannunnally.visualstudio.com/Diiagramr/Diiagramr%20Team/_wiki 

----------------------------------- Overview of Nodes -----------------------------------

Nodes are comprised of two files:

	View	  - This file is named <YourNodeName>View.xaml
			  - It describes the visual appearance of your node 
			  - It has bindings to commands and properties to the ViewModel of your node

	ViewModel - This file is named <YourNodeName>ViewModel.cs
			  - It contains a called called <YourNodeName>ViewModel
			  - The class inherits from the PluginNode class
			  - It contains the functionality of your node
			  - Input and outputs of your node are described here

PluginNode is the base class of all nodes. It has one method that you must override, SetupNode, which allows you to opt in to features for your node.
The SetupNode method gives you a NodeSetup object which has methods you can optionally invoke to turn on and off features and set up your node.  The basic features are:

	setup.NodeSize(int, int)   				   - Sets the width and height of your node
	setup.NodeName(string)					   - Sets the name of your node
	setup.InputTerminal<T>(string, Direction)  - Creates a new input terminal on your node. 
	                                             The terminal will support data of type T, will have the given name, and will show up on your node facing the given Direction.
												 The returned terminal has a ValueChanged event that you should subscribe to know when the input data has changed and to react to it.
	setup.OutputTerminal<T>(string, Direction) - Creates a new output terminal on your node. 
	                                             The terminal will support data of type T, will have the given name, and will show up on your node facing the given Direction.
												 The returned terminal has a Value event that you should set when you want nodes wired to that output to see that data.											 

Each node has:

	Inputs	  - Where data enters your node
			  - That have a typed Value field that stores the value of the input
			  - That have a ValueChanged event that your node subscribes to in order to react to the value changing

	Outputs   - Where your node puts data
			  - That has a typed Value field that your node should set when it wants to output data

The rest is up to you.

----------------------------------- Publishing Your First Diiagramr Library -----------------------------------

Follow these simple steps to publish a simple node that simply forwards the data from its input to its output

1. In 'ExampleNodeView' replace <YOUR NAMESPACE> with the namespace of your project
2. In 'ExampleNodeViewModel' replace <YOUR NAMESPACE> with the namespace of your project
3. Observe the code in both these files.  This is a very simple node.
4. Build the project
5. Go to Tools -> NuGet Package Manger -> Package Manager Console
6. Type "cd <Your project name>"
7. Type "Publish-NuGet" and follow the prompts

Congratulations! Your library is published!
