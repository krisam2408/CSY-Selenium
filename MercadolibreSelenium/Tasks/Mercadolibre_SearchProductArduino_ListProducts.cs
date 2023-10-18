using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_SearchProductArduino_ListProducts : BaseTask
{
    protected override int TestId => 3;
    protected override string TestName => "Buscar Producto Arduino";

    public Mercadolibre_SearchProductArduino_ListProducts(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if(!await Precondition())
            return;

        IWebElement searcher = Driver.FindElement(By.Id("cb1-edit"));
        searcher.SendKeys("arduino");
        searcher.SendKeys(Keys.Enter);

        await PostCondition();
    }

    protected override async Task PostCondition()
    {
        async Task assert(int v)
        {
            if(v > 40)
            {
                await Assert(true);
                return;
            }
            await Assert(false);
        }

        IWebElement results = Driver.FindElement(By.ClassName("ui-search-search-result__quantity-results"));
        string[] split = results.Text.Split(" ");
        if (int.TryParse(split[0].Replace(".", ""), out int value))
        {
            await Terminal.WriteAsync($"Se encontraron {value} resultados", TerminalColor.Yellow);
            await assert(value);
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
