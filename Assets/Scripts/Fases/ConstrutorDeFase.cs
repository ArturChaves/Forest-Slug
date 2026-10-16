using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Plataforma.Fases
{
    public static class ConstrutorDeFase
    {
        public const int OrdemCapim = -10;
        public const int OrdemTerreno = 0;

        public struct Surgimento
        {
            public char Simbolo;
            public Vector2Int Celula;

            public Vector2 Centro => new Vector2(Celula.x + 0.5f, Celula.y + 0.5f);
        }

        public sealed class Resultado
        {
            public MapaDaFase Mapa;
            public Vector2 InicioJogador;
            public readonly List<Surgimento> Surgimentos = new List<Surgimento>();
            public Rect Limites;
        }

        public static Resultado Construir(DadosDaFase dados, Transform pai)
        {
            Resultado resultado = new Resultado();
            MapaDaFase mapa = new MapaDaFase(dados.Mapa);
            resultado.Mapa = mapa;
            resultado.Limites = new Rect(0f, 0f, mapa.Largura, mapa.Altura);
            resultado.InicioJogador = new Vector2(2.5f, 3.5f);

            GameObject objetoGrade = new GameObject("Grid");
            objetoGrade.transform.SetParent(pai, false);
            objetoGrade.AddComponent<Grid>();

            Tilemap capim = CriarTilemap(objetoGrade.transform, "Grama", OrdemCapim);
            Tilemap terreno = CriarTilemap(objetoGrade.transform, "Terreno", OrdemTerreno);

            List<Vector2Int> rampasEsquerda = new List<Vector2Int>();
            List<Vector2Int> rampasDireita = new List<Vector2Int>();

            for (int y = 0; y < mapa.Altura; y++)
            {
                for (int x = 0; x < mapa.Largura; x++)
                {
                    char simbolo = mapa.Obter(x, y);
                    Vector3Int posicao = new Vector3Int(x, y, 0);

                    if (simbolo == MapaDaFase.Solido)
                    {
                        PecaDeTerreno peca = mapa.Peca(x, y);
                        bool ehRampa = peca == PecaDeTerreno.RampaEsquerda || peca == PecaDeTerreno.RampaDireita;
                        Vector2Int celula = CatalogoDeTiles.CelulaDe(peca);
                        terreno.SetTile(posicao, CatalogoDeTiles.ObterTile(celula.x, celula.y,
                            ehRampa ? Tile.ColliderType.None : Tile.ColliderType.Grid));

                        if (peca == PecaDeTerreno.RampaEsquerda)
                        {
                            rampasEsquerda.Add(new Vector2Int(x, y));
                        }
                        else if (peca == PecaDeTerreno.RampaDireita)
                        {
                            rampasDireita.Add(new Vector2Int(x, y));
                        }
                        else if (peca == PecaDeTerreno.Topo && y + 1 < mapa.Altura)
                        {
                            Vector2Int pontas = CatalogoDeTiles.CelulaCapim;
                            capim.SetTile(new Vector3Int(x, y + 1, 0),
                                CatalogoDeTiles.ObterTile(pontas.x, pontas.y, Tile.ColliderType.None));
                        }
                        continue;
                    }

                    if (simbolo == 'P')
                    {
                        resultado.InicioJogador = new Vector2(x + 0.5f, y + 0.5f);
                    }
                    else if (simbolo != MapaDaFase.Vazio)
                    {
                        resultado.Surgimentos.Add(new Surgimento { Simbolo = simbolo, Celula = new Vector2Int(x, y) });
                    }
                }
            }

            AdicionarColisao(terreno, mapa, rampasEsquerda, rampasDireita);
            return resultado;
        }

        private static Tilemap CriarTilemap(Transform pai, string nome, int ordemDeDesenho)
        {
            GameObject objetoTilemap = new GameObject(nome);
            objetoTilemap.transform.SetParent(pai, false);
            Tilemap tilemap = objetoTilemap.AddComponent<Tilemap>();
            TilemapRenderer desenho = objetoTilemap.AddComponent<TilemapRenderer>();
            desenho.sortingOrder = ordemDeDesenho;
            return tilemap;
        }

        private static void AdicionarColisao(Tilemap terreno, MapaDaFase mapa,
            List<Vector2Int> rampasEsquerda, List<Vector2Int> rampasDireita)
        {
            GameObject objetoTerreno = terreno.gameObject;

            Rigidbody2D corpo = objetoTerreno.AddComponent<Rigidbody2D>();
            corpo.bodyType = RigidbodyType2D.Static;

            CompositeCollider2D composto = objetoTerreno.AddComponent<CompositeCollider2D>();
            composto.geometryType = CompositeCollider2D.GeometryType.Polygons;

            TilemapCollider2D colisorTilemap = objetoTerreno.AddComponent<TilemapCollider2D>();
            colisorTilemap.compositeOperation = Collider2D.CompositeOperation.Merge;

            foreach (Vector2Int celula in rampasEsquerda)
            {
                AdicionarRampa(objetoTerreno, new[]
                {
                    new Vector2(celula.x, celula.y),
                    new Vector2(celula.x + 1f, celula.y),
                    new Vector2(celula.x + 1f, celula.y + 1f)
                });
            }

            foreach (Vector2Int celula in rampasDireita)
            {
                AdicionarRampa(objetoTerreno, new[]
                {
                    new Vector2(celula.x, celula.y),
                    new Vector2(celula.x + 1f, celula.y),
                    new Vector2(celula.x, celula.y + 1f)
                });
            }

            float alturaParede = mapa.Altura + 20f;
            AdicionarParede(objetoTerreno, new Vector2(-0.5f, alturaParede * 0.5f - 5f), new Vector2(1f, alturaParede));
            AdicionarParede(objetoTerreno, new Vector2(mapa.Largura + 0.5f, alturaParede * 0.5f - 5f), new Vector2(1f, alturaParede));

            colisorTilemap.ProcessTilemapChanges();
            composto.GenerateGeometry();
        }

        private static void AdicionarRampa(GameObject alvo, Vector2[] pontos)
        {
            PolygonCollider2D poligono = alvo.AddComponent<PolygonCollider2D>();
            poligono.pathCount = 1;
            poligono.SetPath(0, pontos);
            poligono.compositeOperation = Collider2D.CompositeOperation.Merge;
        }

        private static void AdicionarParede(GameObject alvo, Vector2 centro, Vector2 tamanho)
        {
            BoxCollider2D parede = alvo.AddComponent<BoxCollider2D>();
            parede.offset = centro;
            parede.size = tamanho;
            parede.compositeOperation = Collider2D.CompositeOperation.Merge;
        }
    }
}
