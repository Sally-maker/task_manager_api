using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.DTOs;

public class CreateTaskDto
{
    [Required(ErrorMessage = "O titulo e obrigatorio.")]
    [MinLength(3, ErrorMessage = "O titulo deve ter no minimo 3 caracteres.")]
    [MaxLength(200, ErrorMessage = "O titulo deve ter no maximo 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "A descricao deve ter no maximo 2000 caracteres.")]
    public string? Description { get; set; }
}
