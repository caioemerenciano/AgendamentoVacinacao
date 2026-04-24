CREATE TABLE dbo.tb_paciente (
    id_paciente INT IDENTITY(1,1) NOT NULL,
    dsc_nome VARCHAR(255) NOT NULL,
    dat_nascimento DATE NOT NULL,
    dat_criacao DATETIME NOT NULL,
    
    CONSTRAINT PK_TB_PACIENTE PRIMARY KEY (id_paciente)
);
GO