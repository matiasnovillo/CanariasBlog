﻿namespace CanariasBlog.ConnectionStrings
{
    public static class ConnectionStrings
    {
        public static string Production()
        {
            return "";
        }

        public static string Development()
        {
            return "data source =.; initial catalog = fiyistack_CanariasBlog; Integrated Security = SSPI; MultipleActiveResultSets=True;Pooling=false;Persist Security Info=True;App=EntityFramework;TrustServerCertificate=True";
        }

    }
}
