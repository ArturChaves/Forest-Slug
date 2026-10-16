using Plataforma.Fases;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Plataforma.Visual
{
    public class CenarioDeFundo : MonoBehaviour
    {
        private const int LarguraDoChao = 80;
        private const float TopoDoChao = 3f;

        private Camera cam;
        private Transform corredor;
        private SpriteRenderer desenhoCorredor;
        private SpriteRenderer desenhoTroncoCorredor;
        private float percurso = 40f;
        private float rapidez = 1.6f;
        private float inicioX;
        private float ultimoX;

        public static CenarioDeFundo Criar(Camera cam, Color tom, Sprite[] quadrosCorredor, Sprite troncoCorredor)
        {
            GameObject raiz = new GameObject("Cenario");
            CenarioDeFundo cenario = raiz.AddComponent<CenarioDeFundo>();
            cenario.Construir(cam, tom, quadrosCorredor, troncoCorredor);
            return cenario;
        }

        private void Construir(Camera cameraAlvo, Color tom, Sprite[] quadrosCorredor, Sprite troncoCorredor)
        {
            cam = cameraAlvo;
            cam.orthographic = true;
            cam.orthographicSize = 6f;

            float meiaLargura = cam.orthographicSize * cam.aspect;
            inicioX = meiaLargura + 1f;
            percurso = Mathf.Max(1f, LarguraDoChao - 2f * meiaLargura - 2f);
            cam.transform.position = new Vector3(inicioX, cam.orthographicSize, cam.transform.position.z);
            ultimoX = inicioX;

            ConstrutorDeFundo.Construir(cam, tom);

            GameObject objetoGrade = new GameObject("Grid");
            objetoGrade.transform.SetParent(transform, false);
            objetoGrade.AddComponent<Grid>();
            GameObject objetoTilemap = new GameObject("Chao");
            objetoTilemap.transform.SetParent(objetoGrade.transform, false);
            Tilemap tilemap = objetoTilemap.AddComponent<Tilemap>();
            objetoTilemap.AddComponent<TilemapRenderer>().sortingOrder = ConstrutorDeFase.OrdemTerreno;

            Vector2Int topo = CatalogoDeTiles.CelulaDe(PecaDeTerreno.Topo);
            Vector2Int miolo = CatalogoDeTiles.CelulaDe(PecaDeTerreno.Miolo);
            for (int x = 0; x < LarguraDoChao; x++)
            {
                for (int y = 0; y < (int)TopoDoChao; y++)
                {
                    Vector2Int celula = y == (int)TopoDoChao - 1 ? topo : miolo;
                    tilemap.SetTile(new Vector3Int(x, y, 0), CatalogoDeTiles.ObterTile(celula.x, celula.y, Tile.ColliderType.None));
                }
            }

            char[] objetos = { 'Z', 'T', 'A', 'O', 'I', 'Q', 'J', 'U' };
            for (int i = 0, x = 4; x < LarguraDoChao - 4; i++, x += 7)
            {
                CatalogoDeObjetos.Criar(objetos[i % objetos.Length], new Vector2Int(x, (int)TopoDoChao), transform);
            }

            if (quadrosCorredor != null && quadrosCorredor.Length > 0)
            {
                GameObject objetoCorredor = new GameObject("Soldado");
                objetoCorredor.transform.SetParent(transform, false);
                desenhoCorredor = objetoCorredor.AddComponent<SpriteRenderer>();
                desenhoCorredor.sortingOrder = 10;
                objetoCorredor.AddComponent<AnimadorDeSprite>().Tocar(quadrosCorredor, quadrosCorredor.Length > 1 ? 12f : 1f);
                objetoCorredor.transform.localScale = Vector3.one * 2f;
                corredor = objetoCorredor.transform;

                if (troncoCorredor != null)
                {
                    GameObject objetoTronco = new GameObject("Tronco");
                    objetoTronco.transform.SetParent(corredor, false);
                    desenhoTroncoCorredor = objetoTronco.AddComponent<SpriteRenderer>();
                    desenhoTroncoCorredor.sprite = troncoCorredor;
                    desenhoTroncoCorredor.sortingOrder = 11;
                }
            }
        }

        private void LateUpdate()
        {
            if (cam == null)
            {
                return;
            }

            float x = inicioX + Mathf.PingPong(Time.time * rapidez, percurso);
            cam.transform.position = new Vector3(x, cam.orthographicSize, cam.transform.position.z);

            if (corredor != null)
            {
                float meiaLargura = cam.orthographicSize * cam.aspect;
                corredor.position = new Vector3(x - meiaLargura * 0.55f, TopoDoChao + 0.94f, 0f);
                if (Mathf.Abs(x - ultimoX) > 0.0001f)
                {
                    desenhoCorredor.flipX = x < ultimoX;
                    if (desenhoTroncoCorredor != null)
                    {
                        desenhoTroncoCorredor.flipX = desenhoCorredor.flipX;
                    }
                }
            }
            ultimoX = x;
        }
    }
}
