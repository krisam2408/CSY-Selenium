using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_SearchImpossibleProduct_ListNothing : BaseTask
{
    protected override int TestId => 2;
    protected override string TestName => "Buscar Producto Imposible";

    public Mercadolibre_SearchImpossibleProduct_ListNothing(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if(!await Precondition())
            return;

        IWebElement searcher = Driver.FindElement(By.Id("cb1-edit"));
        searcher.SendKeys("kjhklfasdhkfjasdifasdkjfnasdklfjnasdkjlf");
        searcher.SendKeys(Keys.Enter);

        await PostCondition();
    }

    protected override async Task PostCondition()
    {
        IWebElement results = Driver.FindElement(By.ClassName("ui-search-rescue__title"));
        
        if (results.Text.ToLower().Contains("no hay publicaciones"))
        {
            await Assert(true);
            return;
        }

        await base.PostCondition();
    }

    protected override async Task<bool> Precondition()
    {
        try
        {
            Driver.Navigate()
                .GoToUrl(HostUrl);

            await Task.Delay(16);
        }
        catch
        {
            return await base.Precondition();
        }
        return true;
    }
}
