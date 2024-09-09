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
    public class RecipeController : ControllerGeneric<SoRecipe>
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<SoRecipe>> PaginationByCategory(Guid idCategory, int pageNumber, int pageSize)
        {
            try
            {
                QRecipe qRecipe = new();
                _so.data.additional = new Pagination();
                (_so.data.listDto, _so.data.additional) = await qRecipe.GetRecipesByCategory(idCategory,pageNumber, pageSize);
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
        public ActionResult<SoRecipe> TheMostLiked()
        {
            try
            {
                QRecipe qRecipe = new();
                _so.data.listDto = qRecipe.TopThreeMostLiked();
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
        public ActionResult<SoRecipe> GetById( Guid id )
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _so.data.dto = null;
                    _so.message.listMessage.Add("Proporcione la ID de categoria válida.");
                    return Unauthorized(_so.message);
                }
                
                QRecipe qRecipe = new();
                _so.data.dto = qRecipe.GetByIdIncludeImageVideo(id);
                if (_so.data.dto is not null)
                {
                    _so.message.Success();
                }
                else
                {
                    _so.data.dto = null;
                    _so.message.listMessage.Add("Receta no encontrada.");
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
        public ActionResult<SoRecipe> GetAll()
        {
            try
            {
                QRecipe qRecipe = new();
                _so.data.listDto = qRecipe.GetAll();
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
        [HttpPost]
        [Route("[action]")]
        public ActionResult<DtoMessage> Create([FromBody] SoRecipe so)
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                DtoMessage dataValidation  = ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.title),
                    nameof(so.data.dto.description),
                    nameof(so.data.dto.instruction),
                    nameof(so.data.dto.preparation),
                    nameof(so.data.dto.cooking),
                    nameof(so.data.dto.estimated),
                    nameof(so.data.dto.difficulty),
                });
                _so.message.listMessage.AddRange(dataValidation.listMessage);

                if ( Guid.Parse(so.data.dto.idCategory) == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione la ID de la Categoria.");
                }
                else
                {
                    QCategory qCategory = new QCategory();
                    if (!qCategory.ExistCategoryById(Guid.Parse(so.data.dto.idCategory) ))
                    {
                        _so.message.listMessage.Add("Proporcione una Categoria existente.");
                    }
                }
                
                if (so.data.dto.images.Count == 0)
                {
                    _so.message.listMessage.Add("Debe proporcionar al menos una imagen con URL válida.");
                }
                
                foreach (DtoImage image in so.data.dto.images)
                {
                    QImage qImage = new QImage();
                    DtoMessage ImageValidation  = ValidateDto(image, new List<string>() 
                    {
                        nameof(image.url),
                    });
                    _so.message.listMessage.AddRange(ImageValidation.listMessage);
                    
                    DtoImage dtoImageTemp = qImage.GetImageByUrl(image.url);
                    if (dtoImageTemp is not null && dtoImageTemp.url == image.url)
                    {
                        _so.message.Error();
                        _so.message.listMessage.Add("Este URL de imagen para la receta ya esta registrada proporcione otro.");
                        return Conflict( _so.message);
                    }
                }
                
                if (so.data.dto.videos.Count > 0)
                {
                    QVideo qVideo = new();
                    foreach (DtoVideo video in so.data.dto.videos)
                    {
                        DtoMessage VideoValidation  = ValidateDto(video, new List<string>() 
                        {
                            nameof(video.title),
                            nameof(video.url)
                        });
                        _so.message.listMessage.AddRange(VideoValidation.listMessage);
                        
                        DtoVideo dtoVideoTemp = qVideo.GetVideoByUrl(video.url);
                        if (dtoVideoTemp is not null && dtoVideoTemp.url == video.url)
                        {
                            _so.message.Error();
                            _so.message.listMessage.Add("Este URL de video para la receta ya esta registrada proporcione otro.");
                            return Conflict( _so.message);
                        }
                    }
                }
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);

                QRecipe qRecipe = new();
                if (qRecipe.ExistByTitle(so.data.dto.title))
                {
                    _so.message.Error();
                    _so.message.listMessage.Add("Este título para la receta ya esta registrada proporcione otro.");
                    return Conflict( _so.message);
                }
                
                if (qRecipe.CreateRecipe(so.data.dto, idUser) != 0)
                {
                    so.data.dto = null;
                    _so.message.listMessage.Add("Registro exitoso.");
                    so.message.Success();
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
        public ActionResult<DtoMessage> Update([FromBody] SoRecipe so)
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));

                DtoMessage dataValidation  = ValidateDto(so.data.dto, new List<string>() 
                {
                    nameof(so.data.dto.title),
                    nameof(so.data.dto.description),
                    nameof(so.data.dto.instruction),
                    nameof(so.data.dto.preparation),
                    nameof(so.data.dto.cooking),
                    nameof(so.data.dto.estimated),
                    nameof(so.data.dto.difficulty),
                });
                _so.message.listMessage.AddRange(dataValidation.listMessage);
                
                QCategory qCategory = new QCategory();
                QRecipe qRecipe = new();
                QImage qImage = new QImage();
                QVideo qVideo = new QVideo();
                if (so.data.dto.id == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione la ID de la receta.");
                }
                else
                {
                    if (!qRecipe.ExistById(so.data.dto.id))
                    {
                        _so.message.listMessage.Add("Esta receta no existe seleccione una existente.");
                    }
                }
                
                if (Guid.Parse(so.data.dto.idCategory) == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione la ID de la categoria.");
                }
                else
                {
                    if (!qCategory.ExistCategoryById(Guid.Parse(so.data.dto.idCategory) ))
                    {
                        _so.message.listMessage.Add("Proporcione una Categoria existente.");
                    }
                }
                
                if (so.data.dto.images.Count == 0)
                {
                    _so.message.listMessage.Add("Debe proporcionar al menos una imagen con URL válida.");
                }
                foreach (DtoImage image in so.data.dto.images)
                {
                    DtoMessage ImageValidation  = ValidateDto(image, new List<string>() 
                    {
                        nameof(image.url),
                    });
                    _so.message.listMessage.AddRange(ImageValidation.listMessage);
                }
                
                if (so.data.dto.videos.Count > 0)
                {
                    foreach (DtoVideo video in so.data.dto.videos)
                    {
                        DtoMessage VideoValidation  = ValidateDto(video, new List<string>() 
                        {
                            nameof(video.title),
                            nameof(video.url)
                        });
                        _so.message.listMessage.AddRange(VideoValidation.listMessage);
                    }
                }
                
                if (_so.message.ExistsMessage()) return BadRequest(_so.message);

                DtoRecipe dtoRecipeTemp = qRecipe.GetByTitle(so.data.dto.title);
                if (qRecipe.GetByTitle(so.data.dto.title) is not null && 
                    dtoRecipeTemp.title == so.data.dto.title && dtoRecipeTemp.id != so.data.dto.id)
                {
                    _so.message.Error();
                    _so.message.listMessage.Add("Este título para la receta ya esta registrada proporcione otro.");
                    return Conflict( _so.message);
                }

                foreach (DtoImage image in so.data.dto.images)
                {
                    if (image.id == Guid.Empty)
                    {
                        _so.message.listMessage.Add("Proporcione la ID del imagen.");
                    }
                    if (!qImage.ExistImageById(image.id))
                    {
                        _so.message.Error();
                        _so.message.listMessage.Add("Proporcione una ID de imagen existente.");
                    }
                    if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                    
                    DtoImage dtoImageTemp = qImage.GetImageByUrl(image.url);
                    DtoImage imagebyId = qImage.GetImageByIdRecipe(so.data.dto.id);
                    if (qImage.GetImageByUrl(image.url) is not null && dtoImageTemp.url == image.url && dtoImageTemp.id != imagebyId.id)
                    {
                        _so.message.Error();
                        _so.message.listMessage.Add("Este URL de imagen para la receta ya esta registrada proporcione otro.");
                        return Conflict( _so.message);
                    }
                }

                if (so.data.dto.videos.Count > 0)
                {
                    foreach (DtoVideo video in so.data.dto.videos)
                    {
                        if (video.id == Guid.Empty)
                        {
                            _so.message.listMessage.Add("Proporcione la ID del video.");
                        }
                        if (!qVideo.ExistVideoById(video.id))
                        {
                            _so.message.Error();
                            _so.message.listMessage.Add("Proporcione una ID de video existente.");
                        }
                        if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                        
                        DtoVideo dtoVideoTemp = qVideo.GetVideoByUrl(video.url);
                        DtoVideo videobyId = qVideo.GetVideoByIdRecipe(so.data.dto.id);
                        if (qVideo.GetVideoByUrl(video.url) is not null && dtoVideoTemp.url == video.url && dtoVideoTemp.id != videobyId.id)
                        {
                            _so.message.Error();
                            _so.message.listMessage.Add("Este URL de video para la receta ya esta registrada proporcione otro.");
                            return Conflict( _so.message);
                        }
                    }
                }
                
                if (qRecipe.UpdateRecipe(so.data.dto, idUser) != 0)
                {
                    so.data.dto = null;
                    _so.message.listMessage.Add("Actualización exitoso.");
                    so.message.Success();
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
        [HttpDelete]
        [Route("[action]")]
        public ActionResult<SoRecipe> Delete(Guid id, [FromBody] PasswordRequest data)
        {
            try
            {
                _so.data.dto = null;
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                QRecipe qRecipe = new QRecipe();

                DtoMessage userValidation= ValidateDto(data, new List<string>() 
                {
                    nameof(data.Password),
                });
                _so.message.listMessage.AddRange(userValidation.listMessage);
                
                if (id == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione la ID de receta válida.");
                }
                else
                {
                    if (!qRecipe.ExistById(id))
                    {
                        _so.message.listMessage.Add("Esta receta no existe seleccione una existente.");
                    }
                }

                if (_so.message.ExistsMessage()) return BadRequest(_so.message);
                
                QUser qUser = new();
                DtoUser dtoUser = qUser.GetById(idUser);
                if (BCrypt.Net.BCrypt.Verify( data.Password, dtoUser.authetication.password))
                {
                    qRecipe.DeleteOnCascade(id);
                    _so.message.listMessage.Add("Receta eliminado correctamente.");
                    _so.message.Success();
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

            return _so;
        }
        
        [Authorize(Roles = "Logged")]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<SoRecipe> WhatDoYouLike()
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                QRecipe qRecipe = new();
                _so.data.listDto = qRecipe.RecipesYouLiked(idUser);
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
