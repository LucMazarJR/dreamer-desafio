# Regras de Negócio

> Decisões assumidas estão marcadas com 🔵 e justificadas logo abaixo de cada seção.

---

## Perfis de Usuário

| Perfil | Descrição |
|--------|-----------|
| **Colaborador** | Registra a própria jornada e consulta seu histórico |
| **Gestor** | Acompanha a jornada dos colaboradores do seu time |
| **RH / Admin** | Gerencia colaboradores, realiza fechamentos e tem visão geral |

> **Decisões tomadas:** o desafio não especifica diretemante os perfis. Adotei três níveis por ser o modelo mais comum em plataformas de jornada corporativa e pela necessidade de divisão conforme as funções presentes na plataforma.

---

## Registro de Jornada

- O colaborador pode registrar: **entrada**, **saída** e **intervalos**
- Cada registro é vinculado ao colaborador e ao momento exato do evento (timestamp)
- O colaborador pode adicionar uma **observação** ao registro (ex: "home office", "viagem", "cliente X")
- 🔵 Registros não podem ser editados pelo colaborador após o fechamento do período
- 🔵 Gestor ou RH podem corrigir registros com justificativa

> **Decisões tomadas:** Registros não podem ser alterados pós fechamento para manter dados mais válidos. Gestor e RH podem corrigir registros pois podem ocorrer erros de marcação por parte do funcionário ou necessidade de ajuste pontual.

---

## Fuso Horário

- A empresa opera no **Brasil** e na **Europa**, os times estão em fusos diferentes
- 🔵 Todos os registros são armazenados em **UTC** no banco de dados
- 🔵 A exibição é feita no **fuso local do colaborador**, mas mostra equivalência para o usuário que está visualizando
- 🔵 O fuso horário é definido no cadastro do colaborador e pode ser atualizado pelo RH

> **Decisões tomadas:** Armazenamento em UTC para padronização e homogeneidade dos dados. Exibição com base no fuso definido pelo RH, com equivalência visível, pensado especialmente para contextos de viagem. O RH define e atualiza o fuso para garantir consistência dos dados.

---

## Jornada de Trabalho

- 🔵 Jornada padrão: **8h diárias** com **1h de intervalo** (base CLT para colaboradores no Brasil)
- 🔵 Para colaboradores na Europa, a jornada padrão segue o mesmo modelo até que regras específicas sejam definidas
- Horas além da jornada padrão são registradas como **horas extras**
- Horas abaixo da jornada são registradas como **banco negativo**

> **Decisões tomadas:** Jornada padrão com base no Brasil, mas permite personalização por colaborador caso siga um regime diferente, seja por tipo de contratação ou por padrão do país onde atua.

---

## Viagens e Deslocamentos

- 🔵 Colaboradores que viajam entre países mantêm o registro baseado no **fuso do local onde estão**
- 🔵 O colaborador pode informar que está em **deslocamento** ao registrar o ponto, para contextualizar registros em fusos diferentes do habitual

> **Decisões tomadas:** Usar o fuso do local onde o colaborador está torna o registro mais fiel à realidade do dia de trabalho. A flag de deslocamento existe para dar contexto ao registro, sem ela, um ponto feito num fuso diferente do cadastrado poderia parecer inconsistência, quando na verdade é uma viagem esperada.

---

## Fechamento Mensal

- O período de apuração é **mensal**
- O fechamento é realizado pelo **RH**
- Após o fechamento, os registros do período ficam **bloqueados para edição**
- 🔵 Antes do fechamento, o gestor pode revisar e aprovar a jornada do time

> **Decisões tomadas:** Incluir a etapa de revisão pelo gestor antes do fechamento pelo RH cria uma camada de validação mais próxima do time, o gestor conhece o contexto das ausências e ajustes melhor do que o RH, o que reduz erros no fechamento.

---

## Consulta e Relatórios

- **Colaborador:** visualiza apenas a própria jornada
- **Gestor:** visualiza a jornada dos colaboradores do seu time
- **RH / Admin:** visualiza todos os colaboradores
- A consulta permite filtrar por **período** e **colaborador**

---
