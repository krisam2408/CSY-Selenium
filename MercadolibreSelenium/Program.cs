using MercadolibreSelenium.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TerminalWrapper;
using TerminalWrapper.Console;

IWebDriver driver = new ChromeDriver();

MainTask[] tasks = new MainTask[]
{
    new Mercadolibre_Navigate(driver),
    new Mercadolibre_SearchProductArduino_ListProducts(driver),
    new Mercadolibre_BuyProductXiaomi_RedirectToBuy(driver),
    new Mercadolibre_BuyProductXiaomi_RedirectToLogin(driver),
    new Mercadolibre_AddProductXiaomiRedmiToCart_RedirectToProductAdded(driver),
    new Mercadolibre_AddMoreProductsToCart_QuantityIncreased(driver),
    new Mercadolibre_TakeProductFromCart_QuantityReduced(driver),
};

ConsoleTerminal terminal = ConsoleTerminal.CreateTerminal(tasks);
terminal.OnExit += () =>
{
    driver.Quit();
};

await terminal.RunAsync();