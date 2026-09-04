# Clyvo NET - API de Gestão Veterinária

## Descrição do Projeto

O **Clyvo NET** é uma API REST desenvolvida utilizando **ASP.NET Core Web API**, **Entity Framework Core** e **Oracle Database**, criada para gerenciamento de uma clínica veterinária.

A API permite o cadastro e gerenciamento das informações relacionadas a:

- Pessoas
- Contatos
- Responsáveis
- Veterinários
- Animais
- Consultas

Além do CRUD completo (Create, Read, Update e Delete), a aplicação também possui consultas específicas utilizando filtros para facilitar a busca de informações.

---

# Tecnologias Utilizadas

- C#
- ASP.NET Core Web API
- Entity Framework Core
- Oracle Database
- Swagger/OpenAPI
- Git/GitHub
- Visual Studio 2022

---

# Estrutura do Projeto

```bash
Challenge_Clyvo_NET
│
├── Controllers
├── Models
├── Data
├── Migrations
├── Properties
├── appsettings.json
├── Program.cs
└── README.md
```

---

# Entidades do Sistema

## Pessoa

Armazena:

- Nome
- CPF
- Data de nascimento

## Contato

Armazena:

- Número de telefone
- Email

## Responsável

Armazena:

- Endereço
- Pessoa associada

## Veterinário

Armazena:

- Especialidade
- Pessoa associada

## Animal

Armazena:

- Nome
- Idade
- Espécie
- Raça
- Sexo
- Peso
- Responsável

## Consulta

Armazena:

- Data de agendamento
- Data da consulta
- Animal
- Veterinário

---

# Documentação das Rotas

## Animais

### GET

| Endpoint | Descrição |
|------------|------------|
| `/api/Animais` | Retorna todos os animais |
| `/api/Animais/{id}` | Retorna animal por ID |
| `/api/Animais/especie/{especie}` | Busca por espécie |
| `/api/Animais/raca/{raca}` | Busca por raça |
| `/api/Animais/responsavel/{id}` | Busca animais por responsável |

### POST

| Endpoint |
|-----------|
| `/api/Animais` |

Exemplo:

```json
{
  "nome":"Rex",
  "idade":5,
  "especie":"Cachorro",
  "raca":"Labrador",
  "sexo":"M",
  "dataNascimento":"2021-03-15",
  "peso":25.5,
  "responsavelId":1
}
```

### PUT

| Endpoint |
|-----------|
| `/api/Animais?id={id}` |

### DELETE

| Endpoint |
|-----------|
| `/api/Animais?id={id}` |

---

## Consulta

### GET

| Endpoint | Descrição |
|------------|------------|
| `/api/Consulta` | Retorna todas as consultas |
| `/api/Consulta/{id}` | Retorna consulta por ID |
| `/api/Consulta/animal/{animalId}` | Busca consultas por animal |
| `/api/Consulta/veterinario/{veterinarioId}` | Busca consultas por veterinário |

### POST

| Endpoint |
|-----------|
| `/api/Consulta` |

Exemplo:

```json
{
  "dataAgendamento":"2026-05-25",
  "dataConsulta":"2026-05-28",
  "animalId":1,
  "veterinarioId":1
}
```

### PUT

| Endpoint |
|-----------|
| `/api/Consulta?id={id}` |

### DELETE

| Endpoint |
|-----------|
| `/api/Consulta?id={id}` |

---

## Contato

### GET

| Endpoint | Descrição |
|------------|------------|
| `/api/Contato` | Retorna todos os contatos |
| `/api/Contato/{id}` | Busca contato por ID |
| `/api/Contato/numero/{numero}` | Busca contato por número |
| `/api/Contato/pessoa/{pessoaId}` | Busca contato por pessoa |

### POST

| Endpoint |
|-----------|
| `/api/Contato` |

### PUT

| Endpoint |
|-----------|
| `/api/Contato?id={id}` |

### DELETE

| Endpoint |
|-----------|
| `/api/Contato?id={id}` |

---

## Pessoas

### GET

| Endpoint | Descrição |
|------------|------------|
| `/api/pessoas` | Retorna todas as pessoas |
| `/api/pessoas/{id}` | Busca pessoa por ID |
| `/api/pessoas/cpf/{cpf}` | Busca pessoa por CPF |

### POST

| Endpoint |
|-----------|
| `/api/pessoas` |

Exemplo:

```json
{
  "nome":"João Silva",
  "cpf":"12345678900",
  "dataNascimento":"1990-05-20"
}
```

### PUT

| Endpoint |
|-----------|
| `/api/pessoas/{id}` |

### DELETE

| Endpoint |
|-----------|
| `/api/pessoas/{id}` |

---

## Responsável

### GET

| Endpoint | Descrição |
|------------|------------|
| `/api/Responsavel` | Retorna todos os responsáveis |
| `/api/Responsavel/{id}` | Busca responsável por ID |
| `/api/Responsavel/endereco/{endereco}` | Busca por endereço |
| `/api/Responsavel/pessoa/{pessoaId}` | Busca responsável por pessoa |

### POST

| Endpoint |
|-----------|
| `/api/Responsavel` |

### PUT

| Endpoint |
|-----------|
| `/api/Responsavel?id={id}` |

### DELETE

| Endpoint |
|-----------|
| `/api/Responsavel?id={id}` |

---

## Veterinário

### GET

| Endpoint | Descrição |
|------------|------------|
| `/api/Veterinario` | Retorna todos os veterinários |
| `/api/Veterinario/{id}` | Busca veterinário por ID |
| `/api/Veterinario/especialidade/{especialidade}` | Busca por especialidade |
| `/api/Veterinario/pessoa/{pessoaId}` | Busca por pessoa |

### POST

| Endpoint |
|-----------|
| `/api/Veterinario` |

### PUT

| Endpoint |
|-----------|
| `/api/Veterinario?id={id}` |

### DELETE

| Endpoint |
|-----------|
| `/api/Veterinario?id={id}` |

---

# Instalação e Execução

## Pré-requisitos

Instalar:

- .NET SDK 8
- Oracle Database
- Visual Studio 2022
- Git

---

## Clonar repositório

```bash
git clone https://github.com/nfreitas2000/Challenge_Clyvo_NET.git
```

Entrar na pasta:

```bash
cd Challenge_Clyvo_NET
```

---

## Restaurar dependências

```bash
dotnet restore
```

---

## Configurar conexão com banco

Abrir:

```json
appsettings.json
```

Editar:

```json
"ConnectionStrings": {
   "OracleConnection":"User Id=usuario;Password=senha;Data Source=localhost:1521/XEPDB1"
}
```

---

## Executar Migrations

Pelo Package Manager Console:

```powershell
Update-Database
```

ou pelo terminal:

```bash
dotnet ef database update
```

---

## Executar projeto

```bash
dotnet run
```

---

## Swagger

Após executar a aplicação:

```txt
/swagger
```

A interface Swagger permitirá visualizar e testar todos os endpoints disponíveis.

---

# Integrantes

- RM562979 - Caio Kenzo Tayra – 2TDSPI
- RM563000 - Enzo Vieira Bernardini - 2TDSPI
- RM564992 - Natan Freitas de Moraes – 2TDSPI
- RM561857 - Nicolas Mota Cândido - 2TDSPI# Challenge_Clyvo_NET
