using Microsoft.Playwright;

namespace SpecFlowProject1.PageObjects
{
    public abstract class BasePageObject
    {
        public abstract string PagePath { get; }
    
        public abstract IPage Page { get; set; }

        public abstract IBrowser Browser { get; }

        public async Task NavigateAsync()
        {
            Page = await Browser.NewPageAsync();
            await Page.GotoAsync(PagePath);
        }
    }
}