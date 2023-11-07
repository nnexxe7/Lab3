using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lab3.Controllers
{
    public class ValuesController : ApiController
    {
        public class Osoba
        {
            public int Id { get; set; }
            public string Imie { get; set; }
            public string Miasto { get; set; }
            public int RokUrodzenia { get; set; }
        }

        public class OsobyController : ApiController
        {
            private static List<Osoba> osoby = new List<Osoba>();
            private static int currentId = 1;

            // GET api/osoby
            public IHttpActionResult Get()
            {
                return Ok(osoby);
            }

            // GET api/osoby/5
            public IHttpActionResult Get(int id)
            {
                Osoba osoba = osoby.Find(o => o.Id == id);
                if (osoba == null)
                {
                    return NotFound();
                }
                return Ok(osoba);
            }

            // POST api/osoby
            public IHttpActionResult Post([FromBody] Osoba osoba)
            {
                if (!IsValidYearOfBirth(osoba.RokUrodzenia))
                {
                    return BadRequest("Niepoprawny rok urodzenia.");
                }

                osoba.Id = currentId++;
                osoby.Add(osoba);
                return Created(new Uri(Request.RequestUri + "/" + osoba.Id), osoba);
            }

            // PUT api/osoby/5
            public IHttpActionResult Put(int id, [FromBody] Osoba osoba)
            {
                Osoba existingOsoba = osoby.Find(o => o.Id == id);
                if (existingOsoba == null)
                {
                    return NotFound();
                }

                if (!IsValidYearOfBirth(osoba.RokUrodzenia))
                {
                    return BadRequest("Niepoprawny rok urodzenia.");
                }

                existingOsoba.Imie = osoba.Imie;
                existingOsoba.Miasto = osoba.Miasto;
                existingOsoba.RokUrodzenia = osoba.RokUrodzenia;
                return Ok(existingOsoba);
            }

            // DELETE api/osoby/5
            public IHttpActionResult Delete(int id)
            {
                Osoba osoba = osoby.Find(o => o.Id == id);
                if (osoba == null)
                {
                    return NotFound();
                }
                osoby.Remove(osoba);
                return Ok();
            }

            // Sprawdzenie daty urodzenia //
            private bool IsValidYearOfBirth(int year)
            {
                int currentYear = DateTime.Now.Year;
                return year >= 1900 && year <= currentYear;
            }
        }
    }
}