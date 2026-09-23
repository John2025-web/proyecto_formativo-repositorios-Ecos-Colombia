using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using ApiPrueba.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class ADMINISTRADORESController : ControllerBase
    {
        private readonly IADMINISTRADORESRepository _administradoresController;
        private readonly AppDbContext _context;

        public ADMINISTRADORESController(IADMINISTRADORESRepository administradoresRepository, AppDbContext context)
        {
            _administradoresController = administradoresRepository;
            _context = context;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearAdministrador([FromBody] CrearAdministradorRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre)
                || string.IsNullOrWhiteSpace(request.Correo)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { mensaje = "Nombre, correo y contraseña son obligatorios." });
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });
            }

            var correo = request.Correo.Trim();
            var nombre = request.Nombre.Trim();
            var exists = await _context.ADMINISTRADORES.AnyAsync(x => x.Correo == correo || x.Nombre == nombre);
            if (exists)
            {
                return Conflict(new { mensaje = "Ya existe un administrador con ese nombre o correo." });
            }

            var administrador = new ADMINISTRADORES
            {
                Nombre = nombre,
                Correo = correo,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.ADMINISTRADORES.Add(administrador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerId), new { id = administrador.IdAdmin }, new
            {
                administrador.IdAdmin,
                administrador.Nombre,
                administrador.Correo
            });
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

    public class CrearAdministradorRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}