using UnityEngine;

namespace Plataforma.Fases
{
    public enum PecaDeTerreno
    {
        Nenhuma,
        Topo,
        RampaEsquerda,
        RampaDireita,
        Miolo,
        ParedeEsquerda,
        ParedeDireita
    }

    public sealed class MapaDaFase
    {
        public const char Vazio = '.';
        public const char Solido = '#';

        private readonly char[,] celulas;

        public int Largura { get; }
        public int Altura { get; }

        public MapaDaFase(string[] linhas)
        {
            Altura = linhas != null ? linhas.Length : 0;
            int largura = 0;
            for (int i = 0; i < Altura; i++)
            {
                largura = Mathf.Max(largura, linhas[i] != null ? linhas[i].Length : 0);
            }
            Largura = largura;

            celulas = new char[Largura, Altura];
            for (int y = 0; y < Altura; y++)
            {
                string linha = linhas[Altura - 1 - y] ?? string.Empty;
                for (int x = 0; x < Largura; x++)
                {
                    celulas[x, y] = x < linha.Length ? linha[x] : Vazio;
                }
            }
        }

        public char Obter(int x, int y)
        {
            if (x < 0 || x >= Largura || y < 0 || y >= Altura)
            {
                return Vazio;
            }
            return celulas[x, y];
        }

        public bool EhSolido(int x, int y)
        {
            if (y < 0 || y >= Altura)
            {
                return false;
            }
            if (x < 0 || x >= Largura)
            {
                return true;
            }
            return celulas[x, y] == Solido;
        }

        public PecaDeTerreno Peca(int x, int y)
        {
            if (!EhSolido(x, y) || x < 0 || x >= Largura)
            {
                return PecaDeTerreno.Nenhuma;
            }

            bool topo = !EhSolido(x, y + 1);
            bool esquerda = EhSolido(x - 1, y);
            bool direita = EhSolido(x + 1, y);

            if (topo)
            {
                if (esquerda == direita)
                {
                    return PecaDeTerreno.Topo;
                }
                return esquerda ? PecaDeTerreno.RampaDireita : PecaDeTerreno.RampaEsquerda;
            }

            if (!esquerda && direita)
            {
                return PecaDeTerreno.ParedeEsquerda;
            }
            if (esquerda && !direita)
            {
                return PecaDeTerreno.ParedeDireita;
            }
            return PecaDeTerreno.Miolo;
        }

        public bool EhRampa(int x, int y)
        {
            PecaDeTerreno peca = Peca(x, y);
            return peca == PecaDeTerreno.RampaEsquerda || peca == PecaDeTerreno.RampaDireita;
        }

        public bool EhChaoPlano(int x, int y)
        {
            return EhSolido(x, y) && !EhSolido(x, y + 1) && !EhRampa(x, y);
        }
    }
}
