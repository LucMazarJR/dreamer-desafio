# API Routes

> Documentação interativa disponível em `/scalar/v1` (ambiente de desenvolvimento).

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
  "expiresAt": "2026-05-27T16:00:00Z",
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

### `GET /api/auth/me`
Retorna os dados do usuário autenticado com base no token JWT. Útil para o frontend carregar o perfil do usuário logado sem precisar armazenar o ID separadamente.

**Permissão:** qualquer usuário autenticado

**Response** `200 OK`
```json
{
  "id": 1,
  "name": "João Silva",
  "email": "joao@empresa.com",
  "role": "Collaborator",
  "cpf": "123.456.789-00",
  "timeZone": "America/Sao_Paulo",
  "managerId": 2,
  "isActive": true,
  "createdAt": "2026-01-15T10:00:00Z"
}
```

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Dados do usuário autenticado |
| `401 Unauthorized` | Token inválido ou ausente |
| `404 Not Found` | Usuário não encontrado |

---

## Recursos

- [Time](#time-apitime)
- [Users](#users-apiusers)
- [Time Events](#time-events-apitimeevents)
- [Monthly Periods](#monthly-periods-apimonthlyperiods)

---

## Time `/api/time`

### `GET /api/time`
Retorna o horário atual do servidor em UTC. Usado pelo frontend para validar o timestamp dos registros de ponto.

**Permissão:** pública

**Response** `200 OK`
```json
{
  "utc": "2026-05-28T18:00:00.000Z"
}
```

---

## Users `/api/users`

### `GET /api/users`
Retorna todos os usuários ativos. Aceita filtro opcional por gestor via query param.

**Permissão:** `Manager`, `HrAdmin`

**Query params**
| Nome | Tipo | Obrigatório | Descrição |
|------|------|-------------|-----------|
| `managerId` | `int` | ❌ | Filtra pelos colaboradores de um gestor específico |

Exemplos:
- `GET /api/users`, retorna todos os usuários ativos
- `GET /api/users?managerId=2`, retorna apenas os colaboradores do gestor 2

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
    "createdAt": "2026-01-15T10:00:00Z"
  }
]
```

---

### `GET /api/users/{id}`
Retorna um usuário pelo ID. Retorna `404` se o usuário não existir ou estiver inativo.

**Permissão:** qualquer usuário autenticado

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

**Permissão:** `HrAdmin`

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
| `timeZone` | `string` | ✅ | Fuso horário no formato IANA (ex: `America/Sao_Paulo`, `Europe/Paris`) |
| `managerId` | `int` | ❌ | ID do gestor responsável |

**Responses**
| Status | Descrição |
|--------|-----------|
| `201 Created` | Usuário criado, retorna o recurso criado |
| `400 Bad Request` | E-mail já cadastrado ou gestor informado inválido |

---

### `PUT /api/users/{id}`
Atualiza os dados de um usuário. Todos os campos são opcionais, apenas os campos enviados são alterados.

**Permissão:** `HrAdmin`

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do usuário |

**Request body**
```json
{
  "name": "João Silva Atualizado",
  "email": "novo@empresa.com",
  "timeZone": "Europe/Paris",
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
| `400 Bad Request` | E-mail já em uso ou gestor informado inválido |

---

### `DELETE /api/users/{id}`
Remove um usuário via soft delete, marcando `isActive` como `false`. O registro é mantido no banco.

**Permissão:** `HrAdmin`

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

**Permissão:** qualquer usuário autenticado

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
    "recordedAt": "2026-05-27T08:00:00Z",
    "timezoneAtRecording": "America/Sao_Paulo",
    "isTravel": false,
    "observation": "home office",
    "createdAt": "2026-05-27T08:00:00Z"
  }
]
```

---

### `GET /api/timeevents/summary`
Retorna um resumo da jornada do colaborador em um mês específico, com o total de horas trabalhadas, horas extras e banco negativo. O cálculo considera pares entrada/saída e desconta os intervalos registrados.

**Permissão:** qualquer usuário autenticado

**Query params**
| Nome | Tipo | Obrigatório | Descrição |
|------|------|-------------|-----------|
| `userId` | `int` | ✅ | ID do colaborador |
| `year` | `int` | ✅ | Ano (ex: `2026`) |
| `month` | `int` | ✅ | Mês de 1 a 12 |

Exemplo: `GET /api/timeevents/summary?userId=1&year=2026&month=5`

**Response** `200 OK`
```json
{
  "userId": 1,
  "year": 2026,
  "month": 5,
  "daysWorked": 20,
  "totalWorkedMinutes": 9800,
  "totalExpectedMinutes": 9600,
  "overtimeMinutes": 200,
  "negativeMinutes": 0,
  "periodStatus": "Open"
}
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `daysWorked` | `int` | Dias com par entrada/saída completo |
| `totalWorkedMinutes` | `int` | Total de minutos trabalhados no mês |
| `totalExpectedMinutes` | `int` | Total esperado (dias trabalhados × 480 min) |
| `overtimeMinutes` | `int` | Minutos acima do esperado |
| `negativeMinutes` | `int` | Minutos abaixo do esperado |
| `periodStatus` | `string` | Status do período (`Open`, `InReview`, `Closed`, `NoPeriod`) |

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Resumo calculado |
| `404 Not Found` | Usuário não encontrado ou inativo |

---

### `GET /api/timeevents/{id}`
Retorna um registro de ponto pelo ID.

**Permissão:** qualquer usuário autenticado

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
Cria um novo registro de ponto. `recordedAt` deve ser enviado em UTC. Retorna erro se o período do mês estiver fechado.

**Permissão:** qualquer usuário autenticado

**Request body**
```json
{
  "userId": 1,
  "eventType": "Entry",
  "recordedAt": "2026-05-27T08:00:00Z",
  "timezoneAtRecording": "UTC-3",
  "isTravel": false,
  "observation": "home office"
}
```

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `userId` | `int` | ✅ | ID do colaborador |
| `eventType` | `string` | ✅ | Tipo do evento (`Entry`, `Exit`, `BreakStart`, `BreakEnd`) |
| `recordedAt` | `datetime` | ✅ | Momento do registro em UTC |
| `timezoneAtRecording` | `string` | ✅ | Fuso horário no momento do registro, formato IANA (ex: `America/Sao_Paulo`) |
| `isTravel` | `bool` | ❌ | Indica se o colaborador estava em deslocamento. Padrão: `false` |
| `observation` | `string` | ❌ | Observação livre |

**Responses**
| Status | Descrição |
|--------|-----------|
| `201 Created` | Registro criado, retorna o recurso criado |
| `400 Bad Request` | Período do mês está fechado |
| `404 Not Found` | Usuário não encontrado ou inativo |

---

### `PUT /api/timeevents/{id}`
Atualiza um registro de ponto. Todos os campos são opcionais, apenas os campos enviados são alterados. Retorna erro se o período do mês estiver fechado.

**Permissão:** `Manager`, `HrAdmin`

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do registro |

**Request body**
```json
{
  "eventType": "Exit",
  "recordedAt": "2026-05-27T17:00:00Z",
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
| `400 Bad Request` | Período do mês está fechado |
| `404 Not Found` | Registro não encontrado |

---

### `DELETE /api/timeevents/{id}`
Remove um registro de ponto. Retorna erro se o período do mês estiver fechado.

**Permissão:** `Manager`, `HrAdmin`

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do registro |

**Responses**
| Status | Descrição |
|--------|-----------|
| `204 No Content` | Removido com sucesso |
| `400 Bad Request` | Período do mês está fechado |
| `404 Not Found` | Registro não encontrado |

---

## Monthly Periods `/api/monthlyperiods`

### `GET /api/monthlyperiods`
Retorna todos os períodos mensais cadastrados.

**Permissão:** qualquer usuário autenticado

**Response** `200 OK`
```json
[
  {
    "id": 1,
    "year": 2026,
    "month": 5,
    "status": "Open",
    "closedById": null,
    "closedAt": null
  }
]
```

---

### `GET /api/monthlyperiods/{id}`
Retorna um período mensal pelo ID.

**Permissão:** qualquer usuário autenticado

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

**Permissão:** qualquer usuário autenticado

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `year` | `int` | Ano (ex: `2026`) |
| `month` | `int` | Mês de 1 a 12 |

Exemplo: `GET /api/monthlyperiods/by-date/2026/5`

**Responses**
| Status | Descrição |
|--------|-----------|
| `200 OK` | Período encontrado |
| `404 Not Found` | Nenhum período cadastrado para esta data |

---

### `POST /api/monthlyperiods`
Cria um novo período mensal. Retorna erro se já existir um período para o mesmo mês e ano.

**Permissão:** `HrAdmin`

**Request body**
```json
{
  "year": 2026,
  "month": 5
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
Atualiza o status de um período. Usado para avançar o fluxo de fechamento mensal. As transições permitidas são `Open → InReview`, `Open → Closed` e `InReview → Closed`. O status `Closed` é terminal.

**Permissão:** `Manager`, `HrAdmin`

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
| `400 Bad Request` | Transição de status inválida |
| `404 Not Found` | Período não encontrado |

---

### `DELETE /api/monthlyperiods/{id}`
Remove um período mensal. Períodos com status `Closed` não podem ser removidos.

**Permissão:** `HrAdmin`

**Parâmetros de rota**
| Nome | Tipo | Descrição |
|------|------|-----------|
| `id` | `int` | ID do período |

**Responses**
| Status | Descrição |
|--------|-----------|
| `204 No Content` | Removido com sucesso |
| `400 Bad Request` | Período fechado não pode ser removido |
| `404 Not Found` | Período não encontrado |
