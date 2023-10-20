﻿using Aide;
using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public sealed class Mercadolibre_TakeProductFromCart_QuantityReduced : BaseTask
{
    protected override int TestId => 8;
    protected override string TestName => "Quitar Producto de Carro";

    private readonly string[] m_products = new string[] { "Xiaomi", "Samsung", "Huawei" };
    private int m_quantity;
    private int m_price;

    public Mercadolibre_TakeProductFromCart_QuantityReduced(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if (!await Precondition())
            return;

        m_quantity = GetFirstItemQuantity();
        m_price = GetTotalSum();

        IWebElement minusButton = Driver.FindElement(By.Id("full_item_row_1_quantity_selector-button-decrease"));
        minusButton.Click();

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

    private int GetFirstItemQuantity()
    {
        IWebElement quantityInput = Driver.FindElement(By.Id("full_item_row_1_quantity_selector-textfield"));
        string quantity = quantityInput.GetAttribute("value");
        return int.Parse(quantity);
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
        bool moreThanOneProduct = await CheckMoreThanOneProduct();

        if (AideBool.AndCheck(authenticated, productsInCart, moreThanOneProduct))
            return true;

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

    private async Task<bool> CheckMoreThanOneProduct()
    {
        IWebElement cartLink = Driver.FindElement(By.Id("nav-cart"));
        cartLink.Click();

        await Task.Delay(1000);

        int firstItemQuantity = GetFirstItemQuantity();
        if(firstItemQuantity == 1)
        {
            IWebElement plusButton = Driver.FindElement(By.Id("full_item_row_1_quantity_selector-button-increase"));
            plusButton.Click();

            await Task.Delay(1000);
        }

        return true;
    }

    protected override async Task PostCondition()
    {
        bool quantityDecreased = CheckQuantityDecrease();
        bool sumDecreased = CheckTotalSumDecrease();

        if (AideBool.AndCheck(quantityDecreased, sumDecreased))
        {
            await Assert(true);
            return;
        }

        await base.PostCondition();
    }

    private bool CheckQuantityDecrease()
    {
        int quantity = GetFirstItemQuantity();
        if (quantity < m_quantity)
            return true;

        return false;
    }

    private bool CheckTotalSumDecrease()
    {
        int price = GetTotalSum();
        if (price < m_price)
            return true;

        return false;
    }
}
