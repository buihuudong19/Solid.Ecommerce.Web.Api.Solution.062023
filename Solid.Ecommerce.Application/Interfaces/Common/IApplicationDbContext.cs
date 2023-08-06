using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Ecommerce.Application.Interfaces.Common;

public interface IApplicationDbContext
{
    DbContext DbContext { get; } // => goi xuong DB via EFC
}
