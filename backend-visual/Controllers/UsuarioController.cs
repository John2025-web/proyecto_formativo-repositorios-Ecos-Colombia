using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUsuarioRepo _usuarioController;

        public UserController(IUsuarioRepo UsuarioRepository)
        {
            _usuarioController = UsuarioRepository;
        }

        [HttpGet("obtenerUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetUsuario()
        {
            {
                try
                {
                    var response = await _usuarioController.GetUsuario();

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
                    var response = await _usuarioController.GetUsuarioById(id);

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

        public async Task<IActionResult> PostUsuario(Usuario Usuario)
        {
            {
                try
                {
                    var response = await _usuarioController.PostUsuario(Usuario);

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

        public async Task<IActionResult> PutUsuario(Usuario Usuario)
        {
            {
                try
                {
                    var response = await _usuarioController.PutUsuario(Usuario);

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> DeleteUauario(Usuario Usuario)
        {
            {
                try
                {
                    var response = await _usuarioController.DeleteUsuario(Usuario);

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
    }
}