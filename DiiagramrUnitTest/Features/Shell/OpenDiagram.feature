Feature: OpenDiagram
	For users to edit diagrams
	they must be opened for editing

Scenario: The user opens a closed diagram
	Given a project is open
	When there is a diagram 'd1'
	And diagram 'd1' is selected
	Then the diagram well should open the selected diagram

Scenario: The diagram well opens the same diagram twice
	Given there is a diagram
	When the diagram is opened in the diagram well twice
	Then the diagram well only has one diagram open