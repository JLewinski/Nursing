using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing.Services;

public class EFDatabase : Models.NursingContext
{
    public EFDatabase(DbContextOptions<EFDatabase> options) : base(options)
    {
    }

    public void Migrate()
    {
        Database.Migrate();
    }
}
