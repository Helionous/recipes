using apirecipe.Config;
using apirecipe.DataAccess.Query;
using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.ObjectEnum;
using apirecipe.DataTransferObject.OtherObject;
using apirecipe.Generic;
using apirecipe.ServiceObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apirecipe.Controllers
{
    public class UserController : ControllerGeneric<SoUser>
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public ActionResult<DtoMessage> Register([FromBody] SoUser so)
        {
            try
            {
                _so.message = ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.firstName),
                    nameof(so.data.dto.lastName),
                    nameof(so.data.dto.email),
                    nameof(so.data.dto.authetication.username),
                    nameof(so.data.dto.authetication.password)
                });
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                QUser qUser = new();
                EncryptWithAes aes = new();
                DtoUser? userUsername = qUser.GetByUsername(aes.Encrypt(so.data.dto.authetication.username));
                if (userUsername != null && aes.Decrypt(userUsername.authetication.username) == so.data.dto.authetication.username)
                {
                    _so.message.listMessage.Add($"Este usuario ({so.data.dto.authetication.username}) ya se encuentra registrado en el sistema.");
                    _so.message.Warning();
                    return Conflict( _so.message);
                }
                DtoUser? userEmail = qUser.GetByEmail(aes.Encrypt(so.data.dto.email));
                if (userEmail != null && aes.Decrypt(userEmail.email) == so.data.dto.email)
                {
                    _so.message.listMessage.Add($"Este correo ({so.data.dto.email}) ya se encuentra registrado en el sistema.");
                    _so.message.Warning();
                    return Conflict( _so.message);
                }
                
                if (so.data.dto != null)
                {
                    so.data.dto.authetication.id = Guid.NewGuid();
                    so.data.dto.authetication.username = aes.Encrypt(so.data.dto.authetication.username);
                    so.data.dto.authetication.password = BCrypt.Net.BCrypt.HashPassword(so.data.dto.authetication.password);
                    so.data.dto.authetication.role = Role.Logged;
                    so.data.dto.authetication.status = true;
                    so.data.dto.id = Guid.NewGuid();
                    so.data.dto.idAuthentication = so.data.dto.authetication.id.ToString();
                    so.data.dto.email = aes.Encrypt(so.data.dto.email);
                    so.data.dto.createdAt = DateTime.UtcNow;
                    so.data.dto.updatedAt = DateTime.UtcNow;
            
                    qUser.Register(so.data.dto);
                    _so.message.listMessage.Add("Registro exitoso!");
                    _so.message.Success();
                }
            }
            catch (Exception ex)
            {
                _so.message.listMessage.Add(ex.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return StatusCode(200, _so.message);
        }
        
        [Authorize(Roles="Admin,Other,Logged")]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<SoUser> MyProfile()
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                QUser qUser = new();
                _so.data.dto = qUser.MyProfile(idUser);
                if (_so.data.dto != null)
                {
                    EncryptWithAes aes = new();
                    _so.data.dto.email = aes.Decrypt(_so.data.dto.email);
                    _so.data.dto.authetication.username = aes.Decrypt(_so.data.dto.authetication.username);
                    _so.data.dto.authetication.password = null;
                    _so.message.Success();
                }
            }
            catch (Exception e)
            {
                _so.message.listMessage.Add(e.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return _so;
        }
        
        [Authorize(Roles="Admin,Other,Logged")]
        [HttpPut]
        [Route("[action]")]
        public ActionResult<DtoMessage> Update([FromBody] SoUser so)
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                var userValidation= ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.firstName),
                    nameof(so.data.dto.lastName),
                    nameof(so.data.dto.email),
                    nameof(so.data.dto.authetication.username)
                });
                _so.message.listMessage.AddRange(userValidation.listMessage);
                
                var authenticationValidation = ValidateDto(so.data.dto.authetication, new List<string>() 
                {
                    nameof(so.data.dto.authetication.username)
                });
                _so.message.listMessage.AddRange(authenticationValidation.listMessage);
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                QUser qUser = new();
                EncryptWithAes aes = new();
                DtoUser? userUsername = qUser.GetByUsername(aes.Encrypt(so.data.dto.authetication.username));
                
                if (userUsername != null && 
                    aes.Decrypt(userUsername.authetication.username) == so.data.dto.authetication.username
                    && idUser != userUsername.id)
                {
                    _so.message.listMessage.Add($"Este usuario ({so.data.dto.authetication.username}) ya se encuentra registrado en el sistema.");
                    _so.message.Warning();
                    return Conflict( _so.message);
                }
                DtoUser? userEmail = qUser.GetByEmail(aes.Encrypt(so.data.dto.email));
                if (userEmail != null && aes.Decrypt(userEmail.email) == so.data.dto.email && idUser != userEmail.id)
                {
                    _so.message.listMessage.Add($"Este correo ({so.data.dto.email}) ya se encuentra registrado en el sistema.");
                    _so.message.Warning();
                    return Conflict( _so.message);
                }
                
                DtoUser? dtoUser = qUser.GetById(idUser);
                if (so.data.dto != null)
                {
                    dtoUser.firstName = so.data.dto.firstName;
                    dtoUser.lastName = so.data.dto.lastName;
                    dtoUser.email = aes.Encrypt(so.data.dto.email);
                    dtoUser.authetication.username = aes.Encrypt(so.data.dto.authetication.username);
                    dtoUser.updatedAt = DateTime.UtcNow;
            
                    qUser.Update(dtoUser);
                    _so.message.listMessage.Add("Actualización exitosa!");
                    _so.message.Success();
                }
            }
            catch (Exception e)
            {
                _so.message.listMessage.Add(e.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return StatusCode(200, _so.message);
        }
        
        [Authorize(Roles="Admin")]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<SoUser> GetAll()
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                QUser qUser = new();
                EncryptWithAes aes = new();
                _so.data.dto = null;
                _so.data.listDto = qUser.GetAll(idUser);
                
                foreach (DtoUser user in _so.data.listDto)
                {
                    user.idAuthentication = null;
                    user.email = aes.Decrypt(user.email);
                    user.authetication.username = null;
                    user.authetication.password = null;
                }
                _so.message.Success();
            }
            catch (Exception e)
            {
                _so.message.listMessage.Add(e.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return _so;
        }
        
        [Authorize(Roles="Admin,Other,Logged")]
        [HttpPatch]
        [Route("[action]")]
        public ActionResult<DtoMessage> ChangePassword([FromBody] DtoPassword so)
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                _so.message = ValidateDto(so, new List<string>() 
                {
                    nameof(so.oldpassword),
                    nameof(so.newpassword)
                });

                if (_so.message.ExistsMessage()) 
                    return BadRequest(_so.message);

                QUser qUser = new();
                DtoUser? dtoUser = qUser.GetById(idUser);

                if (BCrypt.Net.BCrypt.Verify(so.oldpassword, dtoUser.authetication.password))
                {
                    if (so.oldpassword == so.newpassword)
                    {
                        _so.message.listMessage.Add("Su contraseña no puede ser la misma que la anterior.");
                        _so.message.Error();
                        return Conflict( _so.message);
                    }

                    string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(so.newpassword);
                    qUser.UpdatePassword(idUser, newHashedPassword);
                    _so.message.listMessage.Add("Su contraseña fue cambiada con éxito.");
                    _so.message.Success();
                }
                else
                {
                    _so.message.listMessage.Add("Su contraseña anterior no coincide.");
                    _so.message.Error();
                    return Unauthorized(_so.message);
                }
            }
            catch (Exception e)
            {
                _so.message.listMessage.Add(e.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return Ok(_so.message);
        }
        
        [Authorize(Roles="Admin")]
        [HttpPut]
        [Route("[action]")]
        public ActionResult<DtoMessage> PromoteOrDemoteUser([FromBody] SoUser so)
        {
            try
            {
                if (so.data.dto.id == Guid.Empty)
                {
                    _so.message.listMessage.Add("ID de usuario no válido.");
                    _so.message.Error();
                    return BadRequest(_so.message);
                }

                _so.message = ValidateDto(so.data.dto.authetication, new List<string>() 
                {
                    nameof(so.data.dto.authetication.role),
                });
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                QUser qUser = new();
                DtoUser? dtoUser = qUser.GetById(so.data.dto.id);
                if (dtoUser != null)
                {
                    dtoUser.authetication.role = so.data.dto.authetication.role;
                    dtoUser.authetication.status = so.data.dto.authetication.status;

                    if (qUser.UpdatePromoteOrDemote(dtoUser) != 0)
                    {
                        DtoUser? dtoUserTemp = qUser.GetById(dtoUser.id);
                        switch (dtoUserTemp.authetication.role)
                        {
                            case Role.Admin:
                                _so.message.listMessage.Add("Ahora el usuario es un Administrador.");
                                break;
                            case Role.Other:
                                _so.message.listMessage.Add("Ahora el usuario es un Moderador.");
                                break;
                            case Role.Logged:
                                _so.message.listMessage.Add("Ahora el usuario es un Cliente.");
                                break;
                            default:
                                _so.message.listMessage.Add("Rol desconocido.");
                                break;
                        }
                        if (!dtoUserTemp.authetication.status)
                        {
                            _so.message.listMessage.Add("La cuenta del usuario fue desactivada.");
                        }
                        _so.message.Success();
                    }
                }
                else
                {
                    _so.message.listMessage.Add("Usuario no encontrado verifique.");
                    _so.message.Error();
                    return Unauthorized(_so.message);
                }
            }
            catch (Exception e)
            {
                _so.message.listMessage.Add(e.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return Ok(_so.message);
        }
    }
}
