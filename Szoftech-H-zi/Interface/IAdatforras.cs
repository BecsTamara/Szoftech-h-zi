using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Interface
{
    public interface IAdatforras
    {
        AdatHalmaz AdatHalmaz { get; }  
        void TorolMindent();
    }
}
