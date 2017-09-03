Feature: AddWireFeature
	In order to connect things on a diagram
	wires need to be created between terminals

Scenario: Output terminal dropped on input terminal
	Given I have an output terminal
	And I have an input terminal
	When I drop the output terminal on the input terminal
	Then The output terminal should be wired to the input terminal

Scenario: Input terminal dropped on output terminal
	Given I have an output terminal
	And I have an input terminal
	When I drop the input terminal on the output terminal
	Then The output terminal should be wired to the input terminal
