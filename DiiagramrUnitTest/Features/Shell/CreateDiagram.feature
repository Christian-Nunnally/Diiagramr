Feature: CreateDiagram
	For a user to begin creating
	they need to be able to create a new diagram

Scenario: The user can create a new diagram
	Given a project is open
	When the project has 0 diagrams
	And the new diagram button is pressed
	Then the project has 1 diagrams
	And the diagram is named 'diagram1'

Scenario: The user can create a second new diagram
	Given a project is open
	When the project has 0 diagrams
	And the new diagram button is pressed
	Then the project has 2 diagrams
	And the second diagram is named 'diagram2'