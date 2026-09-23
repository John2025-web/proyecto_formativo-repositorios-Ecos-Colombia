using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiPrueba.DBcontext;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecenaController : ControllerBase
    {
        private readonly IRecenaRepo _recenaController;
        private readonly RecenaService _resenas;

        public RecenaController(IRecenaRepo RecenaRepository, RecenaService resenas)
        {
            _recenaController = RecenaRepository;
            _resenas = resenas;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetRecena()
        {
            {
                try
                {
                    var response = await _recenaController.GetRecena();

                    if (response == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            mensaje = "No se pudo obtener la información."
                        });
                    }

                    return Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new
                    {
                        mensaje = "No tiene autorización para realizar esta operación."
                    });
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "Los datos enviados no son válidos.",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        mensaje = "Ocurrió un error interno en el servidor.",
                        error = ex.Message
                    });
                }
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> obtenerId(int id)
        {
            {
                try
                {
                    var response = await _recenaController.GetRecenaById(id);

                    if (response == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            mensaje = "No se pudo obtener la información."
                        });
                    }

                    return Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new
                    {
                        mensaje = "No tiene autorización para realizar esta operación."
                    });
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "Los datos enviados no son válidos.",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        mensaje = "Ocurrió un error interno en el servidor.",
                        error = ex.Message
                    });
                }
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> PostRecena(Recena Recena)
        {
            {
                try
                {
                    var response = await _recenaController.PostRecena(Recena);

                    if (response == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            mensaje = "No se pudo obtener la información."
                        });
                    }

                    return Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new
                    {
                        mensaje = "No tiene autorización para realizar esta operación."
                    });
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "Los datos enviados no son válidos.",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        mensaje = "Ocurrió un error interno en el servidor.",
                        error = ex.Message
                    });
                }
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> PutRecena(Recena Recena)
        {
            {
                try
                {
                    var response = await _recenaController.PutRecena(Recena);

                    if (response == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            mensaje = "No se pudo obtener la información."
                        });
                    }

                    return Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new
                    {
                        mensaje = "No tiene autorización para realizar esta operación."
                    });
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "Los datos enviados no son válidos.",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        mensaje = "Ocurrió un error interno en el servidor.",
                        error = ex.Message
                    });
                }
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> DeleteRecena(Recena Recena)
        {
            {
                try
                {
                    var response = await _recenaController.DeleteRecena(Recena);

                    if (response == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            mensaje = "No se pudo obtener la información."
                        });
                    }

                    return Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new
                    {
                        mensaje = "No tiene autorización para realizar esta operación."
                    });
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "Los datos enviados no son válidos.",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        mensaje = "Ocurrió un error interno en el servidor.",
                        error = ex.Message
                    });
                }
            }
        }

        [HttpPost("{id}/censurar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Censurar(int id, [FromBody] CensurarResenaRequest request)
        {
            var resena = await _resenas.recena.FirstOrDefaultAsync(x => x.idRecena == id);
            if (resena == null) return NotFound(new { mensaje = "Reseña no encontrada." });
            if (request.Palabras == null || request.Palabras.Count == 0)
                return BadRequest(new { mensaje = "Indica al menos una palabra para censurar." });

            var comentario = resena.Comentario ?? string.Empty;
            foreach (var palabra in request.Palabras.Where(x => !string.IsNullOrWhiteSpace(x)))
                comentario = System.Text.RegularExpressions.Regex.Replace(comentario, $"\\b{System.Text.RegularExpressions.Regex.Escape(palabra.Trim())}\\b", "***", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            resena.Comentario = comentario;
            await _resenas.SaveChangesAsync();
            return Ok(resena);
        }
    }

    public class CensurarResenaRequest
    {
        public List<string> Palabras { get; set; } = new();
    }
}