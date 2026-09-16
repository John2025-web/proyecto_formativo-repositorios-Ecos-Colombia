using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Tipo_Cont : ControllerBase
    {
        private readonly ITipo_ContenidoRepo _tipo_contenidoController;

        public Tipo_Cont(ITipo_ContenidoRepo Tipo_ContenidoRepository)
        {
            _tipo_contenidoController = Tipo_ContenidoRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetNotificacion()
        {
            {
                try
                {
                    var response = await _tipo_contenidoController.GetTipo_Contenido();

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
                    var response = await _tipo_contenidoController.GetTipo_ContenidoById(id);

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

        public async Task<IActionResult> PostTipo_Contenido(Tipo_Contenido Tipo_Contenido)
        {
            {
                try
                {
                    var response = await _tipo_contenidoController.PostTipo_Contenido(Tipo_Contenido);

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

        public async Task<IActionResult> PutTipo_Contenido(Tipo_Contenido Tipo_Contenido)
        {
            {
                try
                {
                    var response = await _tipo_contenidoController.PutTipo_Contenido(Tipo_Contenido);

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

        public async Task<IActionResult> DeleteTipo_Contenido(Tipo_Contenido Tipo_Contenido)
        {
            {
                try
                {
                    var response = await _tipo_contenidoController.DeleteTipo_Contenido(Tipo_Contenido);

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