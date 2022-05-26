using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.PageObjects;

public sealed class UpdateHppTokenPageObject : BasePageObject
{
    public override string PagePath =>
        "https://hpp3-dev.caledoncard.com:3125/v3/token?terminal_id=BPQATEST&reference_number=12345&card_type=V&cvv=N&avs=N&cardholder_name=N&locale=en-ca&token=SPEC2&token_action=UPDATE&return_url=https://hpp3-dev.caledoncard.com:3125/testing";
    
    public override IPage Page { get; set; } 
    public override IBrowser Browser { get; }

    
    public UpdateHppTokenPageObject(IBrowser browser)
    {
        Browser = browser;
    }

    public async Task FillOutUpdateTokenForm(string dataFilename)
    {
        
        
        var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Data",
            dataFilename);
        var companyData = JsonSerializer.Deserialize<UpdateToken>(await File.ReadAllTextAsync(path)) ??
                          throw new SerializationException("Could not deserialize file - " + path);

        
        
        await Page.FillAsync("#card_number", companyData.CardNumber!);
        
        await Page.ClickAsync(@"#card_expiry\.month-button > span:nth-child(2)");
        await Page.ClickAsync(@"#ui-id-2");
        
        await Page.ClickAsync(@"#card_expiry\.year-button > span:nth-child(2)");
        await Page.ClickAsync(@"#ui-id-17");


        //await Page.FillAsync("#cvv", companyData.Ccs!);
        //await Page.FillAsync("#street", companyData.StreetAddress!);
        //await Page.FillAsync("#city", companyData.City!);
        //await Page.FillAsync("#province", companyData.ProvinceState!);
        //await Page.FillAsync("#postal", companyData.Postcode!);
    }  
    
    public Task ClickUpdateTokenSubmit() => Page.ClickAsync("#Submit");
    
    public async Task CheckUpdateTokenSuccessMessage()
    {
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/Users/stevenmurdoch/Documents/PlaywrightScreenshots\\updatetoken_confirmation.png" });

        var feedbackText = await Page.EvalOnSelectorAsync<string>("pre", "el => el.innerText");
        var expectedText = "auth_result      => \"A\"";
        feedbackText.Should().Contain(expectedText);

    }

    public async Task ClickUpdateToken()
    {
        await NavigateAsync();
        await Page.ClickAsync(
            "#toplogo");
    }


}