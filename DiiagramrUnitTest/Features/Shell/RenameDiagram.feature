Feature: RenameDiagram
	Because it would be silly if you couldn't
	users need to be able to rename diagrams

Scenario: Selecting a diagram
	Given a project is open
	When the project has 1 diagrams
	When the user selects the 0 diagram
	Then the 0 diagram is selected

Scenario: Renaming to a valid name
	Given a project is open
	When the project has 1 diagrams
	When the user selects the 0 diagram
	And the user tries to rename the selected item to 'renamedDiagram'
	Then the 0 diagram name is 'renamedDiagram'

Scenario: Renaming to an existing name
	Given a project is open
	When the project has 2 diagrams
	And the user selects the 0 diagram
	And the user tries to rename the selected item to 'renamedDiagram'
	And the user selects the 1 diagram
	And the user tries to rename the selected item to 'renamedDiagram'
	Then the 1 diagram name is not 'renamedDiagram'

Scenario: Remaning changes names in open diagrams
	Given a project is open
	When the project has 1 diagrams
	And the user selects the 0 diagram
	And the user tries to rename the selected item to 'renamedDiagram'
	Then the 0 diagram name is 'renamedDiagram'
