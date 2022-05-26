using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.Steps;

[Binding]
public class UpdateHppTokenSteps
{
    private readonly UpdateHppTokenPageObject _pageObject;

    public UpdateHppTokenSteps(UpdateHppTokenPageObject pageObject)
    {
        _pageObject = pageObject;
    }

    [When(@"the user navigates to the HPP UpdateToken form")]
    public async Task WhenTheUserNavigatesToTheHppUpdateTokenForm()
    {
        await _pageObject.ClickUpdateToken();
    }

    [When(@"submits the UpdateToken entry")]
    public async Task WhenSubmitsTheUpdateTokenEntry(Table table)
    {
        var dataFilename = table.Rows[0][0];

        await _pageObject.FillOutUpdateTokenForm(dataFilename);
        await _pageObject.ClickUpdateTokenSubmit();
    }

    [Then(@"the token is successfully updated")]
    
    public async Task ThenTheTokenIsSuccessfullyAdded()

    {
        await _pageObject.CheckUpdateTokenSuccessMessage();
    }
}
    