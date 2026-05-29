# DDG Journey — Plataforma de Jornada de Trabalho

Sistema interno de registro e gestão da jornada de trabalho da **Dungeons & Dragons Group**, desenvolvido como desafio técnico.

---

## Tecnologias

| Camada | Stack |
|--------|-------|
| Backend | .NET 9 · ASP.NET Core · Entity Framework Core · SQL Server · JWT |
| Frontend | Angular 21 · Tailwind CSS 4 · TypeScript |
| Infraestrutura | Docker · Docker Compose |

---

## Rodando com Docker (recomendado)

### Pré-requisitos
- [Docker](https://www.docker.com/products/docker-desktop) instalado e rodando

### Passo a passo

```bash
# 1. Clone o repositório
git clone <url-do-repositorio>
cd dreamer-desafio

# 2. Crie o arquivo de variáveis de ambiente
cp .env.example .env

# 3. Suba todos os serviços
docker compose up --build
```

Após o build, acesse:

| Serviço | URL |
|---------|-----|
| Frontend | http://localhost:4200 |
| API | http://localhost:5000 |
| Docs interativos (Scalar) | http://localhost:5000/scalar/v1 |

> O banco de dados é criado automaticamente na primeira execução. Um usuário admin é criado via seed.

---

## Rodando localmente (sem Docker)

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org)
- [SQL Server](https://www.microsoft.com/sql-server) (ou SQL Server Developer/Express)

### Backend

```bash
cd dedg-back

# Copie o arquivo de configuração local (já tem valores padrão prontos para uso)
cp appsettings.Development.example.json appsettings.Development.json

dotnet run
# API disponível em http://localhost:5000
```

> O arquivo `appsettings.Development.example.json` já contém connection string apontando para `localhost:1433` com senha `Senha@1234` e um JWT secret de desenvolvimento. Edite apenas se o seu SQL Server usar configurações diferentes.

### Frontend

```bash
cd dedg-front

npm install
ng serve
# Frontend disponível em http://localhost:4200
```

---

## Variáveis de Ambiente

O arquivo `.env` (na raiz) é usado pelo Docker Compose:

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `MSSQL_SA_PASSWORD` | Senha do SA do SQL Server | `Senha@1234` |
| `JWT__SECRET` | Chave secreta para assinar tokens JWT | `minha-chave-forte` |

Para execução local, configure o `appsettings.Development.json` do backend (use `appsettings.Development.example.json` como base).

---

## Credenciais Padrão

| Campo | Valor |
|-------|-------|
| E-mail | `admin@ddgroup.com` |
| Senha | `Admin@123` |
| Perfil | RH / Admin |

---

## Perfis de Acesso

| Perfil | Permissões |
|--------|-----------|
| **Colaborador** | Registra e consulta a própria jornada |
| **Gestor** | Visualiza e edita registros do time; move períodos para revisão |
| **RH / Admin** | Acesso total: gerencia usuários, períodos e todos os registros |

---

## Documentação

| Documento | Descrição |
|-----------|-----------|
| [Regras de negócio](docs/business-rules.md) | Decisões técnicas e regras da plataforma |
| [Guia de uso](docs/user-guide.md) | Funcionamento detalhado de cada tela |
| [API](docs/api/api-routes.md) | Endpoints, payloads e respostas |
| [Banco de dados](docs/db/db_explanation.md) | Modelo de dados e relacionamentos |
| [Frontend](docs/frontend/screens-explanation.md) | Telas e fluxos |
| [Desafio original](docs/Desafio%20Técnico%20—%20Plataforma%20de%20Jornada.md) | Especificação do desafio |

A documentação interativa da API (Scalar) está disponível em `http://localhost:5000/scalar/v1` com o servidor rodando.

---

## Estrutura do Projeto

```
dreamer-desafio/
├── dedg-back/          # API .NET 9
├── dedg-front/         # SPA Angular 21
├── docs/               # Documentação técnica
├── docker-compose.yml
└── .env.example
```
