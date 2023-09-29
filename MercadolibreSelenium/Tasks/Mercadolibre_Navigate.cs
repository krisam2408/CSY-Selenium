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
        .GoToUrl("https://www.mercadolibre.cl");

      return Task.CompletedTask;
  }
}