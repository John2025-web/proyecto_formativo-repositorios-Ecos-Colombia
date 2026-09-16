using ApiPrueba.models;
using ApiPrueba.Models;
using ApiPrueba.Repositorios;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LugarTuriController : ControllerBase
    {
        private readonly ILugarTuristicoRepo _LugarTuriController;

        public LugarTuriController(ILugarTuristicoRepo lugarTuristicoRepository)
        {
            _LugarTuriController = lugarTuristicoRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLugarTuristico()
        {
            try
            {
                var response = await _LugarTuriController.GetLugarTuristico();

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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> obtenerId(int id)
        {
            try
            {
                var response = await _LugarTuriController.GetLugarTuristicoById(id);

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLugarTuristico(LUGAR_TURISTICO LugarTuristico)
        {
            try
            {
                var response = await _LugarTuriController.PostLugarTuristico(LugarTuristico);

                if (response == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "No se pudo crear el lugar turístico."
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

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutLugarTuristico(LUGAR_TURISTICO LugarTuristico)
        {
            try
            {
                var response = await _LugarTuriController.PutLugarTuristico(LugarTuristico);

                if (response == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "No se pudo actualizar el lugar turístico."
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

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteLugarTuristico(LUGAR_TURISTICO LugarTuristico)
        {
            try
            {
                var response = await _LugarTuriController.DeleteLugarTuristico(LugarTuristico);

                if (response == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "No se pudo eliminar el lugar turístico."
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
}