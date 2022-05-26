using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.Steps;

[Binding]
public class TokenUserPaySteps
{
    private readonly TokenUserPayPageObject _pageObject;

    public TokenUserPaySteps(TokenUserPayPageObject pageObject)
    {
        _pageObject = pageObject;
    }


    [When(@"the user navigates to the HPP TokenUserPay form")]
    public async Task WhenTheUserNavigatesToTheHppTokenUserPayForm()
    {
        await _pageObject.ClickAddTransaction();
    }

    [When(@"submits the TokenUserPay entry")]
    
    public async Task WhenSubmitsTheTokenUserPayEntry(Table table)
    {
        var dataFilename = table.Rows[0][0];

        await _pageObject.FillOutTokenUserPayForm(dataFilename);
        await _pageObject.ClickTokenUserPaySubmit();
    }

    [Then(@"the TokenUserPay transaction is successfully added")]
    public async Task ThenTheTokenUserPayTransactionIsSuccessfullyAdded()
    {
        await _pageObject.CheckTokenUserPaySuccessMessage();
    }
}