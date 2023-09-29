using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_Login_FailedByUsername : BaseTask
{
  protected override int TestId => 1;
  protected override string TestName => "Login fallido por usuario inexistente";

  public Mercadolibre_Login_FailedByUsername(IWebDriver driver) : base(driver) { }

  public override async Task ExecuteAsync()
  {
    if(Precondition(out IWebElement? loginLink))
    {
      if(loginLink == null)
      {
        await Terminal.WriteAsync("Link de inicio de sesión no encontrado", TerminalColor.Yellow);
        return;
      }
      loginLink.Click();

      await CheckForCaptchaAsync();
      
      IWebElement userInput = Driver.FindElement(By.Id("user_id"));
      userInput.SendKeys("testerina@gmail.com");
      userInput.SendKeys(Keys.Enter);

      await Task.Delay(3000);

      await CheckForCaptchaAsync(() =>
      {
        return Postcondition();
      });

      if(Postcondition())
      {
        await Assert(true);
        return;
      }

      await Assert(false);
      return;
    }

    await Terminal.WriteAsync("Esta prueba requiere que el usuario no esté autenticado", TerminalColor.Red);
    
  }

  private bool Precondition(out IWebElement? loginLink)
  {
    bool result = CheckIfSignedOut(out loginLink);
    return result;
  }

  private bool Postcondition()
  {
    IWebElement[] noUserLabels = Driver
      .FindElements(By.Id("user_id-message"))
      .ToArray();

    foreach(IWebElement m in noUserLabels)
      if(m.Text == "Revisa tu e-mail o usuario.")
        return true;
    return false;
  }
}