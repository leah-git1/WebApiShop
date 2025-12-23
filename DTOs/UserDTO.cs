using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record LessInfoUserDTO
    (
        
    );
    public record MoreInfoUserDTO
    (
        int UserId,
        string FirstName,
        string LastName,
        string UserName,           
        string Password
    );
}