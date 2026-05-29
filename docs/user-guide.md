# Guia de Uso — DDG Journey

> Plataforma de registro e gestão da jornada de trabalho do D&D Group.

---

## Sumário

1. [Acesso ao sistema](#1-acesso-ao-sistema)
2. [Navegação e layout](#2-navegação-e-layout)
3. [Registro de Ponto — Dashboard](#3-registro-de-ponto--dashboard)
4. [Histórico de Ponto](#4-histórico-de-ponto)
5. [Gestão de Usuários](#5-gestão-de-usuários)
6. [Períodos Mensais](#6-períodos-mensais)
7. [Perfis e permissões](#7-perfis-e-permissões)

---

## 1. Acesso ao sistema

### Login

Acesse `http://localhost:4200` e informe e-mail e senha.

| Campo | Descrição |
|-------|-----------|
| E-mail | Endereço de e-mail cadastrado |
| Senha | Senha definida pelo RH no cadastro |
| Lembrar de mim | Mantém a sessão após fechar o navegador (localStorage). Desmarcado, a sessão expira ao fechar a aba (sessionStorage) |

**Credenciais padrão (admin):**
- E-mail: `admin@ddgroup.com`
- Senha: `Admin@123`

Após o login bem-sucedido, o sistema redireciona automaticamente para o **Dashboard**.

---

## 2. Navegação e layout

A navegação principal fica na barra lateral (sidebar), disponível em todas as telas autenticadas.

### Itens do menu

| Item | Visível para | Destino |
|------|-------------|---------|
| Dashboard | Todos | Registro de ponto |
| Histórico | Todos | Lista de eventos |
| Usuários | Gestor e RH | Gestão de colaboradores |
| Períodos | RH / Admin | Gestão de fechamentos |

### Informações do usuário

No rodapé da sidebar há um cartão com o nome, iniciais e cargo do usuário logado.

### Mobile

Em telas menores, a sidebar é recolhida por padrão. O ícone de menu no cabeçalho abre e fecha o painel. O botão de fechar (×) também está disponível dentro da sidebar.

### Logout

O botão de logout fica no canto superior direito do cabeçalho, disponível em qualquer tela.

---

## 3. Registro de Ponto — Dashboard

Tela principal do colaborador. Permite registrar os eventos da jornada diária com validação de sequência.

### Relógio e horário

- O relógio exibe o **horário atual sincronizado com o servidor**, evitando distorções por relógio local desajustado.
- O horário é exibido no fuso horário cadastrado do usuário.

### Sequência de eventos

Os botões de ação seguem uma **máquina de estados** que impede registros fora de ordem:

```
[Entrada] → habilitado sempre que não há jornada aberta no dia
    ↓
[Início de Intervalo] → habilitado após Entrada
[Saída]               → habilitado após Entrada (sem intervalo aberto)
    ↓ (após Início de Intervalo)
[Fim de Intervalo] → habilitado após Início de Intervalo
    ↓
[Saída] → habilitado após Fim de Intervalo
```

Botões indisponíveis para o estado atual aparecem desativados (cinza).

### Registrando um evento

O registro acontece em **dois passos**:

**Passo 1 — Selecionar o evento**

Clique no botão correspondente ao evento que deseja registrar (Entrada, Início de Intervalo, Fim de Intervalo ou Saída). Um painel de confirmação abre abaixo dos botões.

**Passo 2 — Confirmar o registro**

O painel exibe o horário exato do evento e permite configurar opções adicionais antes de confirmar:

| Campo | Descrição |
|-------|-----------|
| Fuso horário | Fuso em que o colaborador está no momento do registro. Por padrão, usa o fuso cadastrado. Pode ser alterado para casos de viagem. |
| Em deslocamento | Checkbox que sinaliza que o colaborador está em outro país/fuso. Aparece na listagem do histórico para contextualizar registros em fusos diferentes do habitual. |
| Observação | Campo livre (opcional). Exemplos: "home office", "viagem São Paulo", "cliente X". |

Clique em **Confirmar** para gravar o evento ou em **Cancelar** para descartar.

### Resumo do dia

Abaixo dos botões de ação, o painel exibe:

- **Eventos do dia:** lista os registros feitos hoje com horário no fuso local
- **Status da jornada em andamento:** "Em jornada desde HH:MM", "Em intervalo desde HH:MM" ou "Jornada encerrada"
- **Total trabalhado no dia** (calculado em tempo real)

### Resumo mensal

No rodapé do dashboard:

| Campo | Descrição |
|-------|-----------|
| Dias trabalhados | Dias com par entrada/saída completo no mês |
| Horas trabalhadas | Total de minutos trabalhados convertido em horas |
| Horas esperadas | Total esperado com base nos dias trabalhados (8h/dia) |
| Horas extras | Excedente acima do esperado |
| Banco negativo | Déficit abaixo do esperado |
| Status do período | Open, InReview ou Closed |

### Avisos de período

- **Período fechado:** quando o RH encerrou o período do mês atual, os botões de registro ficam bloqueados e uma mensagem de aviso é exibida.
- **Sem período:** quando nenhum período foi criado para o mês atual, um aviso é exibido (o RH precisa criar o período antes de registros serem feitos).
- **Em revisão:** quando o período está em revisão, um aviso visual indica o status.

---

## 4. Histórico de Ponto

Lista completa dos registros de ponto com filtros e ações de edição (para Gestor e RH).

### Filtros

| Filtro | Descrição |
|--------|-----------|
| Mês / Ano | Seletor no topo da página. Navega entre meses usando as setas `‹` `›`. |
| Busca por nome | Campo de texto. Filtra os registros pelo nome do colaborador (visível para Gestor e RH). |

### Lista de eventos

Cada linha exibe:

- **Avatar** com iniciais do colaborador (cor consistente por usuário)
- **Nome** do colaborador
- **Tipo do evento** com badge colorido:
  - `Entrada` — verde
  - `Saída` — vermelho
  - `Início de Intervalo` — amarelo
  - `Fim de Intervalo` — azul
- **Horário no fuso de registro** — horário local no momento em que o evento foi feito
- **Horário no fuso do visualizador** — equivalente no fuso cadastrado do usuário que está vendo (exibido quando diferente)
- **Flag de deslocamento** — ícone de avião quando `isTravel = true`
- **Observação** — texto livre informado no registro

### Paginação

- 20 registros por página
- Navegação por botões de página com reticências para intervalos grandes
- Indicador "X de Y páginas"

### Edição de registro (Gestor e RH)

Cada linha tem um botão de **editar** (ícone de lápis). Ao clicar, os campos da linha ficam editáveis inline:

| Campo editável | Descrição |
|----------------|-----------|
| Tipo do evento | Dropdown: Entry, Exit, BreakStart, BreakEnd |
| Data e hora | Campos de data e hora separados |
| Fuso horário | Seletor de timezone IANA |
| Em deslocamento | Checkbox |
| Observação | Campo de texto livre |

Confirme com o botão **salvar** (ícone de check) ou cancele com **×**. A edição é bloqueada se o período do mês estiver fechado.

### Exclusão de registro (Gestor e RH)

Cada linha tem um botão de **excluir** (ícone de lixeira). A exclusão é bloqueada se o período estiver fechado.

### Visibilidade por perfil

| Perfil | O que vê |
|--------|----------|
| Colaborador | Apenas os próprios registros |
| Gestor | Próprios registros + registros dos colaboradores do seu time |
| RH / Admin | Registros de todos os colaboradores |

---

## 5. Gestão de Usuários

Acesso restrito a **Gestores** e **RH / Admin**. Permite cadastrar, editar e visualizar colaboradores.

### Visualização em lista

A aba padrão exibe todos os usuários em formato de lista com:

- Avatar com iniciais e cor consistente
- Nome, e-mail e cargo
- Status (ativo/inativo)

**Busca:** campo de texto no topo filtra por nome ou e-mail em tempo real.

### Visualização em times (RH / Admin)

O botão **Times** alterna para uma visão agrupada por gestor responsável. Cada grupo é expansível e mostra os colaboradores sob aquele gestor. Colaboradores sem gestor ficam em um grupo separado "Sem gestor".

### Cadastrar novo usuário

Clique no botão **+ Novo usuário** (visível apenas para RH). O formulário de cadastro possui três seções:

**Informações pessoais**

| Campo | Obrigatório | Descrição |
|-------|-------------|-----------|
| Nome | ✅ | Nome completo |
| E-mail | ✅ | Endereço de e-mail único |
| CPF | ❌ | CPF do colaborador |

**Acesso**

| Campo | Obrigatório | Descrição |
|-------|-------------|-----------|
| Senha | ✅ | Senha de acesso inicial |
| Cargo | ✅ | Colaborador, Gestor ou RH/Admin |

**Configurações de timezone**

| Campo | Obrigatório | Descrição |
|-------|-------------|-----------|
| Fuso horário | ✅ | Seletor com todos os fusos IANA. Padrão: `America/Sao_Paulo` |
| Gestor responsável | ❌ | Campo de busca com autocomplete. Exibe gestores disponíveis. |

Clique em **Criar usuário** para salvar.

### Editar usuário

Clique no ícone de editar ao lado do usuário na lista. O formulário de edição é idêntico ao de criação, com as diferenças:

- Senha não é exibida (só pode ser redefinida se informada novamente)
- Campo **Status** (Ativo / Inativo) fica disponível para desativar o colaborador (soft delete)

---

## 6. Períodos Mensais

Acesso restrito a **RH / Admin**. Controla o ciclo de fechamento mensal que bloqueia edições de ponto.

### Ciclo de vida de um período

```
[Aberto] → [Em Revisão] → [Fechado]
```

- **Aberto (Open):** período ativo, registros podem ser criados e editados normalmente.
- **Em revisão (InReview):** período em processo de conferência. Indica ao time que o mês está sendo apurado.
- **Fechado (Closed):** período encerrado. Nenhum registro do mês pode ser criado, editado ou excluído. Estado irreversível.

### Lista de períodos

Exibe todos os períodos cadastrados com:

- Mês e ano
- Data de início e fim do período
- Status com badge colorido (verde = aberto, amarelo = em revisão, vermelho = fechado)
- Quem realizou o fechamento e quando (para períodos fechados)

### Criar novo período

Clique em **+ Novo período**. Selecione o ano e o mês. Não é possível criar dois períodos para o mesmo mês e ano.

### Avançar status

Cada período aberto ou em revisão tem botões de ação:

| Ação | Transição | Quem pode |
|------|-----------|-----------|
| Iniciar revisão | Open → InReview | RH |
| Fechar período | InReview → Closed | RH |

O fechamento é irreversível. Após fechar, nenhum registro daquele mês pode ser alterado.

---

## 7. Perfis e permissões

Resumo completo das permissões por tela e ação:

### Dashboard

| Ação | Colaborador | Gestor | RH / Admin |
|------|:-----------:|:------:|:----------:|
| Registrar ponto | ✅ | ✅ | ✅ |
| Ver resumo do dia | ✅ | ✅ | ✅ |
| Ver resumo mensal | ✅ | ✅ | ✅ |

### Histórico

| Ação | Colaborador | Gestor | RH / Admin |
|------|:-----------:|:------:|:----------:|
| Ver próprios registros | ✅ | ✅ | ✅ |
| Ver registros do time | ❌ | ✅ | ✅ |
| Ver todos os registros | ❌ | ❌ | ✅ |
| Editar registros | ❌ | ✅ | ✅ |
| Excluir registros | ❌ | ✅ | ✅ |

### Usuários

| Ação | Colaborador | Gestor | RH / Admin |
|------|:-----------:|:------:|:----------:|
| Acessar a tela | ❌ | ✅ | ✅ |
| Ver lista de usuários | ❌ | ✅* | ✅ |
| Ver visão por times | ❌ | ❌ | ✅ |
| Criar usuário | ❌ | ❌ | ✅ |
| Editar usuário | ❌ | ❌ | ✅ |
| Desativar usuário | ❌ | ❌ | ✅ |

> *Gestor vê apenas os colaboradores do seu time.

### Períodos

| Ação | Colaborador | Gestor | RH / Admin |
|------|:-----------:|:------:|:----------:|
| Acessar a tela | ❌ | ❌ | ✅ |
| Ver períodos | ❌ | ❌ | ✅ |
| Criar período | ❌ | ❌ | ✅ |
| Iniciar revisão | ❌ | ❌ | ✅ |
| Fechar período | ❌ | ❌ | ✅ |
