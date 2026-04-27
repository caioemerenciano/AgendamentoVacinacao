# Agendamento de Vacinação - API 💉

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=dotnet)
![JWT](https://img.shields.io/badge/JWT-Authentication-black?logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Swagger-Documentation-85EA2D?logo=swagger)

## 🎯 Sobre o Projeto

O **Agendamento de Vacinação** é uma API robusta desenvolvida para gerenciar o fluxo de vacinação em clínicas e centros de saúde. O sistema permite que pacientes agendem suas doses de forma organizada e que profissionais de saúde (enfermeiros) gerenciem a execução desses agendamentos.

### Fluxos Principais:
- **Autenticação e Registro**: Cadastro de usuários com perfis distintos.
- **Agendamento**: Solicitação de vacinação com validação de disponibilidade e intervalo de segurança.
- **Gestão de Vagas**: Controle automático de limite diário (20 vagas) e simultâneo (2 agendamentos por hora).
- **Painel de Controle**: Listagem dinâmica baseada em permissões.

---

## 🚀 Tecnologias e Padrões

### Stack Técnica:
- **Framework**: .NET 8.0
- **Persistência**: Entity Framework Core com SQL Server
- **Validação**: FluentValidation para garantir integridade dos DTOs
- **Documentação**: Swagger (OpenAPI)

### Padrões de Projeto (Design Patterns):
- **Generic/Base Repository**: Implementação centralizada de CRUD para reduzir duplicação de código e garantir consistência no acesso aos dados.
- **Dependency Injection**: Utilização do container nativo do .NET para desacoplamento de camadas.
- **Domain-Driven Design (Inspirado)**: Separação clara entre Entidades, Regras de Negócio e Interfaces de Repositório.
- **JWT (JSON Web Tokens)**: Segurança stateless para comunicação entre Frontend e Backend.

---

## 🔐 Regras de Acesso

O sistema utiliza **Role-Based Access Control (RBAC)**:

| Perfil | Permissões |
| :--- | :--- |
| **Paciente** | Realizar agendamentos, visualizar seus próprios agendamentos e cancelar solicitações. |
| **Enfermeiro** | Visualizar todos os agendamentos da clínica e marcar agendamentos como "Realizados". |

---

## ⚙️ Pré-requisitos

Antes de começar, você precisará ter instalado em sua máquina:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (ou LocalDB)
- [Git](https://git-scm.com/)

---

## 🛠️ Instalação e Execução

Siga os passos abaixo para rodar o projeto localmente:

1. **Clonar o Repositório**:
   ```bash
   git clone https://github.com/usuario/AgendamentoVacinacao.git
   cd AgendamentoVacinacao
   ```

2. **Restaurar Dependências**:
   ```bash
   dotnet restore
   ```

3. **Configurar o Banco de Dados**:
   Certifique-se de que a Connection String no `appsettings.json` aponta para sua instância do SQL Server e execute as Migrations:
   ```bash
   dotnet ef database update --project AgendamentoVacinacao.Repository --startup-project AgendamentoVacinacao.Api
   ```

4. **Executar a Aplicação**:
   ```bash
   dotnet run --project AgendamentoVacinacao.Api
   ```

---

## 🔑 Variáveis de Ambiente

As configurações de conexão e segurança ficam no arquivo `AgendamentoVacinacao.Api/appsettings.json`. Utilize o template abaixo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=AgendamentoVacinacaoDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "EstaEumaChaveSuperSecretaEmuitoLongaParaGarantirASegurancaDoSistema2026!",
    "Issuer": "AgendamentoVacinacaoApi",
    "Audience": "AgendamentoVacinacaoApp",
    "ExpireHours": 8
  }
}
```

> [!IMPORTANT]
> Nunca versione chaves reais ou senhas em repositórios públicos.

---

## 📚 Documentação da API

Após iniciar a aplicação, você pode acessar a documentação interativa do Swagger para testar os endpoints:

🔗 **URL Padrão**: `http://localhost:5056/swagger/index.html` (Verifique a porta no console ao iniciar)

---
Desenvolvido com foco em escalabilidade e manutenibilidade. 🚀