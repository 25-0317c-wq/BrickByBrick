using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickByBrick.Services
{
    public static class DatabaseConfig
    {
        public const string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=BrickByBrick;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}