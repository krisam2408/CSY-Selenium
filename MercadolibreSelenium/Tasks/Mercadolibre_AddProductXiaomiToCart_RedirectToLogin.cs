using Aide;
using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_AddProductXiaomiToCart_RedirectToLogin : BaseTask
{
    protected override int TestId => 4;
    protected override string TestName => "Añadir Producto Xiaomi Redmi a Carro Sin Sesión Iniciada";

    private const string m_productName = "Xiaomi Redmi 12C";

    public Mercadolibre_AddProductXiaomiToCart_RedirectToLogin(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if (!await Precondition())
            return;

        IWebElement buyButton = Driver.FindElement(By.Id(":R9j9ahit7k:"));

        try
        {
            buyButton.Click();
        }
        catch (ElementClickInterceptedException ex)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript(@"document.getElementsByClassName('andes-modal__overlay--modal')[0].style.display = 'none';");

            buyButton.Click();
        }

        await PostCondition();
    }

    protected override async Task<bool> Precondition()
    {
        bool authenticated = CheckAuthentication(out IWebElement? link);
        bool inProductView = ProductDetailsView();

        if (AideBool.AndCheck(authenticated, inProductView))
            return true;

        if (!authenticated)
            await Terminal.WriteAsync("Sesión iniciada", TerminalColor.Yellow);

        if (!inProductView)
            await Terminal.WriteAsync("Producto no encontrado", TerminalColor.Yellow);

        return await base.Precondition();
    }

    private bool ProductDetailsView()
    {
        try
        {
            IWebElement searcher = Driver.FindElement(By.Id("cb1-edit"));
            searcher.SendKeys(m_productName);
            searcher.SendKeys(Keys.Enter);

            IWebElement[] items = Driver.FindElements(By.ClassName("ui-search-item__title"))
                .ToArray();

            items[0].Click();
        }
        catch
        {
            return false;
        }

        return true;
    }

    protected override async Task PostCondition()
    {
        bool loginRequired = await CheckLoginRedirection();

        if (loginRequired)
        {
            await Assert(true);
            return;
        }

        await base.PostCondition();
    }

    private async Task<bool> CheckLoginRedirection()
    {
        await Task.Delay(3000);

        IWebElement[] warningHeader = Driver.FindElements(By.ClassName("center-card__title"))
            .ToArray();

        foreach (IWebElement h in warningHeader)
            if (h.Text.ToLower().Contains("ingresa a tu cuenta"))
                return true;

        return false;
    }
}
