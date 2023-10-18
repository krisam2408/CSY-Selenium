using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_Register_SMSSent : BaseTask
{
    protected override int TestId => 2;
    protected override string TestName => "Registro de usuario hasta verificaciòn de teléfono";

    public Mercadolibre_Register_SMSSent(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if(await Precondition())
        {
            IWebElement registerLink = Driver
                .FindElements(By.TagName("a"))
                .First(a => a.Text == "Crea tu cuenta");
            
            registerLink.Click();

            IWebElement accountLink = Driver
                .FindElements(By.TagName("button"))
                .First(b => b.Text == "Crear cuenta personal");
            
            
            await Terminal.WriteAsync("Prueba sin terminar", TerminalColor.Red);
            return;
        }

        await Terminal.WriteAsync("Esta prueba requiere que el usuario no esté autenticado", TerminalColor.Red);
    }

    protected override async Task<bool> Precondition()
    {
        bool result = CheckAuthentication(out IWebElement? link);
        await Task.Delay(8);
        return result;
    }

    protected override Task PostCondition()
    {
        throw new NotImplementedException();
    }
}