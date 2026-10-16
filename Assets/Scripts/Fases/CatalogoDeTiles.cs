using System.Collections.Generic;
using Plataforma.Visual;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Plataforma.Fases
{
    public static class CatalogoDeTiles
    {
        private const int Colunas = 10;

        private static readonly Dictionary<int, Tile> CacheDeTiles = new Dictionary<int, Tile>();

        public static Vector2Int CelulaDe(PecaDeTerreno peca)
        {
            switch (peca)
            {
                case PecaDeTerreno.RampaEsquerda: return new Vector2Int(2, 4);
                case PecaDeTerreno.Topo: return new Vector2Int(3, 4);
                case PecaDeTerreno.RampaDireita: return new Vector2Int(4, 4);
                case PecaDeTerreno.ParedeEsquerda: return new Vector2Int(2, 5);
                case PecaDeTerreno.ParedeDireita: return new Vector2Int(4, 5);
                default: return new Vector2Int(3, 5);
            }
        }

        public static Vector2Int CelulaCapim => new Vector2Int(3, 3);

        public static Tile ObterTile(int coluna, int linha, Tile.ColliderType tipoColisor)
        {
            int chave = (linha * Colunas + coluna) * 4 + (int)tipoColisor;
            if (CacheDeTiles.TryGetValue(chave, out Tile emCache) && emCache != null)
            {
                return emCache;
            }

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = FolhaDeSprites.Carregar("Tiles/TilesetRuntime", 32, 32)[linha * Colunas + coluna];
            tile.colliderType = tipoColisor;
            CacheDeTiles[chave] = tile;
            return tile;
        }
    }
}
