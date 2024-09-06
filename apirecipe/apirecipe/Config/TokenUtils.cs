using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.ObjectEnum;
using apirecipe.DataTransferObject.OtherObject;
using apirecipe.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace apirecipe.Config
{
    public class TokenUtils
    {
        public static Task<string> GenerateAccessToken(DtoUser user)
        {
            SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(AppSettings.GetAccessJwtSecret())
            );
            
            List<Claim> claims =
            [
                new Claim("id", user.id.ToString()),
                new Claim(ClaimTypes.Role, user.authetication.role.ToString())
            ];
            
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = AppSettings.GetOriginIssuer(),
                Audience = AppSettings.GetOriginAudience(),
                Expires = DateTime.UtcNow.AddHours(24),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }

        public static Task<string>  GenerateRefreshToken(DtoUser user)
        {
            SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(AppSettings.GetRefreshJwtSecret())
            );
            
            List<Claim> claims =
            [
                new Claim("id", user.id.ToString()),
                new Claim("username", user.authetication.username),
                new Claim("status", user.authetication.status.ToString()),
                new Claim(ClaimTypes.Role, user.authetication.role.ToString())
            ];
            
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = AppSettings.GetOriginIssuer(),
                Audience = AppSettings.GetOriginAudience(),
                Expires = DateTime.UtcNow.AddDays(7),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }
        
        public static async Task<(Tokens, DtoMessage)> GenerateAccessTokenFromRefreshToken(string refreshToken, string secret)
        {
            DtoMessage message = new DtoMessage();
            try
            {
                (ClaimsPrincipal primary, bool isExpired) = await ValidateToken(refreshToken, secret);
                if (isExpired)
                {
                    message.listMessage.Add("El token de actualización ha expirado.");
                    return (null, message);
                }
                if (primary == null)
                {
                    message.listMessage.Add("Token de actualización no recibido o no válido.");
                    return (null, message);
                }
                Claim userClaim = primary.Claims.FirstOrDefault(c => c.Type == "id");
                if (userClaim == null)
                {
                    message.listMessage.Add("Token de actualización no válido.");
                    return (null, message);
                }
                DtoUser dtoUser = new DtoUser
                {
                    id = Guid.Parse(userClaim.Value),
                    authetication = new DtoAuthentication
                    {
                        role = (Role)Enum.Parse(typeof(Role), primary.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value)
                    }
                };
                
                Tokens tokens = new Tokens();
                tokens.accessToken = await GenerateAccessToken(dtoUser);
                tokens.refreshToken = refreshToken;
                message.Success();
                return (tokens, message);
            }
            catch (SecurityTokenException ex)
            {
                message.listMessage.Add(ex.Message);
                return (null, message);
            }
        }

        private static Task<(ClaimsPrincipal?, bool isExpired)> ValidateToken(string token, string secret)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(secret);
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = AppSettings.GetOriginIssuer(),
                ValidAudience = AppSettings.GetOriginAudience(),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                ClaimsPrincipal primary = tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out SecurityToken validatedToken
                );
                bool isExpired = validatedToken is JwtSecurityToken jwtToken && jwtToken.ValidTo < DateTime.UtcNow;
                return Task.FromResult(
                    (validatedToken is JwtSecurityToken jwt && jwt.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) && !isExpired
                        ? primary
                        : null, isExpired)
                );
            }
            catch (SecurityTokenExpiredException)
            {
                return Task.FromResult<(ClaimsPrincipal, bool)>((null, true));
            }
            catch
            {
                return Task.FromResult<(ClaimsPrincipal, bool)>((null, false));
            }
        }


        public static string GetUserIdFromAccessToken(string accessToken)
        {
            String token = accessToken.Replace("Bearer ", "");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            Claim? accessClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "id");
            return accessClaim?.Value ?? string.Empty;
        }
    }
}
