Feature: Update Token
This page allows a token to be updated

   @UpdateToken
	Scenario:Update a HPP Token
		When the user navigates to the HPP UpdateToken form
		And submits the UpdateToken entry
		  | dataFile         |
		  | UpdateToken.json |
		Then the token is successfully updated