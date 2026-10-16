using System.Collections.Generic;
using Plataforma.Fases;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    public static class Sangue
    {
        public const int OrdemDasManchas = -5;
        private const int MaximoDeGotas = 160;

        private static readonly Queue<GotaDeSangue> Gotas = new Queue<GotaDeSangue>();

        public static void Acerto(Vector2 posicao, Vector2 direcao)
        {
            Efeito.Criar(ArteDoJogo.RespingoDeSangue, posicao, 20f, Explosao.OrdemDeDesenho - 1, 1.6f);
            Espirrar(posicao, direcao, 5, 4f);
        }

        public static void Morte(Vector2 posicao, Vector2 direcao)
        {
            Efeito.Criar(ArteDoJogo.RespingoDeSangue, posicao, 14f, Explosao.OrdemDeDesenho - 1, 2.4f);
            Espirrar(posicao, direcao, 12, 5f);
            Poca(posicao);
        }

        public static void Estouro(Vector2 posicao)
        {
            Efeito.Criar(ArteDoJogo.RespingoDeSangue, posicao, 12f, Explosao.OrdemDeDesenho - 1, 3.5f);
            Espirrar(posicao, Vector2.up, 26, 8f);
            Poca(posicao);
        }

        public static void ArrancarMembro(Sprite parte, Vector2 posicao, Vector2 direcao, float rapidez)
        {
            if (parte == null)
            {
                return;
            }
            Vector2 direcaoBase = direcao.sqrMagnitude > 0.001f ? direcao.normalized : Vector2.up;
            Vector2 velocidade = (direcaoBase + Vector2.up * 1.2f + Random.insideUnitCircle * 0.5f).normalized * rapidez;
            Pedaco.Criar(parte, posicao, velocidade);
            Efeito.Criar(ArteDoJogo.RespingoDeSangue, posicao, 18f, Explosao.OrdemDeDesenho - 1, 1.6f);
        }

        public static void Pingar(Vector2 posicao)
        {
            Sprite[] spritesGota = ArteDoJogo.GotasDeSangue;
            if (spritesGota.Length > 0)
            {
                Registrar(GotaDeSangue.Criar(posicao, new Vector2(Random.Range(-0.5f, 0.5f), 0f),
                    spritesGota[Random.Range(0, spritesGota.Length)]));
            }
        }

        private static void Espirrar(Vector2 posicao, Vector2 direcao, int quantos, float rapidez)
        {
            Vector2 direcaoBase = direcao.sqrMagnitude > 0.001f ? direcao.normalized : Vector2.up;
            Sprite[] spritesGota = ArteDoJogo.GotasDeSangue;
            for (int i = 0; i < quantos; i++)
            {
                Vector2 velocidade = (direcaoBase + Random.insideUnitCircle * 0.9f + Vector2.up * 0.6f).normalized
                                   * Random.Range(rapidez * 0.4f, rapidez);
                Sprite sprite = spritesGota.Length > 0 ? spritesGota[Random.Range(0, spritesGota.Length)] : null;
                Registrar(GotaDeSangue.Criar(posicao + Random.insideUnitCircle * 0.1f, velocidade, sprite));
            }
        }

        public static void Poca(Vector2 posicao)
        {
            float alturaChao = ChaoAbaixo(posicao);
            Sprite[] pocas = ArteDoJogo.PocasDeSangue;
            if (pocas.Length == 0)
            {
                return;
            }

            GameObject objetoPoca = new GameObject("Poca de Sangue");
            objetoPoca.transform.position = new Vector3(posicao.x, alturaChao + 0.12f, 0f);
            SpriteRenderer desenho = objetoPoca.AddComponent<SpriteRenderer>();
            desenho.sprite = pocas[Random.Range(0, pocas.Length)];
            desenho.sortingOrder = OrdemDasManchas;
            objetoPoca.AddComponent<DesbotarESumir>().Configurar(14f, 3f);
        }

        public static float ChaoAbaixo(Vector2 posicao)
        {
            MapaDaFase mapa = GerenciadorDaFase.Instancia.Mapa;
            int x = Mathf.FloorToInt(posicao.x);
            for (int y = Mathf.FloorToInt(posicao.y); y >= 0; y--)
            {
                if (mapa.EhSolido(x, y))
                {
                    return y + 1f;
                }
            }
            return 0f;
        }

        private static void Registrar(GotaDeSangue gota)
        {
            Gotas.Enqueue(gota);
            while (Gotas.Count > MaximoDeGotas)
            {
                GotaDeSangue maisAntiga = Gotas.Dequeue();
                if (maisAntiga != null)
                {
                    Object.Destroy(maisAntiga.gameObject);
                }
            }
        }
    }
}
