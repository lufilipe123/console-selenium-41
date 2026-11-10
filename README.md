# console-selenium-41

Projeto de automacao de testes com C#, MSTest e Selenium WebDriver.

## Testes do formulario web

O projeto `ConsoleSelenium.Tests` automatiza o formulario de exemplo do Selenium
([web-form.html](https://www.selenium.dev/selenium/web/web-form.html)) no Firefox.
Os cenarios cobrem:

- preenchimento de campos de texto, senha e area de texto, validacao do campo
  somente leitura e envio do formulario;
- selecao de uma opcao em dropdown e preenchimento de um campo datalist;
- manipulacao de checkbox, radio button e seletor de cor.

Cada cenario confirma o envio verificando a mensagem `Received!`.

## Executar

Requisitos: .NET 10 SDK e Firefox. O Selenium Manager configura o GeckoDriver
automaticamente.

Na raiz do repositorio, execute:

```powershell
dotnet test .\ConsoleSelenium.Tests\ConsoleSelenium.Tests.csproj
```
