USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[irpf]    Script Date: 30/04/2018 02:13:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[irpf](
	[id] [int] NOT NULL,
	[valor_min] [decimal](9, 2) NOT NULL,
	[valor_max] [decimal](9, 2) NOT NULL,
	[taxa] [decimal](9, 2) NOT NULL,
	[deducao] [decimal](9, 2) NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_irpf] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[irpf] ADD  CONSTRAINT [DF_irpf_ativo]  DEFAULT ((1)) FOR [ativo]
GO


