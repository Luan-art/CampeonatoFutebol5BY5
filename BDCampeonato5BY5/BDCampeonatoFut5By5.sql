if not exists (select * from sysobjects where name='TimeFut' and xtype='U')
	CREATE TABLE TimeFut(
		id int identity (1,1),
		nome varchar(100) not null,
		apelido varchar(50) not null,
		dataCriacao date not null,
		pontos int not null,
		golsFeitos int not null,
		golsSofridos int not null,
		maiorNumGols int not null,
		saldoDeGol int not null,
		constraint pkTimeFut Primary Key(id)
	);
Go

if not exists (select * from sysobjects where name='Partida' and xtype='U')
	CREATE TABLE Partida(
		dataJogo date,
		idMandante int,
		idVisitante int,  
		golsMandante int not null,
		golsVisitante int not null,
		totalGols int not null,
		resultado varchar(30) not null,
		constraint pkPartida Primary Key(dataJogo, idMandante, idVisitante),
		foreign key (idMandante) references TimeFut (id),
		foreign key (idVisitante) references TimeFut (id),
	);
Go

CREATE or Alter PROC InserirTime
        @nome varchar(100),
	@apelido varchar(50),
	@dataCriacao date
AS
BEGIN
	
	  INSERT INTO TimeFut (Nome, Apelido, dataCriacao, pontos, golsFeitos, golsSofridos, maiorNumGols, saldoDeGol) VALUES
	  (@nome, @apelido, @dataCriacao, 0, 0, 0, 0, 0);
END;
GO


CREATE or ALTER PROC RealizarJogo
        @idMandante int,
	@idVisitante int,
	@golsMandante int,
	@golsVisitante int,
	@resultado varchar(30)
AS
BEGIN
	   DECLARE @TotalGols int;
		SET @TotalGols = @GolsMandante + @golsVisitante;

	   INSERT INTO Partida (dataJogo, idMandante, idVisitante, golsMandante, golsVisitante, totalGols , resultado) VALUES
			(GETDATE(), @idMandante, @idVisitante, @golsMandante, @golsVisitante, @totalGols, @resultado);	
		
		EXEC CompararMaiorNumGols @idMandante, @idVisitante, @golsMandante, @golsVisitante;

		IF (@resultado = 'Empate')
		BEGIN
			UPDATE TimeFut 
			SET golsFeitos = golsFeitos + @golsMandante, 
			golsSofridos = golsSofridos + @golsVisitante, 
			saldoDeGol = (golsFeitos + @golsMandante) - (golsSofridos + @golsVisitante), 
			pontos = pontos + 1 
		   WHERE id = @idMandante;
			
			UPDATE TimeFut 
			SET golsFeitos = golsFeitos + @golsVisitante, 
			golsSofridos = golsSofridos + @golsMandante, 
			saldoDeGol = (golsFeitos + @golsVisitante) - (golsSofridos + @golsMandante), 
			pontos = pontos + 1 
			WHERE id = @idVisitante;
		END
		ELSE IF (@resultado = 'Mandante Venceu')
		BEGIN
			UPDATE TimeFut
			SET golsFeitos = golsFeitos + @golsMandante, 
				golsSofridos = golsSofridos + @golsVisitante, 
				saldoDeGol = (golsFeitos + @golsMandante) - (golsSofridos + @golsVisitante), 
				pontos = pontos + 3 
			WHERE id = @idMandante;

			UPDATE TimeFut 
			SET golsFeitos = golsFeitos + @golsVisitante, 
		        golsSofridos = golsSofridos + @golsMandante, 
				saldoDeGol = (golsFeitos + @golsVisitante) - (golsSofridos + @golsMandante)
			WHERE id = @idVisitante;
		END
		ELSE IF (@resultado = 'Visitante Venceu') 
		BEGIN
			UPDATE TimeFut 
			SET golsFeitos = golsFeitos + @golsMandante, 
				golsSofridos = golsSofridos + @golsVisitante, 
				saldoDeGol = (golsFeitos + @golsMandante) - (golsSofridos + @golsVisitante)
			WHERE id = @idMandante;

		    UPDATE TimeFut
			SET golsFeitos = golsFeitos + @golsVisitante, 
				golsSofridos = golsSofridos + @golsMandante, 
				saldoDeGol = (golsFeitos + @golsVisitante) - (golsSofridos + @golsMandante), 
				pontos = pontos + 5
		   WHERE id = @idVisitante;
		END			
END;	
GO

CREATE or Alter PROCEDURE CompararMaiorNumGols
	@idMandante int,
	@idVisitante int,
	@golsMandante INT,
	@golsVisitante INT

AS
BEGIN
	DECLARE @golsMaxMandante int, @golsMaxVisitante int;

	Select @golsMaxMandante = maiorNumGols From TimeFut Where id = @idMandante;
	Select @golsMaxVisitante = maiorNumGols From TimeFut Where id = @idVisitante;

	IF( @golsMaxMandante < @golsMandante AND @golsMaxVisitante < @golsVisitante )
	BEGIN
		UPDATE TimeFut 
		Set maiorNumGols =  @golsMandante  
		where id = @idMandante

		UPDATE TimeFut 
		Set maiorNumGols =  @golsVisitante  
		where id = @idVisitante
	END
	ELSE IF (@golsMaxMandante < @golsMandante)
	BEGIN 
		UPDATE TimeFut 
		Set maiorNumGols =  @golsMandante  
		where id = @idMandante
	END
	ELSE IF (@golsMaxVisitante < @golsVisitante)
	BEGIN 
		UPDATE TimeFut 
		Set maiorNumGols =  @golsVisitante  
		where id = @idVisitante
	END
END;	
GO

--Drop Table Partida;
--Drop Table TimeFut;

--Iniciar Novo Campeonato
DELETE FROM Partida;
DELETE FROM TimeFut;

--Todas partidas
SELECT P.dataJogo, TM.nome AS 'Time Mandante', TV.nome AS 'Time Visitante', P.golsMandante, P.golsVisitante, P.totalGols, P.resultado
FROM  Partida AS P JOIN  TimeFut AS TM ON P.idMandante = TM.id JOIN TimeFut AS TV ON P.idVisitante = TV.id;

--Tabela
Select Nome, Apelido, dataCriacao, Pontos, golsFeitos, golsSofridos, MaiorNumGols, saldoDeGol from TimeFut order by pontos Desc, saldoDeGol Desc;

--Campeão
SELECT TOP 1 nome as 'Campeão', apelido, pontos, golsFeitos as 'Gols Feitos', golsSofridos as 'Gols Sofridos', saldoDeGol as 'Saldo de Gols'
FROM TimeFut ORDER BY pontos DESC, saldoDeGol DESC;

--Time que mais marcou
SELECT TOP 1 nome, golsFeitos as 'Gols Feitos', saldoDeGol  as 'Saldo De Gols' FROM TimeFut ORDER BY golsFeitos DESC;

--Time que mais sofreu gol no campeonato
SELECT TOP 1 nome, golsSofridos as 'Gols Sofridos', saldoDeGol  as 'Saldo De Gols' FROM TimeFut ORDER BY golsSofridos DESC;

--Jogo Com Mais Gols
SELECT TOP 1 P.dataJogo, TM.nome as 'Time Mandante', TV.nome as 'Time Visitante', P.TotalGols, P.Resultado
FROM Partida as P join TimeFut as TM on (P.idMandante = TM.id) join TimeFut as TV on (P.idVisitante = TV.id) 
ORDER BY TotalGols DESC;

--MaisGolsEmUmaPartida
SELECT T.nome AS 'Time', T.MaiorNumGols AS 'Maior Número de Gols desse Time em Uma Partida', P.DataJogo AS 'Data do Jogo',
    CASE WHEN P.IdMandante = T.Id THEN TV.nome ELSE TM.nome END AS 'Adversário'
FROM TimeFut T JOIN Partida P ON (P.IdMandante = T.Id AND P.GolsMandante = T.MaiorNumGols) OR (P.IdVisitante = T.Id AND P.GolsVisitante = T.MaiorNumGols)
JOIN TimeFut TM ON P.IdMandante = TM.Id  JOIN TimeFut TV ON P.IdVisitante = TV.Id
ORDER BY T.nome DESC;

