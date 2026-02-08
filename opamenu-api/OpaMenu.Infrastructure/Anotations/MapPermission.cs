using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Anotations
{
    /// <summary>
    /// Mapeia a permissão necessária para acessar o método ou classe.
    /// </summary>
    public class MapPermission: Attribute
    {
        public List<string> Modules { get; set; } = new List<string>();

        /// <summary>
        /// Módulo principal (para compatibilidade). Retorna o primeiro da lista.
        /// </summary>
        public string Modulo 
        { 
            get => Modules.FirstOrDefault();
            set 
            {
                if (Modules.Count == 0) Modules.Add(value);
                else Modules[0] = value;
            }
        }

        public string Operation { get; set; }

        public MapPermission(string modulo, string operation)
        {
            Modules.Add(modulo);
            Operation = operation;
        }

        public MapPermission(string[] modules, string operation)
        {
            Modules.AddRange(modules);
            Operation = operation;
        }
    }
}
