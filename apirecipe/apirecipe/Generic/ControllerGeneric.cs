using apirecipe.DataTransferObject.OtherObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace apirecipe.Generic
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ControllerGeneric<So> : ControllerBase
    {
        public So _so;

        public ControllerGeneric()
        {
            _so = (So)Activator.CreateInstance(typeof(So), new object[] {});
        }

        protected DtoMessage ValidateDto(object dto, List<string> listField)
        {
            DtoMessage dtoMessage = new DtoMessage();
            List<string> errors = new List<string>();
    
            ModelState.Clear();
            TryValidateModel(dto);

            foreach (string fieldName in listField)
            {
                ModelState.TryGetValue(fieldName, out ModelStateEntry modelState);

                if (modelState is not null && modelState.Errors.Count > 0)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        string customErrorMessage = error.ErrorMessage;

                        if (customErrorMessage.Contains("required"))
                        {
                            customErrorMessage = "El campo es obligatorio.";
                        }

                        if (!string.IsNullOrWhiteSpace(customErrorMessage))
                        {
                            errors.Add($"'{fieldName}': {customErrorMessage}");
                        }
                    }
                }
            }
            
            if (errors.Count > 0)
            {
                dtoMessage.listMessage = errors;
                dtoMessage.Error();
            }

            return dtoMessage;
        }
    }
}