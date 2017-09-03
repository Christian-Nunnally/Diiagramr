Feature: SaveProject
	So users can halt and resume work
	they need to be able to save thier projects

Scenario: The project is saved
	Given the user launches the application
	And a project is open
	When the project is saved
	Then the project saved indicator is true
	And the project is saved

Scenario: The project is changed
	Given the user launches the application
	And a project is open
	When the project is changed
	Then the project saved flag is false

