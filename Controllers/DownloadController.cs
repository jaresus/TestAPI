using System;
using System.Collections.Generic;

using System.Linq;

using DocumentFormat.OpenXml;


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenXMLExcel.SLExcelUtility;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly AuthenticationContext context;

        public DownloadController(AuthenticationContext context)
        {
            this.context=context;
        }
        [HttpGet]
        public ActionResult DownloadFile(
            [FromQuery] int[] skills = null,
            [FromQuery] int[] departaments = null,
            [FromQuery] bool RODO = true)
        {
            var data = new SLExcelData();
            data.SheetName = "lista ocen";

            data.Headers.Add("Dział");
            data.Headers.Add("Nr osobowy");
            data.Headers.Add("Pracownik");

            IEnumerable<Kwalifikacja> kwalifikacje;
            if (skills!=null)
            {
                kwalifikacje = context.Kwalifikacje
                    .Include(k => k.KwalifikacjaWydzial)
                    .ThenInclude(w => w.Wydzial)
                    .Where(k => k.KwalifikacjaWydzial.Any(w => skills.Contains(w.WydzialID)))
                    .OrderBy(k => k.KwalifikacjaWydzial.FirstOrDefault().WydzialID);
            }
            else
            {
                kwalifikacje = context.Kwalifikacje
                    .Include(k => k.KwalifikacjaWydzial)
                    .ThenInclude(w => w.Wydzial)
                    .OrderBy(k => k.KwalifikacjaWydzial.FirstOrDefault().WydzialID);
            }
            //sprawdzić!!!
            foreach (var k in kwalifikacje)
            {
                data.Headers.Add(k.Nazwa);
            }

            //pracownicy
            IEnumerable<Pracownik> pracownicy;
            if (departaments!=null)
            {
                pracownicy = context.Pracownicy
                    .Where(p => departaments.Contains(p.WydzialID))
                    .Where(p => p.IsActive == true)
                    .Include(p => p.Oceny)
                    .Include(p => p.Wydzial)
                    .OrderBy(p => p.Nazwisko)
                    .ThenBy(p => p.Imie);
            }
            else
            {
                pracownicy = context.Pracownicy
                    .Where(p => p.IsActive == true)
                    .Include(p => p.Oceny)
                    .Include(p => p.Wydzial)
                    .OrderBy(p => p.Nazwisko)
                    .ThenBy(p => p.Imie);
            }

            var r = new List<string>();
            Ocena o;
            foreach (var p in pracownicy)
            {
                if (p!=null)
                {
                    r.Clear();
                    r.Add(p.Wydzial?.Nazwa);
                    r.Add(p.NrPersonalny);
                    r.Add(RODO ? p.Imie : p.FullName);
                    foreach (var k in kwalifikacje)
                    {
                        o = p.Oceny.Where(o => o.KwalifikacjaID == k.ID && o.DataDo == DateTime.MaxValue.Date).FirstOrDefault();
                        r.Add(o == null ? "" : o.OcenaV.ToString());
                    }
                    data.DataRows.Add(r);
                }
            }
            var file = (new SLExcelWriter()).GenerateExcel(data);
            Response.ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Add("Content-Disposition",
                                 $"attachment; filename=Oceny {DateTime.Now.ToShortDateString()}.xlsx");
            return new FileContentResult(file, "application/xml");
        }
    }
}