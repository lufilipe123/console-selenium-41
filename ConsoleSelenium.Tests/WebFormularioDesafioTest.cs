using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests
{
    /// <summary>
    /// Testes automatizados do formulário web https://www.selenium.dev/selenium/web/web-form.html
    /// cobrindo preenchimento misto de inputs, seletores/listas inteligentes e
    /// manipulação de estados (checkbox, radio e color picker).
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class WebFormularioDesafioTest
    {
        private const string FormUrl = "https://www.selenium.dev/selenium/web/web-form.html";
        private const string NomeCompleto = "Luiz Filipe Pimenta";

        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;

        [TestInitialize]
        public void Inicializar()
        {
            // Selenium Manager baixa e configura o GeckoDriver automaticamente.
            _driver = new FirefoxDriver();
            _driver.Manage().Window.Maximize();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [TestCleanup]
        public void Finalizar()
        {
            try
            {
                _driver?.Quit();
            }
            catch (Exception)
            {
                // Ignora falhas ao encerrar o navegador para não mascarar o resultado do teste.
            }
        }

        /// <summary>
        /// Exercício 1 — Cenário de Cadastro Completo (Preenchimento Misto).
        /// </summary>
        [TestMethod]
        public void PreencherFormulario_CadastroCompleto_DeveExibirReceived()
        {
            // Arrange / Act
            AbrirFormulario();

            EnviarTexto(By.Name("my-text"), NomeCompleto);
            EnviarTexto(By.Name("my-password"), "S3nh4-S3gur4!2026");
            EnviarTexto(By.Name("my-textarea"),
                "Experiencia com testes automatizados: criacao de suites com C#, MSTest e Selenium WebDriver, " +
                "cobrindo cenarios de formulario, seletores e assercoes de estado.");

            // Assert — o input somente leitura deve conter exatamente o valor padrao.
            var campoSomenteLeitura = _driver.FindElement(By.Name("my-readonly"));
            Assert.AreEqual("Readonly input", campoSomenteLeitura.GetAttribute("value"));

            SubmeterFormulario();
            Assert.AreEqual("Received!", ObterMensagemDeSucesso());
        }

        /// <summary>
        /// Exercício 2 — Interagindo com Seletores e Listas Inteligentes (Dropdowns e Datalist).
        /// </summary>
        [TestMethod]
        public void SelecionarDropdownEDatalist_DeveEnviarSemExcecoes()
        {
            // Arrange
            AbrirFormulario();

            // Act — seleciona a opcao "Two" no dropdown (select).
            var dropdown = new SelectElement(_driver.FindElement(By.Name("my-select")));
            dropdown.SelectByText("Two");

            // Assert — a opcao selecionada deve possuir o valor "2".
            Assert.AreEqual("2", dropdown.SelectedOption.GetAttribute("value"));

            // Act — envia o texto de uma cidade disponivel no datalist.
            EnviarTexto(By.Name("my-datalist"), "Seattle");

            SubmeterFormulario();
            Assert.AreEqual("Received!", ObterMensagemDeSucesso());
        }

        /// <summary>
        /// Exercício 3 — Manipulação de Estados, Checkboxes, Radios e Color Pickers.
        /// </summary>
        [TestMethod]
        public void ManipularCheckboxRadioEColorPicker_DeveEnviarComSucesso()
        {
            // Arrange
            AbrirFormulario();

            // Assert — o primeiro checkbox (id my-check-1) ja deve estar marcado por padrao.
            var checkboxPadrao = _driver.FindElement(By.Id("my-check-1"));
            Assert.IsTrue(checkboxPadrao.Selected, "O checkbox my-check-1 deveria estar marcado por padrao.");

            // Act — marca o segundo checkbox (id my-check-2).
            var checkboxDesmarcado = _driver.FindElement(By.Id("my-check-2"));
            checkboxDesmarcado.Click();
            Assert.IsTrue(checkboxDesmarcado.Selected, "O checkbox my-check-2 deveria estar marcado apos o clique.");

            // Act — alterna a selecao para o radio padrao (id my-radio-2).
            var radioPadrao = _driver.FindElement(By.Id("my-radio-2"));
            radioPadrao.Click();
            Assert.IsTrue(radioPadrao.Selected, "O radio my-radio-2 deveria estar selecionado apos o clique.");

            // Act — altera a cor do color picker.
            // Observacao: SendKeys nao funciona em <input type="color"> no Firefox,
            // portanto o valor e alterado via JavaScript, disparando o evento de mudanca.
            var colorPicker = _driver.FindElement(By.Name("my-colors"));
            const string corEsperada = "#00ff00";
            ((IJavaScriptExecutor)_driver).ExecuteScript(
                "arguments[0].value = arguments[1]; arguments[0].dispatchEvent(new Event('change', { bubbles: true }));",
                colorPicker,
                corEsperada);

            var corSelecionada = colorPicker.GetAttribute("value");
            Assert.IsNotNull(corSelecionada, "O color picker deveria possuir um valor de cor.");
            Assert.AreEqual(corEsperada, corSelecionada.ToLowerInvariant());

            SubmeterFormulario();
            Assert.AreEqual("Received!", ObterMensagemDeSucesso());
        }

        private void AbrirFormulario()
        {
            _driver.Navigate().GoToUrl(FormUrl);
            _wait.Until(d => d.FindElement(By.Name("my-text")).Displayed);
        }

        private void EnviarTexto(By localizador, string texto)
        {
            var elemento = _wait.Until(d => d.FindElement(localizador));
            elemento.Clear();
            elemento.SendKeys(texto);
        }

        private void SubmeterFormulario()
        {
            _driver.FindElement(By.TagName("button")).Click();
        }

        private string ObterMensagemDeSucesso()
        {
            // A pagina de sucesso (submitted.html) exibe a mensagem no elemento <p id="message">.
            return _wait.Until(d => d.FindElement(By.Id("message"))).Text;
        }
    }
}
