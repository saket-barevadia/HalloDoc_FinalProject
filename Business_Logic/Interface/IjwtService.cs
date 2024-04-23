using System;
using Data_Layer.CustomModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Layer.DataModels;
using System.IdentityModel.Tokens.Jwt;

namespace Business_Logic.Interface
{
    public interface IjwtService
    {
        public string GenerateJwtToken(patientLogincm loginVM);
        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);
    }
}

