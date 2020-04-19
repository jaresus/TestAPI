using System;
using System.Collections.Generic;

using System.Linq;

using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Authorization;
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

        //api/download/oceny?
        [HttpGet("oceny")]
        [Authorize(Roles = "Admin,Kierownik")]
        public ActionResult DownloadFile([FromQuery] int[] skills = null,
                                         [FromQuery] int[] departaments = null,
                                         [FromQuery] bool RODO = true)
        {


            var data = new SLExcelData();
            data.SheetName = "lista ocen";

            data.Headers.Add("Dział");
            data.Headers.Add("Nr osobowy");
            data.Headers.Add("Pracownik");
            if (skills.Length == 0) skills=null;
            if (departaments.Length == 0) departaments=null;
            List<Kwalifikacja> kwalifikacje;
            if (skills!=null)
            {
                kwalifikacje = context.Kwalifikacje
                    .Include(k => k.KwalifikacjaWydzial)
                    .ThenInclude(w => w.Wydzial)
                    .Where(k => k.KwalifikacjaWydzial.Any(w => skills.Contains(w.WydzialID)))
                    .OrderBy(k => k.KwalifikacjaWydzial.FirstOrDefault().WydzialID).ToList();
            }
            else
            {
                kwalifikacje = context.Kwalifikacje
                    .Include(k => k.KwalifikacjaWydzial)
                    .ThenInclude(w => w.Wydzial)
                    .OrderBy(k => k.KwalifikacjaWydzial.FirstOrDefault().WydzialID).ToList();
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
                    r = new List<string>
                    {
                        p.Wydzial?.Nazwa,
                        p.NrPersonalny,
                        RODO ? p.Imie : p.FullName
                    };
                    foreach (var k in kwalifikacje)
                    {
                        o = p.Oceny.Where(o => o.KwalifikacjaID == k.ID && o.DataDo == DateTime.MaxValue.Date).FirstOrDefault();
                        r.Add(o == null ? "" : o.OcenaV.ToString());
                    }
                    data.DataRows.Add(r);
                }
            }
            var file = (new SLExcelWriter()).GenerateExcel(data);
            //Response.ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.Headers.Add("Content-Disposition", $"attachment; filename=Oceny {DateTime.Now.ToShortDateString()}.xlsx");
            Response.Headers.Add("Access-Control-Expose-Headers", "content-disposition");
            return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        //api/download/archiwum
        [HttpGet("archiwum")]
        [Authorize(Roles = "Admin,Kierownik")]
        public ActionResult DownloadArchiwum()
        {
            var data = new SLExcelData();
            data.SheetName = "archiwum";
            var archiwum = context.OcenaArchiwum.ToList();
            var first = true;
            foreach (var row in archiwum)
            {
                var r= new List<string>();
                foreach (var p in row.GetType().GetProperties())
                {
                    if (first) { data.Headers.Add(p.Name); }
                    r.Add(p.GetValue(row, null).ToString()) ;
                }
                data.DataRows.Add(r);
                first=false;
            }
            var file = (new SLExcelWriter()).GenerateExcel(data);
            //Response.ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.Headers.Add("Content-Disposition", $"attachment; filename=Archiwum {DateTime.Now.ToShortDateString()}.xlsx");
            Response.Headers.Add("Access-Control-Expose-Headers", "content-disposition");
            return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }

    }
}