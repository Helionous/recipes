using apirecipe.DataAccess.Query;
using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.OtherObject;
using apirecipe.Generic;
using apirecipe.ServiceObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apirecipe.Controllers
{
    public class NewsController : ControllerGeneric<SoNew>
    {
        [Authorize(Roles="Admin")]
        [HttpPost]
        [Route("[action]")]
        public ActionResult<DtoMessage> Create([FromBody] SoNew so)
        {
            try
            {
                DtoMessage validateNew = ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.title),
                    nameof(so.data.dto.subtitle),
                    nameof(so.data.dto.content),
                    nameof(so.data.dto.url)
                });
                _so.message.listMessage.AddRange(validateNew.listMessage);

                if (so.data.dto.idRecipe != null && Guid.Parse(so.data.dto.idRecipe) != Guid.Empty)
                {
                    QRecipe qRecipe = new QRecipe();
                    if (!qRecipe.ExistById(Guid.Parse(so.data.dto.idRecipe)))
                    {
                        _so.message.listMessage.Add("La receta seleccionada no existe, ingrese una válida.");
                        _so.message.Error();
                    }
                }
                
                if (so.data.dto.deletedAt == null || so.data.dto.deletedAt <= DateTime.Now)
                {
                    _so.message.listMessage.Add("La fecha de eliminación debe ser una fecha futura y no debe estar vacía.");
                    _so.message.Error();
                }
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                QNew qNew = new QNew();
                DtoNew dtoNewTemp = qNew.GetByTitle(so.data.dto.title);
                if (dtoNewTemp is not null && dtoNewTemp.title.ToLower() == so.data.dto.title.ToLower() )
                {
                    _so.message.listMessage.Add($"Este titulo ({so.data.dto.title}) ya se encuentra registrado en el sistema.");
                    _so.message.Error();
                    return Conflict( _so.message);
                }
                
                DtoNew dtoNewUrlTemp= qNew.GetByUrl(so.data.dto.url);
                if (dtoNewUrlTemp is not null && dtoNewUrlTemp.url == so.data.dto.url )
                {
                    _so.message.listMessage.Add("Esta imagen ya se encuentra registrado en el sistema.");
                    _so.message.Error();
                    return Conflict( _so.message);
                }

                if (qNew.CreateNew(so.data.dto) != 0)
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

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("[action]")]
        public ActionResult<DtoMessage> Update([FromBody] SoNew so)
        {
            try
            {
                DtoMessage validateNew = ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.title),
                    nameof(so.data.dto.subtitle),
                    nameof(so.data.dto.content),
                    nameof(so.data.dto.url),
                    nameof(so.data.dto.status)
                });
                _so.message.listMessage.AddRange(validateNew.listMessage);
                
                QNew qNew= new QNew();
                if (so.data.dto.id == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione la ID de la noticia a modificar.");
                }
                else
                {
                    if (!qNew.ExistById(so.data.dto.id))
                    {
                        _so.message.listMessage.Add("Esta ID de la noticia no existe seleccione una existente.");
                    }
                }
                
                if (so.data.dto.idRecipe != null && Guid.Parse(so.data.dto.idRecipe) != Guid.Empty)
                {
                    QRecipe qRecipe = new QRecipe();
                    if (!qRecipe.ExistById(Guid.Parse(so.data.dto.idRecipe)))
                    {
                        _so.message.listMessage.Add("La receta seleccionada no existe, ingrese una válida.");
                        _so.message.Error();
                    }
                }
                
                if (so.data.dto.deletedAt == null || so.data.dto.deletedAt <= DateTime.Now)
                {
                    _so.message.listMessage.Add("La fecha de eliminación debe ser una fecha futura y no debe estar vacía.");
                    _so.message.Error();
                }
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                DtoNew dtoObtainId = qNew.GetById(so.data.dto.id);
                DtoNew dtoNewTemp = qNew.GetByTitle(so.data.dto.title);
                if (dtoNewTemp is not null && dtoNewTemp.title.ToLower() == so.data.dto.title.ToLower()
                                           && dtoNewTemp.id != dtoObtainId.id)
                {
                    _so.message.listMessage.Add($"Este titulo ({so.data.dto.title}) ya se encuentra registrado en el sistema.");
                    _so.message.Error();
                    return Conflict( _so.message);
                }
                
                DtoNew dtoNewUrlTemp= qNew.GetByUrl(so.data.dto.url);
                if (dtoNewUrlTemp is not null && dtoNewUrlTemp.url == so.data.dto.url  && dtoNewUrlTemp.id != dtoObtainId.id)
                {
                    _so.message.listMessage.Add("Esta imagen ya se encuentra registrado en el sistema.");
                    _so.message.Error();
                    return Conflict( _so.message);
                }
                
                if (qNew.UpdateNew(so.data.dto) != 0)
                {
                    _so.message.listMessage.Add("Actualización exitosa!");
                    _so.message.Success();
                }
                else
                {
                    _so.message.listMessage.Add("No se pudo actualizar el registro.");
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
        
        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<SoNew> GetById( Guid id )
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _so.data.dto = null;
                    _so.message.listMessage.Add("Proporcione la ID de categoria válida.");
                    return Unauthorized(_so.message);
                }
                
                QNew qNew = new();
                _so.data.dto = qNew.GetById(id);
                if (_so.data.dto is not null)
                {
                    _so.message.Success();
                }
                else
                {
                    _so.data.dto = null;
                    _so.message.listMessage.Add("Noticia no encontrada.");
                    _so.message.Warning();
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
        [HttpGet]
        [Route("[action]")]
        public ActionResult<SoNew> GetAll()
        {
            try
            {
                QNew qNew = new();
                _so.data.dto = null;
                _so.data.listDto = qNew.GetAll();
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
        
        [Authorize(Roles="Admin")]
        [HttpDelete]
        [Route("[action]")]
        public ActionResult<DtoMessage> Delete(Guid id)
        {
            try
            {
                QNew qNew = new();
                if (id == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione la ID de receta válida.");
                }
                else
                {
                    if (!qNew.ExistById(id))
                    {
                        _so.message.listMessage.Add("Esta receta no existe seleccione una existente.");
                    }
                }
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                if (qNew.Delete(id) != 0)
                {
                    _so.message.listMessage.Add("Noticia eliminado correctamente.");
                    _so.message.Success();
                }
                else
                {
                    _so.message.listMessage.Add("No se pudo eliminar la noticia.");
                    _so.message.Error();
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
    }
}
