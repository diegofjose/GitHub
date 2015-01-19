using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceSOAP.Classes
{
    public static class Enums
    {
        public enum MaillingStatus
        {
            Importado_Mailling = 1,
            Processando_Dados = 2,
            Processado_Dados_Sucesso = 3,
            Processado_Dados_Erro = 4,
            Nao_Processado_Dados = 5,
            Parado = 6,
            Cancelado = 7,
            Em_Envio = 8,
            Agendado = 9,
            Parado_Sistema = 10,
            Cancelado_Sistema = 11,
            Sem_Saldo = 12,
            Envio_concluido = 13,
            Continuando = 14,
            Sem_mailling = 15,
            Reiniciando = 16,
            Concluido = 17
        }

        public enum MaillingDadoStatus
        {
            Incluso = 0,
            Processado = 1,
            Aguardando = 2,
            Erro_de_Mensagem = 3,
            Em_Envio = 4,
            Disparado = 5,
            Erro_Numero = 6,
            BlackList = 7,
            Sem_Saldo = 8,
            Erro_ao_enviar = 9,
            Duplicado = 10,
            Nao_Entregue = 11,
            Entregue = 12,
            Erro_no_envio = 13,
            Emissao = 14
        }

        public enum Modelo
        {
            SMS_Avulso = 1,
            Arquivo_básico_TXT = 2,
            Arquivo_básico_CSV = 3,
            Concatenado_CSV = 4,
            Arquivo_TXT_Tabulação = 5,
            Arquivo_Coluna_TXT = 6,
            Layout_Padrão = 8,
            Layout_Simples_CSV = 9,
            FTP_Credigy = 10,
            Acordo = 11,
            Novo = 12,
            Linha_Digitavel = 13,
            Carta_Campanha_Acordo = 14,
            Carta_Campanha_Extrato = 15,
            API = 16,
            Acordo_2 = 17,
            Linha_Digitavel_2 = 18,
            Linha_Digitavel_3 = 21,
            Layout = 22,
            Layout_1_1 = 23,
            StandBy = 24,
            Concatenado = 25,
            Layout_2_0 = 26,
            Interativo = 28,
            Posicional_com_Tabulação = 29,
            Layout_Fattor = 30,
            Layout_CRC = 32,
            WebService = 33
        }

        public enum Prioridade
        {
            URGENTE = -2,
            IMEDIATO = -1,
            RAPIDO = 0,
            IMPORTANTE = 1,
            NORMAL = 2,
            LENTO = 3
        }
    }
}