using System.Collections.Generic;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Fases
{
    public static class CatalogoDeObjetos
    {
        public const int OrdemDeDesenho = -20;

        private static readonly Dictionary<char, string> Objetos = new Dictionary<char, string>
        {
            { 'Z', "SacosDeAreia" },
            { 'O', "Barril" },
            { 'Q', "Caixas" },
            { 'A', "Arame" },
            { 'I', "Ourico" },
            { 'U', "Barraca" },
            { 'J', "Jipe" },
            { 'T', "ArvoreQueimada" },
            { 'L', "Bandeira" },
            { '!', "Perigo" },
        };

        public static GameObject Criar(char simbolo, Vector2Int celula, Transform pai)
        {
            if (!Objetos.TryGetValue(simbolo, out string nomeArquivo))
            {
                return null;
            }

            Sprite sprite = FolhaDeSprites.CarregarImagem("Props/" + nomeArquivo, FolhaDeSprites.PixelsPorUnidade, new Vector2(0.5f, 0f));
            if (sprite == null)
            {
                return null;
            }

            GameObject objeto = new GameObject(nomeArquivo);
            objeto.transform.SetParent(pai, false);
            objeto.transform.position = new Vector3(celula.x + 0.5f, celula.y, 0f);
            SpriteRenderer desenho = objeto.AddComponent<SpriteRenderer>();
            desenho.sprite = sprite;
            desenho.sortingOrder = OrdemDeDesenho;
            return objeto;
        }
    }
}
