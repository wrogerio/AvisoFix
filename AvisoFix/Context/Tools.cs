using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvisoFix.Context
{
    public static class Tools
    {
        public static string getConnectionString()
        {
            return "Data Source=BD-SP\\IMP;Initial Catalog=IgecexPD;User ID=igecex;Password=user@igecex201219;Connect Timeout=360;";
        }

        public static string getNumPedComInicial()
        {
            return "59767";
        }
    }
}
