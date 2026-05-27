# API Routes

> A documentação interativa via Swagger estará disponível em `/swagger` quando implementada.

---

## Visão Geral

| Item | Valor |
|------|-------|
| Base URL | `http://localhost:5000/api` |
| Formato | `application/json` |
| Autenticação | Bearer Token (JWT) |

---

## Autenticação

A maioria das rotas exige autenticação. O token é obtido via `POST /api/auth/login` e deve ser enviado no header de todas as requisições protegidas:

```
Authorization: Bearer {token}
```

### `POST /api/auth/login`
Autentica o usuário e retorna um token JWT. Rota pública, não requer autenticação.

**Request body**
```json
{
  "email": "joao@empresa.com",
  "password": "senhaSegura123"
}
```

**Response** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2025-01-15T16:00:00Z",
  "userId": 1,
  "name": "João Silva",
  "role": "Collaborator"
}
```

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Login realizado, retorna o token e dados básicos do usuário |
| `401 Unauthorized` | E-mail ou senha inválidos |

---

## Recursos

- [Users](#users-apiusers)
- [Time Events](#time-events-apitimeevents)
- [Monthly Periods](#monthly-periods-apimonthlyperiods)

---

## Users `/api/users`

### `GET /api/users`
Retorna todos os usuários ativos.

**Response** `200 OK`
```json
[
  {
    "id": 1,
    "name": "João Silva",
    "email": "joao@empresa.com",
    "role": "Collaborator",
    "cpf": "123.456.789-00",
    "timeZone": "America/Sao_Paulo",
    "managerId": 2,
    "isActive": true,
    "createdAt": "2025-01-15T10:00:00Z"
  }
]
```

---

### `GET /api/users/{id}`
Retorna um usuário pelo ID. Retorna `404` se o usuário não existir ou estiver inativo.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do usuário |

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Usuário encontrado |
| `404 Not Found` | Usuário não existe ou está inativo |

---

### `POST /api/users`
Cria um novo usuário. A senha é recebida em texto puro e armazenada com BCrypt.

**Request body**
```json
{
  "name": "João Silva",
  "email": "joao@empresa.com",
  "password": "senhaSegura123",
  "role": "Collaborator",
  "cpf": "123.456.789-00",
  "timeZone": "America/Sao_Paulo",
  "managerId": 2
}
```

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `name` | `string` | ✅ | Nome completo |
| `email` | `string` | ✅ | E-mail |
| `password` | `string` | ✅ | Senha em texto puro, armazenada com BCrypt |
| `role` | `string` | ✅ | Perfil do usuário (`Collaborator`, `Manager`, `HrAdmin`) |
| `cpf` | `string` | ❌ | CPF |
| `timeZone` | `string` | ✅ | Fuso horário no formato IANA (ex: `America/Sao_Paulo`) |
| `managerId` | `int` | ❌ | ID do gestor responsável |

**Responses**
| Status | Descrição |
|--------|-----------|
| `201 Created` | Usuário criado, retorna o recurso criado |

---

### `PUT /api/users/{id}`
Atualiza os dados de um usuário. Todos os campos são opcionais, apenas os campos enviados são alterados.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do usuário |

**Request body**
```json
{
  "name": "João Silva Atualizado",
  "email": "novo@empresa.com",
  "timeZone": "Europe/Lisbon",
  "managerId": 3,
  "isActive": true
}
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `name` | `string` | Nome completo |
| `email` | `string` | E-mail |
| `timeZone` | `string` | Fuso horário no formato IANA |
| `managerId` | `int` | ID do gestor responsável |
| `isActive` | `bool` | Status do usuário |

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Usuário atualizado, retorna o recurso atualizado |
| `404 Not Found` | Usuário não encontrado |

---

### `DELETE /api/users/{id}`
Remove um usuário via soft delete, marcando `isActive` como `false`. O registro é mantido no banco.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do usuário |

**Responses**
| Status | Descrição |
|--------|-----------|
| `204 No Content` | Removido com sucesso |
| `404 Not Found` | Usuário não encontrado |

---

## Time Events `/api/timeevents`

### `GET /api/timeevents`
Retorna todos os registros de ponto. Aceita filtro opcional por usuário via query param.

**Query params**
| Nome | Tipo | Obrigatório | Descrição |
|------|------|-------------|-----------|
| `userId` | `int` | ❌ | Filtra pelos registros de um usuário específico |

Exemplos:
- `GET /api/timeevents`, retorna todos os registros
- `GET /api/timeevents?userId=1`, retorna apenas os registros do usuário 1

**Response** `200 OK`
```json
[
  {
    "id": 1,
    "userId": 1,
    "eventType": "Entry",
    "recordedAt": "2025-01-15T08:00:00Z",
    "timezoneAtRecording": "America/Sao_Paulo",
    "isTravel": false,
    "observation": "home office",
    "createdAt": "2025-01-15T08:00:00Z"
  }
]
```

---

### `GET /api/timeevents/{id}`
Retorna um registro de ponto pelo ID.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do registro |

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Registro encontrado |
| `404 Not Found` | Registro não existe |

---

### `POST /api/timeevents`
Cria um novo registro de ponto. `recordedAt` deve ser enviado em UTC.

**Request body**
```json
{
  "userId": 1,
  "eventType": "Entry",
  "recordedAt": "2025-01-15T08:00:00Z",
  "timezoneAtRecording": "America/Sao_Paulo",
  "isTravel": false,
  "observation": "home office"
}
```

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `userId` | `int` | ✅ | ID do colaborador |
| `eventType` | `string` | ✅ | Tipo do evento (`Entry`, `Exit`, `BreakStart`, `BreakEnd`) |
| `recordedAt` | `datetime` | ✅ | Momento do registro em UTC |
| `timezoneAtRecording` | `string` | ✅ | Fuso horário no momento do registro, formato IANA |
| `isTravel` | `bool` | ❌ | Indica se o colaborador estava em deslocamento. Padrão: `false` |
| `observation` | `string` | ❌ | Observação livre |

**Responses**
| Status | Descrição |
|--------|-----------|
| `201 Created` | Registro criado, retorna o recurso criado |
| `400 Bad Request` | Dados inválidos |

---

### `PUT /api/timeevents/{id}`
Atualiza um registro de ponto. Todos os campos são opcionais, apenas os campos enviados são alterados.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do registro |

**Request body**
```json
{
  "eventType": "Exit",
  "recordedAt": "2025-01-15T17:00:00Z",
  "timezoneAtRecording": "America/Sao_Paulo",
  "isTravel": false,
  "observation": "saída ajustada"
}
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `eventType` | `string` | Tipo do evento (`Entry`, `Exit`, `BreakStart`, `BreakEnd`) |
| `recordedAt` | `datetime` | Momento do registro em UTC |
| `timezoneAtRecording` | `string` | Fuso horário no momento do registro, formato IANA |
| `isTravel` | `bool` | Indica se o colaborador estava em deslocamento |
| `observation` | `string` | Observação livre |

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Registro atualizado, retorna o recurso atualizado |
| `404 Not Found` | Registro não encontrado |

---

### `DELETE /api/timeevents/{id}`
Remove um registro de ponto.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do registro |

**Responses**
| Status | Descrição |
|--------|-----------|
| `204 No Content` | Removido com sucesso |
| `404 Not Found` | Registro não encontrado |

---

## Monthly Periods `/api/monthlyperiods`

### `GET /api/monthlyperiods`
Retorna todos os períodos mensais cadastrados.

**Response** `200 OK`
```json
[
  {
    "id": 1,
    "year": 2025,
    "month": 1,
    "status": "Open",
    "closedById": null,
    "closedAt": null
  }
]
```

---

### `GET /api/monthlyperiods/{id}`
Retorna um período mensal pelo ID.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do período |

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Período encontrado |
| `404 Not Found` | Período não existe |

---

### `GET /api/monthlyperiods/by-date/{year}/{month}`
Retorna o período de um mês específico consultando por ano e mês.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `year` | `int` | Ano (ex: `2025`) |
| `month` | `int` | Mês de 1 a 12 |

Exemplo: `GET /api/monthlyperiods/by-date/2025/1`

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Período encontrado |
| `404 Not Found` | Nenhum período cadastrado para esta data |

---

### `POST /api/monthlyperiods`
Cria um novo período mensal. Retorna erro se já existir um período para o mesmo mês e ano.

**Request body**
```json
{
  "year": 2025,
  "month": 1
}
```

| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| `year` | `int` | ✅ |
| `month` | `int` | ✅ |

**Responses**
| Status | Descrição |
|--------|-----------|
| `201 Created` | Período criado, retorna o recurso criado |
| `400 Bad Request` | Já existe um período para este mês e ano |

---

### `PUT /api/monthlyperiods/{id}`
Atualiza o status de um período. Usado para avançar o fluxo de fechamento mensal.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do período |

**Request body**
```json
{
  "status": "Closed",
  "closedById": 3
}
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `status` | `string` | Novo status do período (`Open`, `InReview`, `Closed`) |
| `closedById` | `int` | ID do usuário RH que realizou o fechamento |

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Período atualizado, retorna o recurso atualizado |
| `404 Not Found` | Período não encontrado |

---

### `DELETE /api/monthlyperiods/{id}`
Remove um período mensal.

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do período |

**Responses**
| Status | Descrição |
|--------|-----------|
| `204 No Content` | Removido com sucesso |
| `404 Not Found` | Período não encontrado |
