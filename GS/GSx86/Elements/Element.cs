using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.Elements
{
    public abstract class Element : Containers5.IHaveKey<string>
    {
        
            public string Alias { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public string Key
            {
                get { return Code; }
            }
        }
    
}
