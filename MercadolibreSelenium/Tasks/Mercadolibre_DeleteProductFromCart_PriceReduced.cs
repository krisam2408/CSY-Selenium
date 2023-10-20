using Aide;
using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_DeleteProductFromCart_PriceReduced : BaseTask
{
    protected override int TestId => 9;
    protected override string TestName => "Eliminar Producto de Carro";

    private readonly string[] m_products = new string[] { "Xiaomi", "Samsung", "Huawei" };
    private int m_price;

    public Mercadolibre_DeleteProductFromCart_PriceReduced(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if (!await Precondition())
            return;

        m_price = GetTotalSum();

        IWebElement? deleteLink = Driver.FindElements(By.ClassName("link"))
            .ToArray()
            .FirstOrDefault(a => a.Text.Contains("Eliminar"));

        if(deleteLink == null)
        {
            await Terminal.WriteAsync("No se pudo continuar con la prueba", TerminalColor.Red);
            return;
        }

        try
        {
            deleteLink.Click();
        }
        catch(ElementClickInterceptedException ex)
        {

        }

        await Task.Delay(1000);

        await PostCondition();
    }

    private int? ProductsInCart()
    {
        IWebElement cartQuantityIcon = Driver.FindElement(By.ClassName("nav-icon-cart-quantity"));

        if (string.IsNullOrWhiteSpace(cartQuantityIcon.Text))
            return 0;

        if (int.TryParse(cartQuantityIcon.Text, out int q))
            return q;

        return null;
    }

    private int GetTotalSum()
    {
        IWebElement priceText = Driver.FindElements(By.ClassName("bf-ui-price-small"))
            .ToArray()
            .Last();

        string price = priceText.Text.Replace(".", "");
        return int.Parse(price);
    }

    protected override async Task<bool> Precondition()
    {
        bool authenticated = !CheckAuthentication(out IWebElement? link);
        bool productsInCart = await CheckProductsInCart();

        if (AideBool.AndCheck(authenticated, productsInCart))
        {
            IWebElement cartLink = Driver.FindElement(By.Id("nav-cart"));
            cartLink.Click();

            await Task.Delay(1000);
            return true;
        }

        if (!authenticated)
            await Terminal.WriteAsync("Sesión no iniciada", TerminalColor.Yellow);

        return await base.Precondition();
    }

    private async Task<bool> CheckProductsInCart()
    {
        int? productsInCart = ProductsInCart();

        if (productsInCart > 0)
            return true;

        if (productsInCart == 0)
        {
            await AddProducts();
            return true;
        }

        return true;
    }

    private async Task AddProducts()
    {
        foreach (string product in m_products)
            await AddProduct(product);
    }

    private async Task AddProduct(string product)
    {
        IWebElement searcher = Driver.FindElement(By.Id("cb1-edit"));
        searcher.SendKeys(product);
        searcher.SendKeys(Keys.Enter);

        IWebElement[] items = Driver.FindElements(By.ClassName("ui-search-item__title"))
            .ToArray();

        items[0].Click();

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

        await Task.Delay(3000);
    }

    protected override async Task PostCondition()
    {
        bool sumDecreased = CheckTotalSumDecrease();
        bool noProductsInCart = CheckNoProducts();

        if (AideBool.OrCheck(sumDecreased, noProductsInCart))
        {
            await Assert(true);
            return;
        }

        await base.PostCondition();
    }

    private bool CheckTotalSumDecrease()
    {
        int price;
        try
        {
            price = GetTotalSum();
        }
        catch
        {
            return false;
        }

        if (price < m_price)
            return true;

        return false;
    }

    private bool CheckNoProducts()
    {
        IWebElement? empty = Driver.FindElements(By.Id("empty_state"))
            .FirstOrDefault();

        if(empty != null)
            return true;

        return false;
    }
}
