using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampeonatoFutebol5BY5
{
    internal class Partida
    {
        public DateOnly dataJogo { get; set; }
        public int idMandante { get; set; }
        public int idVisitante { get; set; }
        public int golsMandante { get; set; }
        public int golsVisitante { get; set; }
        public int totalGols {  get; set; } 
        public string resultado { get; set; }

        public Partida(int idMandante, int idVisitante, int golsMandante, int golsVisitante, string resultado)
        {
            this.idMandante = idMandante;
            this.idVisitante = idVisitante;
            this.golsMandante = golsMandante;
            this.golsVisitante = golsVisitante;
            this.resultado = resultado;
        }

        internal void AdicionarNoBancoDeDados()
        {
            Banco conn = new Banco();
            SqlConnection conexaosql = new SqlConnection(conn.Caminho());
            conexaosql.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("[dbo].[RealizarJogo]", conexaosql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idMandante", SqlDbType.Int)).Value = idMandante;
                cmd.Parameters.Add(new SqlParameter("@idVisitante", SqlDbType.Int)).Value = idVisitante;
                cmd.Parameters.Add(new SqlParameter("@golsMandante", SqlDbType.Int)).Value = golsMandante;
                cmd.Parameters.Add(new SqlParameter("@golsVisitante", SqlDbType.Int)).Value = golsVisitante;
                cmd.Parameters.Add(new SqlParameter("@resultado", SqlDbType.VarChar, 30)).Value = resultado;

                var returnValue = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nMensagem da Exception que aconteceu:");
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conexaosql.Close();
            }
        }
    }
}
