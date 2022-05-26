using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.PageObjects;

public sealed class AddHppTokenPageObject : BasePageObject
{
    public override string PagePath =>
        "https://hpp3-dev.caledoncard.com:3125/v3/token?terminal_id=BPQATEST&reference_number=12345&card_type=V&cvv=N&avs=N&cardholder_name=N&locale=en-ca&token=SPECFIGURE&token_action=ADD&return_url=https://hpp3-dev.caledoncard.com:3125/testing";
    
    public override IPage Page { get; set; } 
    public override IBrowser Browser { get; }

    
    public AddHppTokenPageObject(IBrowser browser)
    {
        Browser = browser;
    }

    public async Task FillOutAddTokenForm(string dataFilename)
    {
        
        
        var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Data",
            dataFilename);
        var companyData = JsonSerializer.Deserialize<CardPay>(await File.ReadAllTextAsync(path)) ??
                          throw new SerializationException("Could not deserialize file - " + path);

        
        
        await Page.FillAsync("#card_number", companyData.CardNumber!);


        await Page.ClickAsync(@"#card_expiry\.month-button > span:nth-child(2)");
        await Page.ClickAsync(@"#ui-id-2");
        
        await Page.ClickAsync(@"#card_expiry\.year-button > span:nth-child(2)");
        await Page.ClickAsync(@"#ui-id-17");
    }  
    
    public Task ClickAddTokenSubmit() => Page.ClickAsync("#Submit");
    
    public async Task CheckAddTokenSuccessMessage()
    {


        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/Users/stevenmurdoch/Documents/PlaywrightScreenshots\\addtoken_confirmation.png" });

        var feedbackText = await Page.EvalOnSelectorAsync<string>("pre", "el => el.innerText");
        var expectedText = "TOKEN ADDED         ";
        feedbackText.Should().Contain(expectedText);

    }

    public async Task ClickAddToken()
    {
        await NavigateAsync();
        await Page.ClickAsync(
            "#toplogo");
    }


}