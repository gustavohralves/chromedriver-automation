using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;



public static class WebDriverExtensions
{
    public static IWebElement Xpath(this IWebDriver driver, string value)
    {
        return driver.FindElement(By.XPath(value));
    }

    public static IWebElement Name(this IWebDriver driver, string value)
    {
        return driver.FindElement(By.Name(value));
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        string chromeVersion = GetChromeVersion();
        string chromeDriverVersion = GetChromeDriverVersion(chromeVersion);
        string chromeDriverZipPath = DownloadChromeDriver(chromeDriverVersion);
        ExtractChromeDriver(chromeDriverZipPath);
        DeleteFile(chromeDriverZipPath);

        // Criar uma instância do WebDriver
        IWebDriver driver = new ChromeDriver();

        // Acessar o site desejado
        driver.Navigate().GoToUrl("https://www.google.com");

        driver.Name("q").SendKeys("Hello World");

        // Utilizar o método de extensão para encontrar um elemento pelo XPath
        driver.Xpath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[4]/center/input[1]").Click();

        // Realizar operações com o elemento encontrado
        // ...

        // Fechar o navegador
        driver.Quit();
    }

    static string GetChromeVersion()
    {
        var chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; //Mudar caminho para o caminho que estiver o chrome.exe na máquina
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(chromePath);
        return fileVersionInfo.FileVersion;
    }

    static string GetChromeDriverVersion(string chromeVersion)
    {
        string url = $"https://chromedriver.chromium.org/downloads";
        WebClient webClient = new WebClient();
        string html = webClient.DownloadString(url);
        chromeVersion = Regex.Match(chromeVersion, @"(\d+\.\d+\.\d+)", RegexOptions.Singleline).Groups[1].Value;
        string value = Regex.Match(html, $@"ChromeDriver\s+{chromeVersion}(\.\d+)", RegexOptions.Singleline).Groups[1].Value;
        chromeVersion = chromeVersion + value;
        value = $"https://chromedriver.storage.googleapis.com/{chromeVersion}/chromedriver_win32.zip";
        return value;
    }

    static string DownloadChromeDriver(string url)
    {
        string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "chromedriver.zip");
        using (WebClient webClient = new WebClient())
        {
            webClient.DownloadFile(url, zipPath);
        }
        return zipPath;
    }

    // Função para excluir um arquivo
    static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    // Função para mover um arquivo
    static void MoveFile(string sourceFilePath, string destinationFilePath)
    {
        if (File.Exists(sourceFilePath))
        {
            File.Move(sourceFilePath, destinationFilePath);
        }
    }

    static void ExtractChromeDriver(string zipPath)
    {
        DeleteFile($"{zipPath.Replace("chromedriver.zip", "")}chromedriver\\chromedriver.exe");
        DeleteFile($"{zipPath.Replace("chromedriver.zip", "")}chromedriver\\LICENSE.chromedriver");
        string extractPath = Path.Combine(Directory.GetCurrentDirectory(), "chromedriver");
        ZipFile.ExtractToDirectory(zipPath, extractPath);

        DeleteFile($"{zipPath.Replace("chromedriver.zip", "")}chromedriver.exe");
        MoveFile($"{zipPath.Replace("chromedriver.zip", "")}chromedriver\\chromedriver.exe",
            $"{zipPath.Replace("chromedriver.zip", "")}chromedriver.exe");
        DeleteFile($"{zipPath.Replace("chromedriver.zip", "")}chromedriver\\chromedriver.exe");
        DeleteFile($"{zipPath.Replace("chromedriver.zip", "")}chromedriver\\LICENSE.chromedriver");
    }
}
