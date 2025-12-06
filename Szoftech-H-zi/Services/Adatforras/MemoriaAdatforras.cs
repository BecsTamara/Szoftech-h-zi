using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;

namespace MeterologiaiAdatbazis.Services.Adatforras
{
    public class MemoriaAdatforras : IAdatforras
    {
        public AdatHalmaz AdatHalmaz { get; } = new AdatHalmaz();

        public void TorolMindent()
        {
            AdatHalmaz.Adatok.Clear();
        }
    }
}