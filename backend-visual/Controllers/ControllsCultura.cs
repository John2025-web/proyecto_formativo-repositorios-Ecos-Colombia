using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CULTURAController : ControllerBase
    {
        private readonly ICULTURARepository _culturaController;

        public CULTURAController(ICULTURARepository culturaRepository)
        {
            _culturaController = culturaRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetCulturas()
        {
            {
                try
                {
                    var response = await _culturaController.GetCulturas();

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
                        error = ex.Message,
                        innerException = ex.InnerException?.Message
                    });
                }
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ObtenerId(int id)
        {
            {
                try
                {
                    var response = await _culturaController.GetCulturaById(id);

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

        public async Task<IActionResult> PostCultura(CULTURA cultura)
        {
            {
                try
                {
                    var response = await _culturaController.PostCultura(cultura);

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
                        error = ex.Message,
                        innerException = ex.InnerException?.Message
                    });
                }
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> PutCultura(CULTURA cultura)
        {
            {
                try
                {
                    var response = await _culturaController.PutCultura(cultura);

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

        public async Task<IActionResult> DeleteCultura(CULTURA cultura)
        {
            {
                try
                {
                    var response = await _culturaController.DeleteCultura(cultura);

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