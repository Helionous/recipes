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
    public class RatingController : ControllerGeneric<SoRating>
    {
        [Authorize(Roles="Logged")]
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DtoMessage>> Liked( [FromBody]DtoLikeds so)
        {
            try
            {
                Guid idUser = Guid.Parse(TokenUtils.GetUserIdFromAccessToken(Request.Headers["Authorization"].ToString()));
                if (so.idRecipe == Guid.Empty)
                {
                    _so.message.listMessage.Add("Proporcione una ID de receta válida.");
                    _so.message.Error();
                    return BadRequest(_so.message);
                }

                QRecipe qRecipe = new();
                if (!qRecipe.ExistById(so.idRecipe))
                {
                    _so.message.listMessage.Add("La receta no se encuentra disponible.");
                    _so.message.Error();
                    return Conflict(_so.message);
                }

                QRating qRating = new();
                QLike qLike = new();
                if (so.status)
                {
                    // liked
                    if (await qLike.HasUserLikedRecipeAsync(idUser, so.idRecipe))
                    {
                        DtoLike? dtoLike = await qLike.GetByIdsAsync(idUser, so.idRecipe);
                        if (dtoLike.status == false)
                        {
                            if (await qRating.IsThereRecipeRatingAsync(so.idRecipe))
                            {
                                await qRating.UpdateRatingLikedAsync(so.idRecipe);
                                await qLike.UpdateStatusAsync(idUser, so.idRecipe, true);
                                _so.message.listMessage.Add("Enhorabuena!");
                            }
                            else
                            {
                                await qRating.CreateRatingLikedAsync(so.idRecipe);
                                await qLike.UpdateStatusAsync(idUser, so.idRecipe, true);
                                _so.message.listMessage.Add("Enhorabuena!");
                            }
                            _so.message.Success();
                        }
                        else
                        {
                            _so.message.listMessage.Add("Ya le dió, me gusta a esta receta!");
                            _so.message.Warning();
                        }
                    }
                    else
                    {
                        DtoLike newLike = new() { idRecipe = so.idRecipe.ToString(), idUser = idUser, status = true};
                        await qLike.GiveLikeAsync(newLike);
                        
                        if (await qRating.IsThereRecipeRatingAsync(so.idRecipe))
                        {
                            await qRating.UpdateRatingLikedAsync(so.idRecipe);
                            _so.message.listMessage.Add("Enhorabuena!");
                        }
                        else
                        {
                            await qRating.CreateRatingLikedAsync(so.idRecipe);
                            _so.message.listMessage.Add("Enhorabuena!");
                        }
                        _so.message.Success();
                    }
                }
                else
                {
                    // dislike
                    if (await qLike.HasUserLikedRecipeAsync(idUser, so.idRecipe))
                    {
                        DtoLike? dtoLike = await qLike.GetByIdsAsync(idUser, so.idRecipe);
                        if (dtoLike.status)
                        {
                            if (await qRating.IsThereRecipeRatingAsync(so.idRecipe))
                            {
                                await qRating.UpdateRatingDislikedAsync(so.idRecipe);
                                await qLike.UpdateStatusAsync(idUser, so.idRecipe, false);
                                _so.message.listMessage.Add("Enhoramala!");
                            }
                            else
                            {
                                await qRating.CreateRatingDislikedAsync(so.idRecipe);
                                await qLike.UpdateStatusAsync(idUser, so.idRecipe, false);
                                _so.message.listMessage.Add("Enhoramala!");
                            }
                            _so.message.Success();
                        }
                        else
                        {
                            _so.message.listMessage.Add("Ya le dió, no me gusta a esta receta!");
                            _so.message.Warning();
                        }
                    }
                    else
                    {
                        DtoLike newLike = new() { idRecipe = so.idRecipe.ToString(), idUser = idUser, status = false};
                        await qLike.GiveLikeAsync(newLike);
                        
                        if (await qRating.IsThereRecipeRatingAsync(so.idRecipe))
                        {
                            await qRating.UpdateRatingDislikedAsync(so.idRecipe);
                            await qLike.UpdateStatusAsync(idUser, so.idRecipe, false);
                            _so.message.listMessage.Add("Enhoramala!");
                        }
                        else
                        {
                            await qRating.CreateRatingDislikedAsync(so.idRecipe);
                            _so.message.listMessage.Add("Enhoramala!");
                        }
                        _so.message.Success();
                    }
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
