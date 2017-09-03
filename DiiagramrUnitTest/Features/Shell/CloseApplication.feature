Feature: CloseApplication
	When the user closes the application
	we should make sure we clean up everything properly

	Scenario: The application is closing and a project is open and has been saved
	Given the user launches the application
	And a project is open
	When the project is in a clean state
	When they press the close button
	Then the application closes

Scenario: The application is closing and a project is open
	Given the user launches the application
	And a project is open
	When the project is in a dirty state
	When they press the close button
	Then the application prompts the user to save

Scenario: The application is closing and a project is not open
	Given the user launches the application
	When they press the close button
	Then the application closes

Scenario: The user does not want to save
	Given the user launches the application
	And a project is open
	When the user presses the dont save before closing button
	Then the project is closed
	And the application closes

Scenario: The user does want to save
	Given the user launches the application
	And a project is open
	When the user presses the save before closing button
	Then the project is saved
	And the project is closed
	And the application closes

Scenario: The user cancels the close
	Given the user launches the application
	And a project is open
	When the user presses the cancel button
	Then the save prompt is no longer visible




	
