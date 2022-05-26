Feature: Pay
This page allows a transaction to be added

   @CardPay   
    Scenario:Process a Card Pay Transaction
        When the user navigates to the HPP CardPay form
        And submits the CardPay entry
          | dataFile              |
          | CardPay.json          |
        Then the transaction is successfully added
        
    @TokenPay
    Scenario:Process a TokenPay Transaction
        When the user navigates to the HPP TokenPay form
        And submits the TokenPay entry
          | dataFile     |
          | CardPay.json |
        Then the TokenPay transaction is successfully added
    
    @UserPay
    Scenario:Process a UserPay Transaction
        When the user navigates to the HPP UserPay form
        And enters fee amount and ticks the confirmation tickbox
        Then the UserPay transaction is successfully added      
        
    @TokenUserPay    
    Scenario:Process a TokenUserPay Transaction
        When the user navigates to the HPP TokenUserPay form
        And submits the TokenUserPay entry
          | dataFile     |
          | TokenUserPay.json |
        Then the TokenUserPay transaction is successfully added  
        
        
        