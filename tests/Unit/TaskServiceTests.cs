using FluentAssertions;
using NSubstitute;
using TaskManagerApi.Constants;
using TaskManagerApi.DTOs;
using TaskManagerApi.Models;
using TaskManagerApi.Repositories;
using TaskManagerApi.Services;
using Xunit;

namespace TaskManagerApi.Tests.Unit;

public class TaskServiceTests
{
    private readonly ITaskRepository _repo;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _repo = Substitute.For<ITaskRepository>();
        _service = new TaskService(_repo);
    }

    [Fact]
    public async Task GetAllAsync_SemFiltro_ChamaRepositorioComNull()
    {
        _repo.GetAllAsync(null).Returns([]);

        await _service.GetAllAsync(null);

        await _repo.Received(1).GetAllAsync(null);
    }

    [Fact]
    public async Task GetAllAsync_ComFiltro_ChamaRepositorioComStatus()
    {
        _repo.GetAllAsync("pending").Returns([]);

        await _service.GetAllAsync("pending");

        await _repo.Received(1).GetAllAsync("pending");
    }

    [Fact]
    public async Task GetByIdAsync_TarefaExistente_RetornaMapeado()
    {
        var task = new TaskItem { Id = 1, Title = "Teste", Status = TaskStatuses.Pending };
        _repo.GetByIdAsync(1).Returns(task);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Teste");
    }

    [Fact]
    public async Task GetByIdAsync_TarefaInexistente_RetornaNull()
    {
        _repo.GetByIdAsync(99).Returns((TaskItem?)null);

        var result = await _service.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_SempreDefinStatusPending()
    {
        var dto = new CreateTaskDto { Title = "Nova tarefa" };
        _repo.CreateAsync(Arg.Any<TaskItem>()).Returns(callInfo => callInfo.Arg<TaskItem>());

        var result = await _service.CreateAsync(dto);

        result.Status.Should().Be(TaskStatuses.Pending);
    }

    [Fact]
    public async Task CreateAsync_DefineTituloCorretamente()
    {
        var dto = new CreateTaskDto { Title = "Minha tarefa" };
        _repo.CreateAsync(Arg.Any<TaskItem>()).Returns(callInfo => callInfo.Arg<TaskItem>());

        var result = await _service.CreateAsync(dto);

        result.Title.Should().Be("Minha tarefa");
    }

    [Fact]
    public async Task CreateAsync_DefineTimestamps()
    {
        var dto = new CreateTaskDto { Title = "Com timestamps" };
        _repo.CreateAsync(Arg.Any<TaskItem>()).Returns(callInfo => callInfo.Arg<TaskItem>());
        var antes = DateTime.UtcNow.AddSeconds(-1);

        var result = await _service.CreateAsync(dto);

        result.CreatedAt.Should().BeAfter(antes);
        result.UpdatedAt.Should().BeAfter(antes);
    }

    [Fact]
    public async Task UpdateAsync_TarefaInexistente_RetornaFoundFalse()
    {
        _repo.GetByIdAsync(1).Returns((TaskItem?)null);

        var (found, conflict) = await _service.UpdateAsync(1, new UpdateTaskDto { Title = "X", Status = "pending" });

        found.Should().BeFalse();
        conflict.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_TarefaDone_RetornaConflictTrue()
    {
        var task = new TaskItem { Id = 1, Title = "Concluida", Status = TaskStatuses.Done };
        _repo.GetByIdAsync(1).Returns(task);

        var (found, conflict) = await _service.UpdateAsync(1, new UpdateTaskDto { Title = "X", Status = "pending" });

        found.Should().BeTrue();
        conflict.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_Sucesso_AtualizaCampos()
    {
        var task = new TaskItem { Id = 1, Title = "Antiga", Status = TaskStatuses.Pending };
        _repo.GetByIdAsync(1).Returns(task);
        var dto = new UpdateTaskDto { Title = "Nova", Description = "Descricao", Status = "in_progress" };

        await _service.UpdateAsync(1, dto);

        task.Title.Should().Be("Nova");
        task.Description.Should().Be("Descricao");
        task.Status.Should().Be("in_progress");
    }

    [Fact]
    public async Task UpdateAsync_Sucesso_RetornaFoundTrueConflictFalse()
    {
        var task = new TaskItem { Id = 1, Title = "Ok", Status = TaskStatuses.Pending };
        _repo.GetByIdAsync(1).Returns(task);

        var (found, conflict) = await _service.UpdateAsync(1, new UpdateTaskDto { Title = "Novo titulo", Status = "in_progress" });

        found.Should().BeTrue();
        conflict.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_TarefaInexistente_RetornaFalse()
    {
        _repo.GetByIdAsync(99).Returns((TaskItem?)null);

        var result = await _service.DeleteAsync(99);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_TarefaExistente_RetornaTrue()
    {
        var task = new TaskItem { Id = 1, Title = "Existente", Status = TaskStatuses.Pending };
        _repo.GetByIdAsync(1).Returns(task);

        var result = await _service.DeleteAsync(1);

        result.Should().BeTrue();
    }
}
