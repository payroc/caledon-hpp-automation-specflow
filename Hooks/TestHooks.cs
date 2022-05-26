using System.ComponentModel;
using BoDi;
using Microsoft.Playwright;
using SpecFlowProject1.PageObjects;
using TechTalk.SpecFlow;

namespace SpecFlowProject2.Hooks;

[Binding]
public class TestHooks
{
    [BeforeScenario("CardPay")]
    public async Task BeforeCardPayScenario(IObjectContainer container)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless  = false,
            SlowMo = 2000
        });
        var pageObject = new CardPayPageObject(browser);
        
        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);
        container.RegisterInstanceAs(pageObject);
    }
    
    [BeforeScenario("TokenPay")]
    public async Task BeforeTokenPayScenario(IObjectContainer container)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless  = false,
            SlowMo = 2000
        });
        var pageObject = new TokenPayPageObject(browser);
        
        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);
        container.RegisterInstanceAs(pageObject);
    }
    
    [BeforeScenario("UserPay")]
    public async Task BeforeUserPayScenario(IObjectContainer container)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless  = false,
            SlowMo = 2000
        });
        var pageObject = new TokenPayPageObject(browser);
        
        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);
        container.RegisterInstanceAs(pageObject);
    }
    
    [BeforeScenario("TokenUserPay")]
    public async Task BeforeTokenUserPayScenario(IObjectContainer container)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless  = false,
            SlowMo = 2000
        });
        var pageObject = new TokenPayPageObject(browser);
        
        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);
        container.RegisterInstanceAs(pageObject);
    }
    
    [BeforeScenario("AddToken")]
    public async Task BeforeAddTokenScenario(IObjectContainer container)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless  = false,
            SlowMo = 2000
        });
        var pageObject = new TokenPayPageObject(browser);
        
        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);
        container.RegisterInstanceAs(pageObject);
    }
    
    [BeforeScenario("UpdateToken")]
    public async Task BeforeUpdateTokenScenario(IObjectContainer container)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless  = false,
            SlowMo = 2000
        });
        var pageObject = new TokenPayPageObject(browser);
        
        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);
        container.RegisterInstanceAs(pageObject);
    }
    
    
    
    

    [AfterScenario]

    public async Task AfterScenario(IObjectContainer container)
    {
        var browser = container.Resolve<IBrowser>();
        await browser.CloseAsync();
        var playwright = container.Resolve<IPlaywright>();
        playwright.Dispose();
        
    }
}