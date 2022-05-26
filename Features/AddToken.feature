Feature: Add HPP Token
This page allows a transaction to be added

   @AddToken
	Scenario:Add a HPP Token
		When the user navigates to the HPP AddToken form
		And submits the AddToken entry
		  | dataFile     |
		  | AddToken.json |
		Then the token is successfully added