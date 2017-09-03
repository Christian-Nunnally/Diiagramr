Feature: LoadProject
	So that users can save and share projects
	The projects need to be able to be loaded
	Loaded projects should be just as they were when they were saved

Scenario: The user can select a single project to load
	Given the user launches the application
	And there is a project in the project directory named 'project1'
	When the open button is pressed
	Then there is a 'project1' button in the open menu

Scenario: The user can select multiple projects to load
	Given the user launches the application
	And there is a project in the project directory named 'project1'
	And there is a project in the project directory named 'project2'
	When the open button is pressed
	Then there is a 'project1' button in the open menu
	And there is a 'project2' button in the open menu

Scenario: The user loads a project
	Given the user launches the application
	And there is no open project
	And there is a project in the project directory named 'project1'
	When the user loads project 'project1'
	Then the project load will be requested for project 'project1'
	And the project will be loaded
