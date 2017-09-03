Feature: DeleteDiagram
	The user should be able to remove diagrams from thier project
	By doing something like right clicking it and pressing delete

Scenario: Selecting a diagram
	Given a project is open
	When the project has 1 diagrams
	And the user selects the 0 diagram
	And the user presses the delete diagram button
	Then the project has 0 diagrams
