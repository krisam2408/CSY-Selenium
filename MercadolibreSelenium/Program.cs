using MercadolibreSelenium.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TerminalWrapper;
using TerminalWrapper.Console;

IWebDriver driver = new ChromeDriver();

MainTask[] tasks = new MainTask[]
{
    new Mercadolibre_Navigate(driver),
    new Mercadolibre_SearchProductArduino_ListProducts(driver)
  
};

ConsoleTerminal terminal = ConsoleTerminal.CreateTerminal(tasks);
terminal.OnExit += () =>
{
    driver.Quit();
};

await terminal.RunAsync();