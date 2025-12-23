using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record LessInfoProductDTO
    (
         int ProductId,
         string ProductName,
         double? ProductPrice,
         string ProductPicture
    );
    public record MoreInfoProductDTO
   (
        int ProductId,
        string ProductName,
        double? ProductPrice,
        string ProductPicture,
        string ProductDescribtion,
        string CategoryName
   );
}
