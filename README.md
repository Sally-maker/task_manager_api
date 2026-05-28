# Task Manager API

API REST para gerenciamento de tarefas, desenvolvida com .NET 8 e ASP.NET Core.

## Tecnologias

- .NET 8 / ASP.NET Core 8
- Entity Framework Core 8 com SQLite
- Swagger / OpenAPI
- xUnit, NSubstitute e FluentAssertions (testes)
- Docker e Docker Compose

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/) (opcional)

## Executando localmente

```bash
dotnet run --project TaskManagerApi.csproj
```

A API ficará disponível em `http://localhost:5000`. O Swagger estará acessível em `http://localhost:5000/swagger`.

## Executando com Docker

```bash
docker compose up --build
```

A API ficará disponível em `http://localhost:5050`.

## Executando os testes

```bash
dotnet test
```

## Endpoints

| Método | Rota               | Descrição                              |
|--------|--------------------|----------------------------------------|
| GET    | /api/tasks         | Lista todas as tarefas                 |
| GET    | /api/tasks?status= | Filtra tarefas por status              |
| GET    | /api/tasks/{id}    | Retorna uma tarefa pelo id             |
| POST   | /api/tasks         | Cria uma nova tarefa                   |
| PUT    | /api/tasks/{id}    | Atualiza uma tarefa existente          |
| DELETE | /api/tasks/{id}    | Remove uma tarefa                      |

## Status disponíveis

| Valor         | Descrição      |
|---------------|----------------|
| `pending`     | Pendente       |
| `in_progress` | Em andamento   |
| `done`        | Concluída      |

> Tarefas com status `done` não podem ser atualizadas.

## Exemplos de uso

### Criar tarefa

```bash
curl -X POST http://localhost:5000/api/tasks \
  -H "Content-Type: application/json" \
  -d '{"title": "Minha tarefa", "description": "Descrição opcional"}'
```

### Listar tarefas

```bash
curl http://localhost:5000/api/tasks
curl http://localhost:5000/api/tasks?status=pending
```

### Atualizar tarefa

```bash
curl -X PUT http://localhost:5000/api/tasks/1 \
  -H "Content-Type: application/json" \
  -d '{"title": "Título atualizado", "status": "in_progress"}'
```

### Remover tarefa

```bash
curl -X DELETE http://localhost:5000/api/tasks/1
```

## Estrutura do projeto

```
src/
  Controllers/    Endpoints da API
  Data/           DbContext e configuração do banco
  DTOs/           Objetos de transferência de dados
  Migrations/     Migrations do EF Core
  Models/         Entidades do domínio
  Repositories/   Acesso a dados
  Services/       Regras de negócio
  Validation/     Atributos de validação customizados
tests/
  Integration/    Testes de integração com WebApplicationFactory
  Unit/           Testes unitários com mocks
```

## Licença

MIT
