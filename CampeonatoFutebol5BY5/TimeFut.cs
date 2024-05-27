using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace CampeonatoFutebol5BY5
{
    internal class TimeFut
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string apelido { get; set; }
        public DateOnly dataCriacao { get; set; }
        public int pontos { get; set; }
        public int golsSofridos { get; set; }
        public int golsFeitos { get; set; }
        public int maiorNumGOls { get; set; }
        public int saldoGols { get; set; }

        public TimeFut(string nome, string apelido, DateOnly dataCriacao)
        {
            this.nome = nome;
            this.apelido = apelido;
            this.dataCriacao = dataCriacao;
  
        }

        public TimeFut ()
        {
    
        }

        public void ReceberDados()
        {
            DateOnly dataCriacao;

            Console.WriteLine("Digite o nome do time: ");
            string nome = Console.ReadLine();

            Console.WriteLine("Digite o Apelido do time: ");
            string apelido = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Digite a data de criação do time (dd/MM/yyyy): ");

                try
                {
                    dataCriacao = DateOnly.Parse(Console.ReadLine());
                    break;
                }catch(Exception e)
                {
                    Console.WriteLine("Data digitada Invalida, digite novamente");
                }
            }
           
            InserirTimeNoBanco(nome, apelido, dataCriacao);
        }

        private void InserirTimeNoBanco(string nome, string apelido, DateOnly dataCriacao)
        {
            Banco conn = new Banco();
            SqlConnection conexaosql = new SqlConnection(conn.Caminho());
            conexaosql.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("[dbo].[InserirTime]", conexaosql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = nome;
                cmd.Parameters.Add(new SqlParameter("@apelido", SqlDbType.VarChar)).Value = apelido;
                cmd.Parameters.Add(new SqlParameter("@dataCriacao", SqlDbType.Date)).Value = dataCriacao.ToString("yyyy-MM-dd");

                var returnValue = cmd.ExecuteNonQuery();

                Console.WriteLine("Time Inserido!");
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
            Console.ReadKey();
        }
    }
}
