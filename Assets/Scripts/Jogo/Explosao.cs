using System.Collections.Generic;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    public static class Explosao
    {
        public const int OrdemDeDesenho = 20;

        private static readonly Collider2D[] Resultados = new Collider2D[32];
        private static readonly HashSet<IAtingivel> JaAtingidos = new HashSet<IAtingivel>();

        public static void Criar(Vector2 posicao, float raio, Equipe equipe, int dano, float escalaVisual)
        {
            Efeito.Criar(ArteDoJogo.Explosao, posicao, 16f, OrdemDeDesenho, escalaVisual);

            Camera cam = Camera.main;
            CameraSeguidora acompanhamento = cam != null ? cam.GetComponent<CameraSeguidora>() : null;
            if (acompanhamento != null)
            {
                acompanhamento.Tremer(0.12f * escalaVisual, 0.25f);
            }

            if (dano <= 0 || raio <= 0f)
            {
                return;
            }

            ContactFilter2D filtro = new ContactFilter2D();
            filtro.useTriggers = true;
            int quantos = Physics2D.OverlapCircle(posicao, raio, filtro, Resultados);

            JaAtingidos.Clear();
            for (int i = 0; i < quantos; i++)
            {
                if (Resultados[i] == null)
                {
                    continue;
                }
                IAtingivel alvo = Resultados[i].GetComponentInParent<IAtingivel>();
                if (alvo != null && alvo.Equipe != equipe && alvo.EstaVivo && JaAtingidos.Add(alvo))
                {
                    alvo.ReceberDano(dano, posicao);
                }
            }
            JaAtingidos.Clear();
        }
    }
}
