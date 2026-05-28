using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerApi.DTOs;
using Xunit;

namespace TaskManagerApi.Tests.Integration;

public class TasksEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public TasksEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_SemTarefas_Retorna200ComListaVazia()
    {
        var response = await _client.GetAsync("/api/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskResponseDto>>();
        tasks.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201()
    {
        var dto = new CreateTaskDto { Title = "Nova tarefa" };

        var response = await _client.PostAsJsonAsync("/api/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_RetornaLocationHeader()
    {
        var dto = new CreateTaskDto { Title = "Tarefa com header" };

        var response = await _client.PostAsJsonAsync("/api/tasks", dto);

        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_TarefaCriadaComStatusPending()
    {
        var dto = new CreateTaskDto { Title = "Tarefa status inicial" };

        var response = await _client.PostAsJsonAsync("/api/tasks", dto);
        var created = await response.Content.ReadFromJsonAsync<TaskResponseDto>();

        created!.Status.Should().Be("pending");
    }

    [Fact]
    public async Task GetById_TarefaExistente_Retorna200()
    {
        var created = await CriarTarefaAsync("Tarefa busca por id");

        var response = await _client.GetAsync($"/api/tasks/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_TarefaInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/tasks/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_FiltrandoPorStatus_RetornaSomenteFiltradas()
    {
        await CriarTarefaAsync("Tarefa para filtro");

        var response = await _client.GetAsync("/api/tasks?status=pending");
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskResponseDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tasks!.Should().OnlyContain(t => t.Status == "pending");
    }

    [Fact]
    public async Task Post_TituloAusente_Retorna400()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_TituloMuitoCurto_Retorna400()
    {
        var dto = new CreateTaskDto { Title = "ab" };

        var response = await _client.PostAsJsonAsync("/api/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ComDadosValidos_Retorna204()
    {
        var created = await CriarTarefaAsync("Tarefa para atualizar");
        var dto = new UpdateTaskDto { Title = "Titulo atualizado", Status = "in_progress" };

        var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Put_TarefaInexistente_Retorna404()
    {
        var dto = new UpdateTaskDto { Title = "Qualquer titulo", Status = "pending" };

        var response = await _client.PutAsJsonAsync("/api/tasks/999999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_StatusInvalido_Retorna400()
    {
        var created = await CriarTarefaAsync("Tarefa status invalido");
        var dto = new UpdateTaskDto { Title = "Titulo ok", Status = "invalido" };

        var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_TarefaDone_Retorna409()
    {
        var created = await CriarTarefaAsync("Tarefa que sera concluida");
        await _client.PutAsJsonAsync($"/api/tasks/{created.Id}",
            new UpdateTaskDto { Title = "Concluida", Status = "done" });

        var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}",
            new UpdateTaskDto { Title = "Tentativa", Status = "pending" });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Delete_TarefaExistente_Retorna204()
    {
        var created = await CriarTarefaAsync("Tarefa para deletar");

        var response = await _client.DeleteAsync($"/api/tasks/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_TarefaJaDeletada_Retorna404()
    {
        var created = await CriarTarefaAsync("Tarefa delete duplo");
        await _client.DeleteAsync($"/api/tasks/{created.Id}");

        var response = await _client.DeleteAsync($"/api/tasks/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<TaskResponseDto> CriarTarefaAsync(string titulo)
    {
        var dto = new CreateTaskDto { Title = titulo };
        var response = await _client.PostAsJsonAsync("/api/tasks", dto);
        return (await response.Content.ReadFromJsonAsync<TaskResponseDto>())!;
    }
}
