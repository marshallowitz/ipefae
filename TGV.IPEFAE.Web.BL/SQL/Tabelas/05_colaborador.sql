/****** Object:  Table [dbo].[colaborador]    Script Date: 27/01/2019 20:48:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[colaborador](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[banco_id] [int] NOT NULL,
	[carteira_trabalho_estado_id] [int] NULL,
	[endereco_cidade_id] [int] NOT NULL,
	[grau_instrucao_id] [int] NOT NULL,
	[naturalidade_cidade_id] [int] NOT NULL,
	[raca_id] [int] NOT NULL,
	[nome] [varchar](100) NOT NULL,
	[cpf] [varchar](11) NOT NULL,
	[rg] [varchar](20) NOT NULL,
	[carteira_trabalho_nro] [varchar](50) NULL,
	[carteira_trabalho_serie] [varchar](50) NULL,
	[titulo_eleitor_nro] [varchar](50) NULL,
	[titulo_eleitor_zona] [varchar](50) NULL,
	[titulo_eleitor_secao] [varchar](50) NULL,
	[pis_pasep_net] [varchar](50) NOT NULL,
	[data_nascimento] [datetime] NOT NULL,
	[nacionalidade] [varchar](50) NOT NULL,
	[nome_mae] [varchar](100) NOT NULL,
	[nome_pai] [varchar](100) NULL,
	[sexo_masculino] [bit] NOT NULL,
	[estado_civil] [varchar](1) NOT NULL,
	[telefone_01] [varchar](11) NOT NULL,
	[telefone_02] [varchar](11) NULL,
	[email] [varchar](100) NOT NULL,
	[senha] [varchar](100) NOT NULL,
	[tipo_conta] [int] NOT NULL,
	[agencia] [int] NOT NULL,
	[agencia_digito] [varchar](1) NULL,
	[conta_corrente] [varchar](20) NOT NULL,
	[conta_corrente_digito] [varchar](1) NOT NULL,
	[endereco_cep] [varchar](8) NOT NULL,
	[endereco_logradouro] [varchar](500) NOT NULL,
	[endereco_nro] [varchar](10) NOT NULL,
	[endereco_bairro] [varchar](100) NOT NULL,
	[endereco_complemento] [varchar](100) NULL,
	[dados_ok] [bit] NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_colaborador] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[colaborador] ADD  CONSTRAINT [DF_colaborador_sexo_masculino]  DEFAULT ((1)) FOR [sexo_masculino]
GO

ALTER TABLE [dbo].[colaborador] ADD  CONSTRAINT [DF_colaborador_tipo_conta]  DEFAULT ((1)) FOR [tipo_conta]
GO

ALTER TABLE [dbo].[colaborador] ADD  CONSTRAINT [DF_colaborador_conta_corrente_digito]  DEFAULT ('x') FOR [conta_corrente_digito]
GO

ALTER TABLE [dbo].[colaborador] ADD  CONSTRAINT [DF_colaborador_dados_ok]  DEFAULT ((0)) FOR [dados_ok]
GO

ALTER TABLE [dbo].[colaborador] ADD  CONSTRAINT [DF_colaborador_ativo]  DEFAULT ((1)) FOR [ativo]
GO

ALTER TABLE [dbo].[colaborador]  WITH CHECK ADD  CONSTRAINT [FK_colaborador_banco] FOREIGN KEY([banco_id])
REFERENCES [dbo].[banco] ([id])
GO

ALTER TABLE [dbo].[colaborador] CHECK CONSTRAINT [FK_colaborador_banco]
GO

ALTER TABLE [dbo].[colaborador]  WITH CHECK ADD  CONSTRAINT [FK_colaborador_grau_instrucao] FOREIGN KEY([grau_instrucao_id])
REFERENCES [dbo].[grau_instrucao] ([id])
GO

ALTER TABLE [dbo].[colaborador] CHECK CONSTRAINT [FK_colaborador_grau_instrucao]
GO

ALTER TABLE [dbo].[colaborador]  WITH CHECK ADD  CONSTRAINT [FK_colaborador_raca] FOREIGN KEY([raca_id])
REFERENCES [dbo].[raca] ([id])
GO

ALTER TABLE [dbo].[colaborador] CHECK CONSTRAINT [FK_colaborador_raca]
GO

ALTER TABLE [dbo].[colaborador]  WITH CHECK ADD  CONSTRAINT [FK_colaborador_tb_cid_cidade_endereco] FOREIGN KEY([endereco_cidade_id])
REFERENCES [dbo].[tb_cid_cidade] ([cid_idt_cidade])
GO

ALTER TABLE [dbo].[colaborador] CHECK CONSTRAINT [FK_colaborador_tb_cid_cidade_endereco]
GO

ALTER TABLE [dbo].[colaborador]  WITH CHECK ADD  CONSTRAINT [FK_colaborador_tb_cid_cidade_naturalidade] FOREIGN KEY([naturalidade_cidade_id])
REFERENCES [dbo].[tb_cid_cidade] ([cid_idt_cidade])
GO

ALTER TABLE [dbo].[colaborador] CHECK CONSTRAINT [FK_colaborador_tb_cid_cidade_naturalidade]
GO

ALTER TABLE [dbo].[colaborador]  WITH CHECK ADD  CONSTRAINT [FK_colaborador_tb_est_estado] FOREIGN KEY([carteira_trabalho_estado_id])
REFERENCES [dbo].[tb_est_estado] ([est_idt_estado])
GO

ALTER TABLE [dbo].[colaborador] CHECK CONSTRAINT [FK_colaborador_tb_est_estado]
GO


