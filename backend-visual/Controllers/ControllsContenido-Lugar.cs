using ApiPrueba.models;
using ApiPrueba.Models;
using ApiPrueba.Repositories;
using ApiPrueba.Repositories.@interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CONTENIDO_LUGARController : ControllerBase
    {
        private readonly ICONTENIDO_LUGARRepository _contenidoLugarController;

        public CONTENIDO_LUGARController(ICONTENIDO_LUGARRepository contenidoLugarRepository)
        {
            _contenidoLugarController = contenidoLugarRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetContenidoLugar()
        {
            {
                try
                {
                    var response = await _contenidoLugarController.GetContenidoLugar();

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

        [HttpGet("{idContenido}/{idLugar}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ObtenerId(int idContenido, int idLugar)
        {
            {
                try
                {
                    var response = await _contenidoLugarController.GetContenidoLugarById(idContenido, idLugar);

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

        public async Task<IActionResult> PostContenidoLugar(CONTENIDO_LUGAR contenidoLugar)
        {
            {
                try
                {
                    var response = await _contenidoLugarController.PostContenidoLugar(contenidoLugar);

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

        public async Task<IActionResult> PutContenidoLugar(CONTENIDO_LUGAR contenidoLugar)
        {
            {
                try
                {
                    var response = await _contenidoLugarController.PutContenidoLugar(contenidoLugar);

                    if (response == null)
                    {
                        return BadRequest(new
                        {
                            mensaje = "No se pudo obtener la información."
                        });
                    }

                    return Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized(new
                    {
                        mensaje = "No tiene autorización para realizar esta operación."
                    });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new
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

        public async Task<IActionResult> DeleteContenidoLugar(CONTENIDO_LUGAR contenidoLugar)
        {
            {
                try
                {
                    var response = await _contenidoLugarController.DeleteContenidoLugar(contenidoLugar);

                    if (response == null)
                    {
                        return BadRequest(new
                        {
                            mensaje = "No se pudo obtener la información."
                        });
                    }

                    return Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized(new
                    {
                        mensaje = "No tiene autorización para realizar esta operación."
                    });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new
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