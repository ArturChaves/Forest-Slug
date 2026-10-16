using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plataforma.Visual
{
    public static class FolhaDeSprites
    {
        public const float PixelsPorUnidade = 32f;
        private const int Margem = 1;

        private static readonly Dictionary<string, Sprite[]> Cache = new Dictionary<string, Sprite[]>();

        public static Sprite[] Carregar(string caminho, int larguraCelula, int alturaCelula)
        {
            if (Cache.TryGetValue(caminho, out Sprite[] emCache) && emCache.All(s => s != null))
            {
                return emCache;
            }

            Texture2D textura = CarregarTextura(caminho);
            if (textura == null)
            {
                return new Sprite[0];
            }

            int passoX = larguraCelula + Margem * 2;
            int passoY = alturaCelula + Margem * 2;
            int colunas = textura.width / passoX;
            int linhas = textura.height / passoY;

            Sprite[] quadros = new Sprite[colunas * linhas];
            for (int linha = 0; linha < linhas; linha++)
            {
                for (int coluna = 0; coluna < colunas; coluna++)
                {
                    float x = coluna * passoX + Margem;
                    float y = textura.height - linha * passoY - Margem - alturaCelula;
                    quadros[linha * colunas + coluna] = Sprite.Create(textura, new Rect(x, y, larguraCelula, alturaCelula),
                        new Vector2(0.5f, 0.5f), PixelsPorUnidade, 0, SpriteMeshType.FullRect);
                }
            }

            Cache[caminho] = quadros;
            return quadros;
        }

        public static Sprite CarregarImagem(string caminho, float pixelsPorUnidade, Vector2 pivo)
        {
            if (Cache.TryGetValue(caminho, out Sprite[] emCache) && emCache[0] != null)
            {
                return emCache[0];
            }

            Texture2D textura = CarregarTextura(caminho);
            if (textura == null)
            {
                return null;
            }

            Sprite sprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), pivo,
                pixelsPorUnidade, 0, SpriteMeshType.FullRect);
            Cache[caminho] = new[] { sprite };
            return sprite;
        }

        public static Sprite[] Recortar(Sprite[] folha, int inicio, int quantos)
        {
            return folha.Skip(inicio).Take(quantos).ToArray();
        }

        private static Texture2D CarregarTextura(string caminho)
        {
            Texture2D textura = Resources.Load<Texture2D>(caminho);
            if (textura == null)
            {
                Debug.LogError("Imagem não encontrada: Assets/Resources/" + caminho + ".png");
                return null;
            }
            textura.filterMode = FilterMode.Point;
            textura.wrapMode = TextureWrapMode.Clamp;
            return textura;
        }
    }
}
