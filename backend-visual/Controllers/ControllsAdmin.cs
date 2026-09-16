using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ADMINISTRADORESController : ControllerBase
    {
        private readonly IADMINISTRADORESRepository _administradoresController;

        public ADMINISTRADORESController(IADMINISTRADORESRepository administradoresRepository)
        {
            _administradoresController = administradoresRepository;
        }

      
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAdministradores()
        {
            try
            {
                var response = await _administradoresController.GetAdministradores();

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

      
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerId(int id)
        {
            try
            {
                var response = await _administradoresController.GetAdministradorById(id);

                if (response == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "No se encontró el administrador."
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

      
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> PostAdministrador(ADMINISTRADORES administrador)
        {
            try
            {
                var response = await _administradoresController.PostAdministrador(administrador);

                if (response != null)
                {
                    return Ok(response);
                }

                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    mensaje = "No se pudo crear el administrador."
                });
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

        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAdministrador(ADMINISTRADORES administrador)
        {
            try
            {
                var response = await _administradoresController.PutAdministrador(administrador);

                if (response == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "No se pudo actualizar el administrador."
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

        
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAdministrador(ADMINISTRADORES administrador)
        {
            try
            {
                var response = await _administradoresController.DeleteAdministrador(administrador);

                if (response == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        mensaje = "No se pudo eliminar el administrador."
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