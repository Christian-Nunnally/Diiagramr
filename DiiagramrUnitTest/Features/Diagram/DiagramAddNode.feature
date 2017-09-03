Feature: DiagramAddNode
	Inserting a node onto the diagram
	requires giving the diagram a node to place
	and then telling it to place it at a certain position

Scenario: I click a diagram while a node is being inserted
	Given I have a diagram
	And the diagram has 0 nodes
	And a demo node is being inserted to the diagram
	When the user clicks the diagram
	Then the diagram has 1 node
	And the diagrams inserting node is null

Scenario: I click a diagram while a node is not being inserted
	Given I have a diagram
	And the diagram has 0 nodes
	And no node is being inserted to the diagram
	When the user clicks the diagram
	Then the diagram has 0 node
