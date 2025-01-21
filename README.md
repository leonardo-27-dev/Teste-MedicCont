
# Teste MedicCont

Este projeto automatiza o processo de login no **Sistema de Serviços Online (SSO)** com validação de CPF (Cadastro de Pessoas Físicas) e senha. A aplicação solicita ao usuário que insira seu CPF e senha, formata o CPF, resolve o CAPTCHA usando um serviço de terceiros e executa as etapas de login com as requisições apropriadas para a API de backend.

## Funcionalidades

- **Entrada de CPF**: O usuário é solicitado a inserir seu CPF. O CPF é formatado no formato padrão brasileiro (XXX.XXX.XXX-XX).
- **Entrada de Senha**: O usuário é solicitado a inserir sua senha.
- **Manipulação de CAPTCHA**: A aplicação solicita um desafio CAPTCHA de um serviço externo (hCaptcha), trata a resposta e envia como parte da requisição de login.
- **Requisições de Login**: O CPF e o CAPTCHA são enviados em requisições separadas, seguidos pela senha na última requisição de login.
- **Mensagem de Sucesso**: Após o login bem-sucedido, a aplicação confirma a autenticação com sucesso e exibe um código de login do URL de redirecionamento.

## Requisitos

- .NET Core SDK ou versão posterior (para executar o programa C#).
- Uma conexão ativa à internet para que as requisições de login sejam processadas.
- Capacidade de resolver CAPTCHAs (pode ser necessária intervenção manual dependendo do provedor de CAPTCHA).

## Como Usar

1. **Clonar ou Baixar o Projeto**:
   Clone o repositório ou baixe o arquivo `.zip` e extraia-o para o seu computador.

2. **Executar a Aplicação**:
   Abra o terminal e navegue até o diretório do projeto. Execute o seguinte comando para rodar a aplicação:
   ```bash
   dotnet run
   ```

3. **Inserir o CPF**:
   Quando solicitado, insira seu CPF. O CPF será automaticamente formatado (XXX.XXX.XXX-XX).

4. **Inserir a Senha**:
   Em seguida, a aplicação pedirá a senha para autenticação.

5. **Resolver o CAPTCHA**:
   A resposta do CAPTCHA é obtida de um serviço externo (hCaptcha). A aplicação trata automaticamente este processo, mas pode ser necessária a interação do usuário.

6. **Confirmação de Login**:
   Após o envio bem-sucedido do CPF, CAPTCHA e senha, a aplicação exibirá uma mensagem de sucesso.

## Explicação do Código

### 1. `ConfigureHttpClient()`
Configura os cabeçalhos necessários para o cliente HTTP, incluindo o user-agent e referrer, para simular uma requisição de navegador real.

### 2. `GetInput()`
Solicita a entrada do usuário (seja CPF ou senha).

### 3. `RequestCaptchaAsync()`
Faz uma requisição HTTP para buscar um desafio CAPTCHA. Ele formata e envia as informações necessárias, e, se bem-sucedido, retorna o resultado do CAPTCHA.

### 4. `SendCpfAsync()`
Envia o CPF do usuário e a resposta do CAPTCHA para o backend para iniciar o processo de login.

### 5. `SendSenhaAsync()`
Envia a senha na última requisição de login, verificando se o redirecionamento contém o código de login bem-sucedido.

### 6. `FormatCPF()`
Formata o CPF no formato brasileiro padrão.

## Exemplo de Saída

```
Digite o CPF:
12345678909
Digite a senha:
********
Resposta do hCaptcha recebida com sucesso.
CPF enviado com sucesso!
Senha enviada com sucesso!
Login realizado com sucesso! Code: code=abcdef123456
```

## Notas

- **Captcha**: A aplicação solicita um CAPTCHA do serviço hCaptcha e espera uma resposta automatizada. No entanto, se for necessária a intervenção manual, será preciso resolver o CAPTCHA antes de prosseguir.
- **Segurança**: Tenha cuidado ao manipular informações sensíveis como senhas e siga práticas seguras de transmissão e armazenamento de dados.
