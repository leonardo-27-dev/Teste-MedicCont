using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        ConfigureHttpClient();

        string cpf = GetInput("Digite o CPF: ");
        cpf = FormatCPF(cpf);

        string senha = GetInput("Digite a senha: ");

        string captchaResult = await RequestCaptchaAsync();
        if (string.IsNullOrEmpty(captchaResult)) return;

        bool isCpfSent = await SendCpfAsync(cpf, captchaResult);
        if (!isCpfSent) return;

        bool isSenhaSent = await SendSenhaAsync(senha);
        if (!isSenhaSent) return;

        Console.WriteLine("Login realizado com sucesso!");
    }

    private static void ConfigureHttpClient()
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        client.DefaultRequestHeaders.Referrer = new Uri("https://sso.acesso.gov.br/");
        client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
    }

    private static string GetInput(string prompt)
    {
        Console.WriteLine(prompt);
        return Console.ReadLine();
    }

    private static async Task<string> RequestCaptchaAsync()
    {
        var captchaUrl = "https://hcaptcha.com/getcaptcha/903db64c-2422-4230-a22e-5645634d893f";
        var motionData = new
        {
            st = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            v = 1,
            topLevel = new
            {
                inv = true,
                size = "invisible",
                theme = 1796889847,
                pel = "<div id=\"hcaptcha-govbr\" class=\"h-captcha\" data-sitekey=\"903db64c-2422-4230-a22e-5645634d893f\" data-size=\"invisible\" data-callback=\"submitGovBr\"></div>",
                st = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                sc = new
                {
                    availWidth = 1920,
                    availHeight = 1050,
                    width = 1920,
                    height = 1080,
                    colorDepth = 24,
                    pixelDepth = 24,
                    availLeft = 0,
                    availTop = 0,
                    onchange = (string)null,
                    isExtended = true
                },
                wi = new[] { 1920, 929 }
            },
            dr = "https://www.google.com/",
            exec = "api",
            wn = Array.Empty<object>(),
            xy = Array.Empty<object>()
        };

        var formData = new Dictionary<string, string>
        {
            { "v", "0b96e5e9c42dbf957bfbc38b40cf19fc6ddb81fb" },
            { "sitekey", "903db64c-2422-4230-a22e-5645634d893f" },
            { "host", "cav.receita.fazenda.gov.br" },
            { "hl", "pt-BR" },
            { "action", "network-error" },
            { "motionData", JsonSerializer.Serialize(motionData) }
        };

        var captchaContent = new FormUrlEncodedContent(formData);
        var captchaResponse = await client.PostAsync(captchaUrl, captchaContent).ConfigureAwait(false);

        if (!captchaResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erro ao solicitar o hCaptcha. Status: {captchaResponse.StatusCode}");
            return null;
        }

        var captchaResult = await captchaResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        Console.WriteLine("Resposta do hCaptcha recebida com sucesso.");
        return captchaResult;
    }

    private static async Task<bool> SendCpfAsync(string cpf, string captchaResult)
    {
        var loginUrl = "https://sso.acesso.gov.br/login?client_id=cav.receita.fazenda.gov.br&authorization_id=19488e25c97";
        var formData = new[]
        {
            new KeyValuePair<string, string>("accountId", cpf),
            new KeyValuePair<string, string>("_csrf", "d2905f5c-18c6-4fe5-92f5-06608d1dfdea"),
            new KeyValuePair<string, string>("operation", "enter-account-id"),
            new KeyValuePair<string, string>("h-captcha-response", captchaResult)
        };

        var cpfContent = new FormUrlEncodedContent(formData);
        var cpfResponse = await client.PostAsync(loginUrl, cpfContent).ConfigureAwait(false);

        if (!cpfResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erro ao enviar CPF. Status: {cpfResponse.StatusCode}");
            return false;
        }

        Console.WriteLine("CPF enviado com sucesso!");
        return true;
    }

    private static async Task<bool> SendSenhaAsync(string senha)
    {
        var loginUrl = "https://sso.acesso.gov.br/login?client_id=cav.receita.fazenda.gov.br&authorization_id=19488e25c97";
        var formData = new[]
        {
            new KeyValuePair<string, string>("password", senha),
            new KeyValuePair<string, string>("operation", "enter-password-id"),
            new KeyValuePair<string, string>("token", "eyJraWQiOiJsb2dpbkZsb3ciLCJhbGciOiJkaXIiLCJlbmMiOiJBMjU2R0NNIn0...")
        };

        var senhaContent = new FormUrlEncodedContent(formData);
        var senhaResponse = await client.PostAsync(loginUrl, senhaContent).ConfigureAwait(false);

        if (senhaResponse.StatusCode == System.Net.HttpStatusCode.Redirect)
        {
            var redirectUrl = senhaResponse.Headers.Location?.ToString();
            if (!string.IsNullOrEmpty(redirectUrl) && redirectUrl.Contains("code"))
            {
                var queryParams = new Uri(redirectUrl).Query;
                Console.WriteLine($"Login realizado com sucesso! Code: {queryParams}");
                return true;
            }

            Console.WriteLine("Erro: Code não encontrado no redirecionamento.");
        }
        else
        {
            Console.WriteLine($"Erro ao enviar a senha. Status: {senhaResponse.StatusCode}");
        }

        return false;
    }

    static string FormatCPF(string cpf)
    {
        cpf = Regex.Replace(cpf, @"\D", "");
        if (cpf.Length == 11)
        {
            return Regex.Replace(cpf, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4");
        }
        return cpf;
    }
}
