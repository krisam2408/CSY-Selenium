using MercadolibreSelenium.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TerminalWrapper;
using TerminalWrapper.Console;

IWebDriver driver = new ChromeDriver();

MainTask[] tasks = new MainTask[]
{
    new Mercadolibre_Navigate(driver),                                              // 01 No es prueba
    new Mercadolibre_SearchProductArduino_ListProducts(driver),                     // 02 NL Correcto
    new Mercadolibre_SearchImpossibleProduct_ListNothing(driver),                   // 03 NL Defecto
    new Mercadolibre_BuyProductXiaomi_RedirectToLogin(driver),                      // 04 NL Defecto
    new Mercadolibre_AddProductXiaomiToCart_RedirectToLogin(driver),                // 05 NL Defecto
    new Mercadolibre_BuyProductXiaomi_RedirectToBuy(driver),                        // 06 RL Correcto
    new Mercadolibre_AddProductXiaomiRedmiToCart_RedirectToProductAdded(driver),    // 07 RL Correcto
    new Mercadolibre_AddMoreProductsToCart_QuantityIncreased(driver),               // 08 RL Correcto
    new Mercadolibre_TakeProductFromCart_QuantityReduced(driver),                   // 09 RL Correcto
    new Mercadolibre_DeleteProductFromCart_PriceReduced(driver),                    // 10 RL Correcto
};

ConsoleTerminal terminal = ConsoleTerminal.CreateTerminal(tasks);
terminal.OnExit += () =>
{
    driver.Quit();
};

await terminal.RunAsync();