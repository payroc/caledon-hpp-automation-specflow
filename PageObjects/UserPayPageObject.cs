using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;


namespace SpecFlowProject1.PageObjects;


public sealed class UserPayPageObject : BasePageObject
{
    public override string PagePath =>
        "https://hpp3-dev.caledoncard.com:3125/v3/userpay?terminal_id=BPQACONV&reference_number=12345&return_url=https://hpp3-dev.caledoncard.com:3125/testing&amount=10000&tran_type=S&avs=N&cvv=N&token=FIREFOXADD1&token_action=ADD";

    //add link for token pay here?

    public override IPage Page { get; set; }
    public override IBrowser Browser { get; }


    public UserPayPageObject(IBrowser browser)
    {
        Browser = browser;
    }
    

    public async Task EntersUserPayDetails()
    {
    

    
    await Page.ClickAsync("#Submit");
    await Page.FillAsync("#fee_amount", "195");
    await Page.ClickAsync("#fee_accept");
    await Page.ClickAsync("#Submit");
    }

public async Task CheckUserPaySuccessMessage()
    {
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/Users/stevenmurdoch/Documents/PlaywrightScreenshots\\cardpay_confirmation.png" });

        var feedbackText = await Page.EvalOnSelectorAsync<string>("pre", "el => el.innerText");
        var expectedText = "auth_result      => \"A\"";
        feedbackText.Should().Contain(expectedText);

    }

    public async Task ClickAddTransaction()
    {
        await NavigateAsync();
        await Page.ClickAsync(
            "#toplogo");
    }


}
    