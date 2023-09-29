using System.Diagnostics.CodeAnalysis;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V115.Page;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

public abstract class BaseTask : MainTask
{
    protected IWebDriver Driver { get; private set; }

    protected abstract int TestId { get; }
    protected abstract string TestName { get; }
    public override string TaskName => $"CP{TestId.ToString().PadLeft(2, '0')} - {TestName}";

    protected BaseTask(IWebDriver driver) : base()
    {
        Driver = driver;
    }

    protected async Task Assert(bool result)
    {
        if(result)
        {
            await Terminal.WriteAsync($"\"{TaskName}\" logrado exitosamente", TerminalColor.Green);
            return;
        }
        await Terminal.WriteAsync($"\"{TaskName}\" presenta fallos", TerminalColor.Red);
    }

    protected async Task CheckForCaptchaAsync(Func<bool>? condition = null)
    {
        IWebElement[] iframes;
        IWebElement? captcha = null;

        bool priorityCondition()
        {
            if(condition != null)
            {
                bool result = condition.Invoke();
                return !result;
            }
            return true;
        }
      
        do
        {
            iframes = Driver.FindElements(By.TagName("iframe"))
                .ToArray();
                
            foreach(IWebElement f in iframes)
                if(f.GetAttribute("title") == "reCAPTCHA")
                {
                    captcha = f;
                    break;
                }
         
            if(captcha != null)
            {
                await Terminal.WriteAsync("Captcha encontrado. Se requiere asistencia humana", TerminalColor.Yellow);
                await Terminal.WriteAsync("Pulse cualquier tecla para continuar");
                await Terminal.PauseAsync();
            }
        }while(captcha != null && priorityCondition());
    }

    protected bool CheckIfSignedOut(out IWebElement? loginLink)
    {
        Driver.Navigate()
            .GoToUrl("https://www.mercadolibre.cl");

        IWebElement[] links = Driver
            .FindElements(By.TagName("a"))
            .ToArray();

        foreach(IWebElement a in links)
            if(a.Text == "Ingresa")
            {
                loginLink = a;
                return true;
            }
      
        loginLink = null;
        return false;
    }
}