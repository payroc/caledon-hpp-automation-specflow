using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.Steps;

[Binding]
public class CardPaySteps
{
    private readonly CardPayPageObject _pageObject;

    public CardPaySteps(CardPayPageObject pageObject)
    {
        _pageObject = pageObject;
    }


    [When(@"the user navigates to the HPP CardPay form")]
    public async Task WhenTheUserNavigatesToTheTransactionEntryForm()
    {
        await _pageObject.ClickAddTransaction();
    }

    [When(@"submits the CardPay entry")]
    public async Task WhenEntersAndSubmitsTheTransactionEntry(Table table)
    {
        var dataFilename = table.Rows[0][0];

        await _pageObject.FillOutAddTransactionForm(dataFilename);
        await _pageObject.ClickAddTransactionSubmit();
    }

    [Then(@"the transaction is successfully added")]

    public async Task ThenTheTransactionIsSuccessfullyAdded()

    {
        await _pageObject.CheckAddTransactionSuccessMessage();
    }

}