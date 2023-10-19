using Aide;
using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_AddProductXiaomiRedmiToCart_RedirectToProductAdded : BaseTask
{
    protected override int TestId => 6;
    protected override string TestName => "Agregar Producto Xiaomi Redmi a Carro";

    private const string m_productName = "Xiaomi Redmi 12C";
    private int? m_productsInCart = null;

    public Mercadolibre_AddProductXiaomiRedmiToCart_RedirectToProductAdded(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if (!await Precondition())
            return;

        IWebElement addToCartButton = Driver.FindElement(By.Id(":R9j9ahit7k:"));

        try
        {
            addToCartButton.Click();
        }
        catch (ElementClickInterceptedException ex)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript(@"document.getElementsByClassName('andes-modal__overlay--modal')[0].style.display = 'none';");

            addToCartButton.Click();
        }

        await PostCondition();
    }

    protected override async Task<bool> Precondition()
    {
        bool authenticated = !CheckAuthentication(out IWebElement? link);
        bool inProductView = ProductDetailsView();
        bool noProductsInCart = ProductsInCartBefore();

        if (AideBool.AndCheck(authenticated, inProductView, noProductsInCart))
            return true;

        if (!authenticated)
            await Terminal.WriteAsync("Sesión no iniciada", TerminalColor.Yellow);

        if (!inProductView)
            await Terminal.WriteAsync("Producto no encontrado", TerminalColor.Yellow);

        if (!noProductsInCart)
            await Terminal.WriteAsync("Existen productos en carro", TerminalColor.Yellow);

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

    private bool ProductsInCartBefore()
    {
        int? cnt = ProductsInCart();

        if (cnt == 0)
        {
            m_productsInCart = 0;
            return true;
        }

        return false;
    }

    protected override async Task PostCondition()
    {
        bool success = await CheckAddedToCartView();
        bool productsInCart = ProductsInCartAfter();

        if (AideBool.AndCheck(success, productsInCart))
        {
            await Assert(true);
            return;
        }

        if(success)
        {
            await Terminal.WriteAsync("Cantidad de productos en ícono de carro no se actualizó", TerminalColor.Yellow);
            await Assert(true);
            return;
        }

        await base.PostCondition();
    }

    private async Task<bool> CheckAddedToCartView()
    {
        await Task.Delay(3000);
        IWebElement[] successHeader = Driver.FindElements(By.ClassName("bf-ui-rich-text--success"))
            .ToArray();

        foreach (IWebElement h in successHeader)
            if (h.Text.Contains("Agregaste a tu carrito"))
                return true;

        return false;
    }

    private bool ProductsInCartAfter()
    {
        int? cnt = ProductsInCart();

        if (cnt == m_productsInCart + 1)
            return true;

        return false;
    }

    private int? ProductsInCart()
    {
        IWebElement cartQuantityIcon = Driver.FindElement(By.ClassName("nav-icon-cart-quantity"));

        if (string.IsNullOrWhiteSpace(cartQuantityIcon.Text))
            return 0;

        if(int.TryParse(cartQuantityIcon.Text, out int q))
            return q;

        return null;
    }
}
