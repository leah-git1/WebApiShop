using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderMoreInfoDTO
    (
        int OrderId,
        DateOnly OrderDate,
        double OrderSum,
        List<LessInfoProductDTO> products
    );
}
