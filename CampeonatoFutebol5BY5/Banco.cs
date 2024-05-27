using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampeonatoFutebol5BY5
{
    internal class Banco
    {
        string Conexao = "Data Source=127.0.0.1; Initial Catalog=DBCampeonato5By5; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes;";

        public Banco()
        {

        }
        public string Caminho()
        {
            return Conexao;
        }
    }
}
