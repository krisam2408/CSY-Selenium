using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_Navigate : BaseTask
{
    protected override int TestId => 0;
    protected override string TestName => "Navegar a MercadoLibre";

    public Mercadolibre_Navigate(IWebDriver driver) : base(driver) { }

    public override Task ExecuteAsync()
    {
        Driver.Navigate()
            .GoToUrl(HostUrl);

        return Task.CompletedTask;
    }

    protected override Task<bool> Precondition()
    {
        throw new NotImplementedException();
    }

    protected override Task PostCondition()
    {
        throw new NotImplementedException();
    }
}