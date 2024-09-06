using apirecipe.Config;
using apirecipe.DataAccess.Query;
using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.OtherObject;
using apirecipe.Generic;
using apirecipe.Helper;
using apirecipe.ServiceObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apirecipe.Controllers
{
    public class AuthenticationController : ControllerGeneric<SoAuthentication>
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<SoAuthentication>>  Login([FromBody] SoAuthentication so)
        {
            try
            {
                _so.message = ValidateDto(so.data.dto, new List<string>() 
                { 
                    nameof(so.data.dto.username), 
                    nameof(so.data.dto.password)
                });

                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                QAuthentication qAuthentication = new QAuthentication();
                EncryptWithAes aes = new();
                if (!qAuthentication.ExistByUsername(aes.Encrypt(so.data.dto.username)))
                {
                    _so.message.listMessage.Add("Este usuario no se encuentra registrado en el sistema.");
                    _so.message.Error();
                    return Unauthorized(_so.message);
                }
                
                _so.data.dto = qAuthentication.GetByUsername(aes.Encrypt(so.data.dto.username));
                
                if (!_so.data.dto.status)
                {
                    _so.data.dto = null;
                    _so.message.listMessage.Add("Este usuario está deshabilitado.");
                    _so.message.Error();
                    return Unauthorized(_so.message);
                }
                
                DtoUser user = qAuthentication.GetUserByIdUsername(aes.Encrypt(so.data.dto.username));
                if (BCrypt.Net.BCrypt.Verify( so.data.dto.password, _so.data.dto.password) && user != null)
                {
                    user.authetication.username = aes.Decrypt(user.authetication.username);
                    _so.data.additional  = new Tokens {
                        accessToken = await TokenUtils.GenerateAccessToken(user),
                        refreshToken = await TokenUtils.GenerateRefreshToken(user)
                    };
                    _so.data.dto.username = aes.Decrypt(_so.data.dto.username);
                    _so.data.dto.password = null;
                    _so.message.listMessage.Add("Bienvenido al sistema.");
                    _so.message.Success();
                }
                else
                {
                    _so.data.dto = null;
                    _so.message.listMessage.Add("Usuario o contraseña incorrecta, verifique.");
                    _so.message.Error();
                    return Unauthorized(_so.message);
                }
            }
            catch (Exception ex)
            {
                _so.message.listMessage.Add(ex.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return _so;
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<SoAuthentication>> RefreshToken([FromBody] Tokens so)
        {
            try
            {
                if (!string.IsNullOrEmpty(so.refreshToken))
                {
                    var (tokens, message) = await TokenUtils.GenerateAccessTokenFromRefreshToken(
                        so.refreshToken, AppSettings.GetRefreshJwtSecret());
                    if (tokens != null)
                    {
                        _so.data.additional = tokens;    
                        _so.message = message;
                        return _so;
                    }

                    _so.message = message;
                    _so.message.Error();
                    return Unauthorized(_so.message);
                }
                _so.message.listMessage.Add("Proporcione el (Refresh Token).");
                _so.message.Error();
                return BadRequest(_so.message);
            }
            catch (Exception ex)
            {
                _so.message.listMessage.Add(ex.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
        }
    }
}
