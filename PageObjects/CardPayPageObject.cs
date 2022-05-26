using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;


namespace SpecFlowProject1.PageObjects;


public sealed class CardPayPageObject : BasePageObject
{
    
    public override string PagePath =>
        "https://hpp3-dev.caledoncard.com:3125/v3/pay?terminal_id=BPQATEST&reference_number=SPECFLOWCARDPAY&return_url=https://hpp3-dev.caledoncard.com:3125/testing&amount=150&tran_type=S";
    
    public CardPayPageObject(IBrowser browser)
    {
        Browser = browser;
    }
    
    public sealed override IPage Page { get; set; } 
    public override IBrowser Browser { get; }
    
    

    public async Task FillOutAddTransactionForm(string dataFilename)
    {
        
        
        var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Data",
            dataFilename);
        var companyData = JsonSerializer.Deserialize<CardPay>(await File.ReadAllTextAsync(path)) ??
                          throw new SerializationException("Could not deserialize file - " + path);

        
        
        await Page.FillAsync("#card_number", companyData.CardNumber!);
        await Page.FillAsync("#cvv", companyData.Ccs!);
        await Page.FillAsync("#street", companyData.StreetAddress!);
        await Page.FillAsync("#city", companyData.City!);
        await Page.FillAsync("#province", companyData.ProvinceState!);
        await Page.FillAsync("#postal", companyData.Postcode!);
        
        
        await Page.ClickAsync(@"#card_expiry\.month-button > span:nth-child(2)");
        await Page.ClickAsync("#ui-id-3");
        
        await Page.ClickAsync(@"#card_expiry\.year-button > span:nth-child(2)");
        await Page.ClickAsync("#ui-id-17");
    }  
    
    public Task ClickAddTransactionSubmit() => Page.ClickAsync("#Submit");
    
    public async Task CheckAddTransactionSuccessMessage()
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
    