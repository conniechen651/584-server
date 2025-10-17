using System.Globalization;
using _584_server.Data;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolModel;

namespace _584_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(SchoolDbContext context, IHostEnvironment environment) : ControllerBase
    {
        string _pathName = Path.Combine(environment.ContentRootPath, "Data/schoolSAT.csv");
        [HttpPost("Districts")]
        public async Task<ActionResult> PostDistricts()
        {
            Dictionary<string, District> districts = await context.Districts.AsNoTracking()
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);

            CsvConfiguration config = new(CultureInfo.InvariantCulture) { 
                    HasHeaderRecord = true, HeaderValidated = null};

            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);
            List<Comp584Csv> records = csv.GetRecords<Comp584Csv>().ToList();

            foreach (Comp584Csv record in records)
            {
                if (!districts.ContainsKey(record.dname))
                {
                    District district = new()
                    {
                        Name = record.dname,
                        County = record.cname
                    };
                    districts.Add(district.Name, district);
                    await context.Districts.AddAsync(district);
                }
            }
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Schools")]
        public async Task<ActionResult> PostSchools()
        {
            Dictionary<string, District> districts = await context.Districts.AsNoTracking()
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };

            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);
            List<Comp584Csv> records = csv.GetRecords<Comp584Csv>().ToList();

            int schoolCount = 0;
            foreach (Comp584Csv record in records)
            {
                if (record.rtype== "S")
                {
                    School school = new()
                    {
                        Name = record.sname,
                        MathScore = record.AvgScrMath ?? 0,
                        WritingScore = record.AvgScrWrit ?? 0,
                        ReadingScore = record.AvgScrRead ?? 0,
                        NumTestTakers = record.NumTstTakr,
                        DistrictId = districts[record.dname].Id
                    };
                    await context.Schools.AddAsync(school);
                    schoolCount++;
                }
            }
            await context.SaveChangesAsync();

            return new JsonResult(schoolCount);
        }
    }
}