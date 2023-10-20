using Aide;
using OpenQA.Selenium;
using TerminalWrapper;

namespace MercadolibreSelenium.Tasks;

[Obsolete("Abandonado por complicaciones con seguridad en productivo")]
public sealed class Mercadolibre_AddEmailToRegisterForm_EmailAdded : BaseTask
{
    protected override int TestId => 1;
    protected override string TestName => "Agregar Correo a Formulario de Registro";

    private const string TestMail = "testerina12@gmail.com";

    public Mercadolibre_AddEmailToRegisterForm_EmailAdded(IWebDriver driver) : base(driver) { }

    public override async Task ExecuteAsync()
    {
        if (!await Precondition())
            return;

        IWebElement mailInput = Driver.FindElement(By.ClassName("andes-form-control__field"));
        mailInput.SendKeys(TestMail);
        mailInput.SendKeys(Keys.Escape);

        await Task.Delay(100);

        IWebElement policiesCheck = Driver.FindElement(By.Id("policies"));
        policiesCheck.Click();

        IWebElement continueButton = Driver.FindElement(By.CssSelector("button.andes-button.enter-email-card__submit.andes-button--large.andes-button--loud.andes-button--full-width"));
        continueButton.Click();

        await PostCondition();
    }

    protected override async Task<bool> Precondition()
    {
        bool authenticated = CheckAuthentication(out IWebElement? link);
        bool view = GoToView();

        if (AideBool.AndCheck(authenticated, view))
            return true;

        await Terminal.WriteAsync("Sesión iniciada", TerminalColor.Yellow);

        return await base.Precondition();
    }

    private bool GoToView()
    {
        Driver.Navigate()
            .GoToUrl(HostUrl);

        IWebElement? registerLink = Driver.FindElements(By.TagName("a"))
            .FirstOrDefault(a => a.Text.Contains("Crea tu cuenta"));

        if(registerLink == null)
            return false;

        registerLink.Click();

        IWebElement firstButton = Driver.FindElement(By.Id("first-button"));
        firstButton.Click();

        IWebElement addEmailButton = Driver.FindElement(By.Id("hub-item-button"));

        if(addEmailButton.Text == "Agregar")
        {
            addEmailButton.Click();
            return true;
        }

        return false;
    }

    protected override async Task PostCondition()
    {
        await Task.Delay(5000);

        IWebElement chooseNameButton = Driver.FindElement(By.Id("hub-item-button"));

        string[] textOptions = new string[] { "Elegir", "Confirmar" };

        if(textOptions.Contains(chooseNameButton.Text))
        {
            await Assert(true);
        }

        await base.PostCondition();
    }


}
