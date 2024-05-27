using CampeonatoFutebol5BY5;
using Microsoft.Data.SqlClient;
using System;
using System.Xml;

int op = 0;

do
{
    Console.Clear();
    op = MenuCamp();

    switch (op)
    {
        case 1:
            EstatisticasCampeonato();
            break;
        case 2:  
            SelecioneNovosTimes();
            break;
        default:
            Console.WriteLine("----------- Volte Sempre! -------------");
            break;
    }

} while (op != 0);

int MenuCamp()
{
    int opcao = 0;

    do
    {

        Console.WriteLine("╔════════════════════════════════════════════╗");
        Console.WriteLine("║                                            ║");
        Console.WriteLine("║     BEM VINDO AO CAMPEONATO DA 5BY5        ║");
        Console.WriteLine("║                                            ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");

        Console.WriteLine("\n\n Gostaria de verificar as estatisticas do antigo campeonato [1] ou realizar um novo[2]?");
        Console.WriteLine("Caso queira ir embora [0]");


        try
        {
            opcao = int.Parse(Console.ReadLine());
        }
        catch (Exception e)
        {
            Console.WriteLine("Voce deve digitar um numero!");
        }

    } while (opcao < 0 || opcao > 2);

    return opcao;
}

void SelecioneNovosTimes()
{
    Console.Clear();

    int num = 0;
    do
    {
        Console.WriteLine("Digite quantos time estarão no campeonato (3 a 5)");

        try
        {
            num = int.Parse(Console.ReadLine());

        }
        catch (Exception e)
        {
            Console.WriteLine("Digite um número");
        }

    } while (num < 3 || num > 5);

    TimeFut tf = new TimeFut();

    LimparCampeonato();

    for (int i = 0; i < num; i++)
    {
        tf.ReceberDados();
    }

    int op = OpcaoDeCampeonato();

    switch (op)
    {
        case 1:
            SimularCampeonato();
            Console.Clear();
            VerificarPartidas();
            break;
        case 2:
            CriarCampeonatoMao();
            Console.Clear();
            VerificarPartidas();
            break;
        default:
            Console.WriteLine("Opção invalida");
            break;
    }
}

void CriarCampeonatoMao()
{
    List<int> timeIds = RecuperarTimeIds();
    List<string> nomesTimes = RecuperarTimesNome();
    Console.WriteLine("Times recuperados");

    for (int i = 0; i < timeIds.Count - 1; i++)
    {
        for (int j = i + 1; j < timeIds.Count; j++)
        {
            InserirPartida(timeIds[i], timeIds[j], nomesTimes[i], nomesTimes[j], "ida");
            InserirPartida(timeIds[j], timeIds[i], nomesTimes[j], nomesTimes[i], "volta");
        }
    }

    Console.WriteLine("Campeonato Simulado");
    Console.ReadKey();
}

void InserirPartida(int idMandante, int idVisitante, string nomeMandante, string nomeVisitante, string tipoPartida)
{
    int golsMandante = 0, golsVisitante = 0;
    string resultado;

    Console.WriteLine($"Partida entre {nomeMandante} contra {nomeVisitante} ({tipoPartida})");
    try
    {
        Console.WriteLine("Digite gols mandante:");
        golsMandante = int.Parse(Console.ReadLine());
        Console.WriteLine("Digite gols visitante:");
        golsVisitante = int.Parse(Console.ReadLine());
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao digitar número");
    }

    if (golsMandante > golsVisitante)
    {
        resultado = "Mandante Venceu";
    }
    else if (golsMandante < golsVisitante)
    {
        resultado = "Visitante Venceu";
    }
    else
    {
        resultado = "Empate";
    }

    Partida partida = new(idMandante, idVisitante, golsMandante, golsVisitante, resultado);
    partida.AdicionarNoBancoDeDados();
}

void SimularCampeonato()
{
    List<int> timeIds = RecuperarTimeIds();
    Console.WriteLine("Time recuperados");
    for (int i = 0; i < timeIds.Count - 1; i++)
    {
        for (int j = i + 1; j < timeIds.Count; j++)
        {
            SimularJogo(timeIds[i], timeIds[j]);
            SimularJogo(timeIds[j], timeIds[i]);
        
        }
    }
    Console.WriteLine("Campeonato Simulado");

    Console.ReadKey();
}

void SimularJogo(int mandanteId, int visitanteId)
{
    int golsMandante = new Random().Next(0, 8);
    int golsVisitante = new Random().Next(0, 8);

    string resultado;
    if (golsMandante > golsVisitante)
    {
        resultado = "Mandante Venceu";
    }
    else if (golsMandante < golsVisitante)
    {
        resultado = "Visitante Venceu";
    }
    else
    {
        resultado = "Empate";
    }

    Partida partida = new(mandanteId, visitanteId, golsMandante, golsVisitante, resultado);
    partida.AdicionarNoBancoDeDados();
}

List<int> RecuperarTimeIds()
{
    List<int> timeIds = new List<int>();
    Banco conn = new Banco();
    SqlConnection conexaosql = new SqlConnection(conn.Caminho());
    conexaosql.Open();

    try
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conexaosql;
        cmd.CommandText = "SELECT id FROM TimeFut";
        SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            timeIds.Add(id);
        }
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

    return timeIds;
}

List<string> RecuperarTimesNome()
{
    List<string> timeNome = new List<string>();
    Banco conn = new Banco();
    SqlConnection conexaosql = new SqlConnection(conn.Caminho());
    conexaosql.Open();

    try
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conexaosql;
        cmd.CommandText = "SELECT nome FROM TimeFut";
        SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string nome = reader.GetString(0);
            timeNome.Add(nome);
        }
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

    return timeNome;
}

int OpcaoDeCampeonato()
{
    Console.Clear();
    int opcao = 0;

    do
    {

        Console.WriteLine("\n Simular o campeonato [1] ou adcionar os dados do campeonato na mão[2]?");

        try
        {
            opcao = int.Parse(Console.ReadLine());
        }
        catch (Exception e)
        {
            Console.WriteLine("Voce deve digitar um numero!");
        }

    } while (opcao < 1 || opcao > 2);

    return opcao;
}

void LimparCampeonato()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conexaosql;

        cmd.CommandText = "DELETE FROM Partida";

        cmd.ExecuteNonQuery();

        cmd.Connection = conexaosql;

        cmd.CommandText = "DELETE FROM TimeFut";

        cmd.ExecuteNonQuery();

        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }
}

void EstatisticasCampeonato()
{
    int opcao = 0;

    do
    {
        Console.Clear();

        Console.WriteLine("╔═════════════════════════════════════════════╗");
        Console.WriteLine("║                                             ║");
        Console.WriteLine("║    Mostrar estatísticas do campeonato       ║");
        Console.WriteLine("║                  passado                    ║");
        Console.WriteLine("║                                             ║");
        Console.WriteLine("╚═════════════════════════════════════════════╝\n\n");

        Console.WriteLine("Escolha uma opção:\n");
        Console.WriteLine("1 - Mostrar campeão");
        Console.WriteLine("2 - Mostrar tabela");
        Console.WriteLine("3 - Mostrar time com mais gols");
        Console.WriteLine("4 - Time que tomou mais gols");
        Console.WriteLine("5 - Jogo com mais gols do campeonato");
        Console.WriteLine("6 - Maior número de gols em um único jogo (caso tenha acontecido mais de uma vez, será mostrado)\n");

        try
        {
            opcao = int.Parse(Console.ReadLine());

        }
        catch (Exception e)
        {
            Console.WriteLine("Voce deve digitar um numero!");
        }

        switch (opcao)
        {
            case 1:
                MostarCampeao();
                break;
            case 2:
                MostarTabela();
                break;
            case 3:
                MostrarTimeQueMarcouMaisGols();
                break;
            case 4:
                MostrarTimeQueMaisTomouGol();
                break;
            case 5:
                JogoComMaisGolsCampeonato();
                break;
            case 6:
                TabelaMaiorNumeroGolsUmJogo();
                break;
            default:
                Console.WriteLine("--------------- Essa opção é invalida ---------------");
                break;
        }

    } while (opcao < 0 || opcao > 6);
}

void TabelaMaiorNumeroGolsUmJogo()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.CommandText = "SELECT T.nome, T.MaiorNumGols , P.DataJogo A," +
            " CASE WHEN P.IdMandante = T.Id THEN TV.nome ELSE TM.nome END " +
            "FROM TimeFut T JOIN Partida P ON (P.IdMandante = T.Id AND P.GolsMandante = T.MaiorNumGols) OR (P.IdVisitante = T.Id AND P.GolsVisitante = T.MaiorNumGols)" +
            "JOIN TimeFut TM ON P.IdMandante = TM.Id  JOIN TimeFut TV ON P.IdVisitante = TV.Id ORDER BY T.nome DESC;";

        cmd.Connection = conexaosql;

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            while (reader.Read())
            {
                Console.WriteLine("Nome: {0}", reader.GetString(0));
                Console.WriteLine("Maior Número de Gols desse Time em Uma Partida: {0}", reader.GetInt32(1));
                Console.WriteLine("Data do Jogo: {0}", reader.GetDateTime(2).ToString("dd/MM/yyyy"));
                Console.WriteLine("Adversário: {0}", reader.GetString(3));
                Console.WriteLine();
            }

        }
        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }

    Console.ReadKey();
}

void JogoComMaisGolsCampeonato()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.CommandText = "SELECT TOP 1 P.dataJogo, TM.nome as 'Time Mandante', TV.nome as 'Time Visitante', P.TotalGols, P.Resultado FROM Partida as P join TimeFut as TM on (P.idMandante = TM.id) join TimeFut as TV on (P.idVisitante = TV.id) ORDER BY TotalGols DESC;";

        cmd.Connection = conexaosql;

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            if (reader.Read())
            {
                Console.WriteLine("Data Jogo: {0}", reader.GetDateTime(0).ToString("dd/MM/yyyy"));
                Console.WriteLine("Mandante: {0}", reader.GetString(1));
                Console.WriteLine("Visitante: {0}", reader.GetString(2));
                Console.WriteLine("Total Gols: {0}", reader.GetInt32(3));
                Console.WriteLine("Resultado: {0}", reader.GetString(4));
            }
            else
            {
                Console.WriteLine("Nenhum registro encontrado.");
            }
        }
        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }

    Console.ReadKey();
}

void MostrarTimeQueMaisTomouGol()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.CommandText = "SELECT TOP 1 nome, golsSofridos as 'Gols Sofridos', saldoDeGol  as 'Saldo De Gols' FROM TimeFut ORDER BY golsSofridos DESC;";

        cmd.Connection = conexaosql;

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            if (reader.Read())
            {
                Console.WriteLine("Nome: {0}", reader.GetString(0));
                Console.WriteLine("Gols Sofridos: {0}", reader.GetInt32(1));
                Console.WriteLine("Saldo de Gols: {0}", reader.GetInt32(2));
            }
            else
            {
                Console.WriteLine("Nenhum registro encontrado.");
            }
        }
        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }

    Console.ReadKey();
}

void MostrarTimeQueMarcouMaisGols()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.CommandText = "SELECT TOP 1 nome, golsFeitos as 'Gols Feitos', saldoDeGol  as 'Saldo De Gols' FROM TimeFut ORDER BY golsFeitos DESC;";

        cmd.Connection = conexaosql;

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            if (reader.Read())
            {
                Console.WriteLine("Nome: {0}", reader.GetString(0));
                Console.WriteLine("Gols Feitos: {0}", reader.GetInt32(1));
                Console.WriteLine("Saldo de Gols: {0}", reader.GetInt32(2));
            }
            else
            {
                Console.WriteLine("Nenhum registro encontrado.");
            }
        }
        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }

    Console.ReadKey();
}

void MostarTabela()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.CommandText = "Select Nome, Apelido, dataCriacao, Pontos, golsFeitos, golsSofridos, MaiorNumGols, saldoDeGol from TimeFut order by pontos Desc, saldoDeGol Desc;";

        cmd.Connection = conexaosql;

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            while (reader.Read())
            {
                Console.WriteLine("Nome: {0}", reader.GetString(0));
                Console.WriteLine("Apelido: {0}", reader.GetString(1));
                Console.WriteLine("Data Criacao: {0}", reader.GetDateTime(2).ToString("dd/MM/yyyy"));
                Console.WriteLine("Pontos: {0}", reader.GetInt32(3));
                Console.WriteLine("Gols Feitos: {0}", reader.GetInt32(4));
                Console.WriteLine("Gols Sofridos: {0}", reader.GetInt32(5));
                Console.WriteLine("Maior numéro de gols em partida: {0}", reader.GetInt32(6));
                Console.WriteLine("Saldo De Gols: {0}", reader.GetInt32(7));
                Console.WriteLine();
            }

        }
        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }

    Console.ReadKey();
}

void MostarCampeao()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.CommandText = "SELECT TOP 1 nome as 'Campeão', apelido, pontos, golsFeitos as 'Gols Feitos', golsSofridos as 'Gols Sofridos', saldoDeGol as 'Saldo de Gols' FROM TimeFut ORDER BY pontos DESC, saldoDeGol DESC;";

        cmd.Connection = conexaosql;

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            if (reader.Read())
            {
                Console.WriteLine("Campeão: {0}", reader.GetString(0));
                Console.WriteLine("Apelido: {0}", reader.GetString(1));
                Console.WriteLine("Pontos: {0}", reader.GetInt32(2));
                Console.WriteLine("Gols Feitos: {0}", reader.GetInt32(3));
                Console.WriteLine("Gols Sofridos: {0}", reader.GetInt32(4));
                Console.WriteLine("Saldo de Gols: {0}", reader.GetInt32(5));
            }
            else
            {
                Console.WriteLine("Nenhum registro encontrado.");
            }
        }
        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }

    Console.ReadKey();

}

void VerificarPartidas()
{
    try
    {
        Banco conn = new Banco();
        SqlConnection conexaosql = new SqlConnection(conn.Caminho());
        conexaosql.Open();

        SqlCommand cmd = new SqlCommand();

        cmd.CommandText = "SELECT P.dataJogo, TM.nome AS 'Time Mandante', TV.nome AS 'Time Visitante', P.golsMandante, P.golsVisitante, P.totalGols, P.resultado " +
            "FROM Partida AS P JOIN TimeFut AS TM ON P.idMandante = TM.id JOIN TimeFut AS TV ON P.idVisitante = TV.id; ";

        cmd.Connection = conexaosql;

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            while (reader.Read())
            {
                Console.WriteLine("Data do Jogo: {0}", reader.GetDateTime(0).ToString("dd/MM/yyyy"));
                Console.WriteLine("Time Mandante: {0}", reader.GetString(1));
                Console.WriteLine("Time Visitante: {0}", reader.GetString(2));
                Console.WriteLine("Gols do Mandante: {0}", reader.GetInt32(3));
                Console.WriteLine("Gols do Visitante: {0}", reader.GetInt32(4));
                Console.WriteLine("Total de Gols: {0}", reader.GetInt32(5));
                Console.WriteLine("Resultado: {0}", reader.GetString(6));
                Console.WriteLine();
            }

        }
        conexaosql.Close();

    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.ReadKey();
    }

    Console.ReadKey();
}