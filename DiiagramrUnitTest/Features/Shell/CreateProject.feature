Feature: CreateProject
	So that a user can begin creating
	they need to be able to start a new project
	the new project needs to be opened

Scenario: Create New Project
	Given the user launches the application
	When the new project button is pressed
	Then a new project is created

Scenario: Application opens correctly
	Given the user launches the application
	Then the save prompt is no longer visible