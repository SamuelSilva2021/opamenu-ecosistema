using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Commons.Api.Commons
{
    public interface IJsonSerializer
    {
        string? Serialize<T>(T value);

        T? Deserialize<T>(string? json);
    }
}
