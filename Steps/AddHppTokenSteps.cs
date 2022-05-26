using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.Steps;

[Binding]
public class AddHppTokenSteps
{
    private readonly AddHppTokenPageObject _pageObject;

    public AddHppTokenSteps(AddHppTokenPageObject pageObject)
    {
        _pageObject = pageObject;
    }

    [When(@"the user navigates to the HPP AddToken form")]
    public async  Task WhenTheUserNavigatesToTheHppAddTokenForm()
    {
        await _pageObject.ClickAddToken();
    }

    [When(@"submits the AddToken entry")]
    public async Task WhenSubmitsTheAddTokenEntry(Table table)
    {
        var dataFilename = table.Rows[0][0];

        await _pageObject.FillOutAddTokenForm(dataFilename);
        await _pageObject.ClickAddTokenSubmit();
    }

    [Then(@"the token is successfully added")]
    public async Task ThenTheTokenIsSuccessfullyAdded()

    {
        await _pageObject.CheckAddTokenSuccessMessage();
    }
} 
    