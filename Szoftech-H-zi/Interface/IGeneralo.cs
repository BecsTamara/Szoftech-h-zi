using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Interface
{
    public interface IGeneralo
    {
        List<Adat> General(int darab, DateTime kezdoIdo, DateTime vegIdo, double minErtek, double maxErtek, string mertekegyseg);
    }
}