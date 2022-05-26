using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.Steps;

[Binding]
public class TokenPaySteps
{
    private readonly TokenPayPageObject _pageObject;

    public TokenPaySteps(TokenPayPageObject pageObject)
    {
        _pageObject = pageObject;
    }


    [When(@"the user navigates to the HPP TokenPay form")]
    public async Task WhenTheUserNavigatesToTheHppTokenPayForm()
    {
        await _pageObject.ClickAddTransaction();
    }


    [When(@"submits the TokenPay entry")]
    public async Task WhenSubmitsTheTokenPayEntry(Table table)
    {
        var dataFilename = table.Rows[0][0];

        await _pageObject.FillOutAddTransactionForm(dataFilename);
        await _pageObject.ClickAddTransactionSubmit();
    }

    [Then(@"the TokenPay transaction is successfully added")]
    public async Task ThenTheTokenPayTransactionIsSuccessfullyAdded()

    {
        await _pageObject.CheckAddTransactionSuccessMessage();
    }

}