using Aide;
using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_BuyProductXiaomi_RedirectToBuy : BaseTask
{
    protected override int TestId => 5;
    protected override string TestName => "Comprar Producto Xiaomi Redmi";

    private const string m_productName = "Xiaomi Redmi 12C";

    public Mercadolibre_BuyProductXiaomi_RedirectToBuy(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if (!await Precondition())
            return;

        IWebElement buyButton = Driver.FindElement(By.Id(":R9b9ahit7k:"));

        try
        {
            buyButton.Click();
        }
        catch (ElementClickInterceptedException ex)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("document.getElementsByClassName('andes-modal__overlay--modal')[0].style.display = 'none';");

            buyButton.Click();
        }

        await PostCondition();
    }

    protected override async Task<bool> Precondition()
    {
        bool authenticated = !CheckAuthentication(out IWebElement? link);
        bool inProductView = ProductDetailsView();

        if (AideBool.AndCheck(authenticated, inProductView))
            return true;

        if (!authenticated)
            await Terminal.WriteAsync("Sesión no iniciada", TerminalColor.Yellow);

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
        await Task.Delay(3000);

        bool warranty = CheckWarrantyView();
        bool buy = CheckBuyView();

        if(AideBool.OrCheck(warranty, buy))
        {
            await Assert(true);
            return;
        }

        await base.PostCondition();
    }

    private bool CheckWarrantyView()
    {
        IWebElement[] warrantyHeader = Driver.FindElements(By.ClassName("header"))
            .ToArray();

        foreach(IWebElement h in warrantyHeader)
            if(h.Text.Contains("Añadir una garantía"))
                return true;

        return false;
    }

    private bool CheckBuyView()
    {
        IWebElement[] destinyHeader = Driver.FindElements(By.ClassName("bf-ui-rich-text"))
            .ToArray();

        foreach (IWebElement h in destinyHeader)
            if (h.Text.Contains("Elige la forma de entrega"))
                return true;

        return false;
    }
}
