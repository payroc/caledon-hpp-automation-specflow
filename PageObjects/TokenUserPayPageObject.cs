using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Playwright;
using SpecFlowProject1.Helpers;


namespace SpecFlowProject1.PageObjects;


public sealed class TokenUserPayPageObject : BasePageObject
{
    public override string PagePath =>
        "https://hpp3-dev.caledoncard.com:3125/v3/tokenuserpay?terminal_id=BPQACONV&fixed_amount=N&token=LASTSPEC3&tran_type=S&reference_number=THURS3&return_url=https://hpp3-dev.caledoncard.com:3125/testing&amount=100000&avs=N&cvv=N&token_action=UPDATE";
    

    
    public override IPage Page { get; set; } 
    public override IBrowser Browser { get; }

    
    public TokenUserPayPageObject(IBrowser browser)
    {
        Browser = browser;
    }

    public async Task FillOutTokenUserPayForm(string dataFilename)
    {
        
        
        var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Data",
            dataFilename);
        var companyData = JsonSerializer.Deserialize<TokenUserPay>(await File.ReadAllTextAsync(path)) ??
                          throw new SerializationException("Could not deserialize file - " + path);

        
        
        await Page.FillAsync("#card_number", companyData.CardNumber!);
        //await Page.FillAsync("#amount", companyData.Amount!);
        await Page.ClickAsync(@"#card_expiry\.month-button > span:nth-child(2)");
        await Page.ClickAsync(@"#card_expiry\.year-button > span:nth-child(2)");
        await Page.ClickAsync("#Submit");
        
        await Page.FillAsync("#fee_amount", companyData.FeeAmount!);
        await Page.ClickAsync("#fee_accept");
        
        
    }  
    
    public Task ClickTokenUserPaySubmit() => Page.ClickAsync("#Submit");
    
    public async Task CheckTokenUserPaySuccessMessage()
    {
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/Users/stevenmurdoch/Documents/PlaywrightScreenshots\\tokenuserpay_confirmation.png" });

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
    