using apirecipe.Config;
using apirecipe.DataAccess.Query;
using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.OtherObject;
using apirecipe.Generic;
using apirecipe.ServiceObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apirecipe.Controllers
{
    public class CategoryController : ControllerGeneric<SoCategory>
    {
        [Authorize(Roles="Admin")]
        [HttpPost]
        [Route("[action]")]
        public ActionResult<DtoMessage> Create([FromBody] SoCategory so)
        {
            try
            {
                _so.message = ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.name),
                    nameof(so.data.dto.description)
                });
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);

                QCategory qCategory = new QCategory();
                DtoCategory? dtoCategoryTemp = qCategory.GetByName(so.data.dto.name);
                if (dtoCategoryTemp is not null && dtoCategoryTemp.name.ToLower() == so.data.dto.name.ToLower() )
                {
                    _so.message.listMessage.Add($"Este nombre ({so.data.dto.name}) ya se encuentra registrado en el sistema.");
                    _so.message.Error();
                    return Conflict( _so.message);
                }
                
                if (qCategory.Create(so.data.dto) != 0)
                {
                    _so.message.listMessage.Add("Registro exitoso!");
                    _so.message.Success();
                }
                else
                {
                    _so.message.listMessage.Add("No se pudo crear el registro.");
                    _so.message.Warning();
                }
            }
            catch (Exception ex)
            {
                _so.message.listMessage.Add(ex.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return _so.message;
        }
        
        [Authorize(Roles="Admin")]
        [HttpPut]
        [Route("[action]")]
        public ActionResult<DtoMessage> Update([FromBody] SoCategory so)
        {
            try
            {
                if (so.data.dto.id == Guid.Empty)
                {
                    _so.message.listMessage.Add("ID de usuario no válido.");
                    _so.message.Error();
                    return BadRequest(_so.message);
                }
                _so.message = ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.name),
                    nameof(so.data.dto.description)
                });
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);

                QCategory qCategory = new QCategory();
                if (!qCategory.ExistCategoryById(so.data.dto.id))
                {
                    _so.message.listMessage.Add("Esta categoria no se encontró para su modicificación.");
                    _so.message.Error();
                    return Conflict( _so.message);
                }
                
                DtoCategory? dtoCategoryTemp = qCategory.GetByName(so.data.dto.name);
                if (dtoCategoryTemp is not null && dtoCategoryTemp.name.ToLower() == so.data.dto.name.ToLower() )
                {
                    _so.message.listMessage.Add($"Este nombre ({so.data.dto.name}) ya se encuentra registrado en el sistema.");
                    _so.message.Error();
                    return Conflict( _so.message);
                }
                
                if (qCategory.Update(so.data.dto) != 0)
                {
                    _so.message.listMessage.Add("Actualización exitoso!");
                    _so.message.Success();
                }
            }
            catch (Exception ex)
            {
                _so.message.listMessage.Add(ex.Message);
                _so.message.Exception();
                return StatusCode(500, _so.message);
            }
            return  _so.message;
        }
        
        [Authorize(Roles="Admin")]
        [HttpDelete]
        [Route("[action]")]
        public ActionResult<DtoMessage> Delete(Guid id, [FromBody] PasswordRequest data)
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));

                DtoMessage userValidation= ValidateDto(data, new List<string>() 
                {
                    nameof(data.Password),
                });
                _so.message.listMessage.AddRange(userValidation.listMessage);
                
                QCategory qCategory = new QCategory();
                if (id == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione la ID de categoria válida.");
                }
                else
                {
                    if (!qCategory.ExistCategoryById(id))
                    {
                        _so.message.listMessage.Add("Esta categoria no existe ingrese una existente.");
                    }
                }
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                QUser qUser = new();
                DtoUser dtoUser = qUser.GetById(idUser);
                if (BCrypt.Net.BCrypt.Verify( data.Password, dtoUser.authetication.password))
                {
                    QRecipe qRecipe = new();
                    if (qRecipe.ExistByIdCategory(id))
                    {
                        _so.message.listMessage.Add("No se puede eliminar esta categoria a un hay recetas relacionadas.");
                        _so.message.Warning();
                        return Unauthorized(_so.message);
                    }
                    
                    if (qCategory.Delete(id) != 0)
                    {
                        _so.message.listMessage.Add("Categoria eliminado correctamente.");
                        _so.message.Success();
                    }
                }
                else
                {
                    _so.message.listMessage.Add("Contraseña inválida.");
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

            return _so.message;
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<SoCategory>> GetByPagination(int pageNumber, int pageSize)
        {
            try
            {
                QCategory qCategory = new();
                _so.data.additional = new Pagination();
                (_so.data.listDto, _so.data.additional) = await qCategory.GetWithOffsetPagination(pageNumber, pageSize);
                if (_so.data.listDto.Count < 2)
                {
                    _so.data.dto = _so.data.listDto.First();
                    _so.data.listDto = null;
                }
                _so.message.Success();
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
        [HttpGet]
        [Route("[action]")]
        public ActionResult<SoCategory> GetAll()
        {
            try
            {
                QCategory qCategory = new();
                _so.data.dto = null;
                _so.data.listDto = qCategory.GetAll();
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
    }
}
