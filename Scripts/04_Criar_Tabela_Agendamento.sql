CREATE TABLE dbo.tb_agendamento (
    id_agendamento INT IDENTITY(1,1) NOT NULL,
    id_paciente INT NOT NULL,
    dat_agendamento DATE NOT NULL,
    hor_agendamento TIME NOT NULL,
    dsc_status VARCHAR(50) NOT NULL,
    dat_criacao DATETIME NOT NULL,
    
    CONSTRAINT PK_TB_AGENDAMENTO PRIMARY KEY (id_agendamento),
    CONSTRAINT fk_agendamento_paciente FOREIGN KEY (id_paciente) 
        REFERENCES dbo.tb_paciente (id_paciente)
);
GO