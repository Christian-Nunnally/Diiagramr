Feature: RenameProject
	Because it would be silly if you couldn't
	users need to be able to rename thier project

Scenario: Selecting a project
	Given a project is open
	When the user selects a project
	Then that project is selected in the view model

Scenario: Renaming to a valid name
	Given a project is open
	When that project is selected
	And the user tries to rename the selected item to 'test'
	Then the project name is changed to 'test'

Scenario: Renaming to an existing name
	Given a project is open
	When that project is selected
	And there is another project with the name 'test'
	And the user tries to rename the selected item to 'test'
	Then the project name is not changed