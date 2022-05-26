using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;
using SpecFlowProject1.PageObjects;

namespace SpecFlowProject1.PageObjects;

//Request was declined

public sealed class TokenPayPageObject : BasePageObject
{
    
    //requires convfee? and unique tokenid within url 
    public override string PagePath =>
        "https://hpp3-dev.caledoncard.com:3125/v3/tokenpay?terminal_id=BPQACONV&tran_type=S&reference_number=THURS3&return_url=https://hpp3-dev.caledoncard.com:3125/testing&amount=100000&token_action=ADD&token=LASTSPEC3&avs=N&cvv=N";
    
    
    
    public override IPage Page { get; set; } 
    public override IBrowser Browser { get; }

    
    public TokenPayPageObject(IBrowser browser)
    {
        Browser = browser;
    }

    public async Task FillOutAddTransactionForm(string dataFilename)
    {
        
        
        var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Data",
            dataFilename);
        var companyData = JsonSerializer.Deserialize<CardPay>(await File.ReadAllTextAsync(path)) ??
                          throw new SerializationException("Could not deserialize file - " + path);

        
        
        await Page.FillAsync("#card_number", companyData.CardNumber!);
        //await Page.FillAsync("#cvv", companyData.Ccs!);
        //await Page.FillAsync("#street", companyData.StreetAddress!);
        //await Page.FillAsync("#city", companyData.City!);
        //await Page.FillAsync("#province", companyData.ProvinceState!);
        //await Page.FillAsync("#postal", companyData.Postcode!);
        
        
        await Page.ClickAsync(@"#card_expiry\.month-button > span:nth-child(2)");
        await Page.ClickAsync("#ui-id-3");
        
        await Page.ClickAsync(@"#card_expiry\.year-button > span:nth-child(2)");
        await Page.ClickAsync("#ui-id-17");
        
        //await Page.SelectOptionAsync("select[id=\"#vt_terminalId_ms\"]", companyData.TerminalId!);
        //await Page.SelectOptionAsync("#transType_ms > span.ui-icon.ui-icon-triangle-1-s", companyData.TransactionType!);
    }  
    
    public Task ClickAddTransactionSubmit() => Page.ClickAsync("#Submit");
    
    public async Task CheckAddTransactionSuccessMessage()
    {
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/Users/stevenmurdoch/Documents/PlaywrightScreenshots\\tokenpay_confirmation.png" });
        
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
    