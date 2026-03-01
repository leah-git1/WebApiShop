using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record UserDTO
    (
        int UserId,
        string FirstName,
        string LastName,
        [EmailAddress]
        string UserName,           
        string Password
    );
}