using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.Steps;

[Binding]
public class UserPaySteps
{
    private readonly UserPayPageObject _pageObject;

    public UserPaySteps(UserPayPageObject pageObject)
    {
        _pageObject = pageObject;
    }


    [When(@"the user navigates to the HPP UserPay form")]
    
    public async Task WhenTheUserNavigatesToTheHppUserPayForm()
    {
        await _pageObject.ClickAddTransaction();
    }

    [When(@"enters fee amount and ticks the confirmation tickbox")]
    public async Task WhenEntersFeeAmountAndTicksTheConfirmationTickbox()
    {
        await _pageObject.EntersUserPayDetails();
    }

    [Then(@"the UserPay transaction is successfully added")]
    public async Task ThenTheUserPayTransactionIsSuccessfullyAdded()
    {
        await _pageObject.CheckUserPaySuccessMessage();
    }


}