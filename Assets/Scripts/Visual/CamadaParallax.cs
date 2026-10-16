using UnityEngine;

namespace Plataforma.Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    [DefaultExecutionOrder(100)]
    public class CamadaParallax : MonoBehaviour
    {
        private Transform transformCamera;
        private Camera cam;
        private float acompanhamento;
        private float larguraImagem;
        private float ancoraX;
        private float ancoraCameraX;
        private float deslocamentoBase;

        public void Configurar(Camera cameraAlvo, Sprite sprite, float fatorAcompanhamento, float distanciaDaBase,
            int ordemDeDesenho, Color tom)
        {
            cam = cameraAlvo;
            transformCamera = cameraAlvo.transform;
            acompanhamento = fatorAcompanhamento;
            deslocamentoBase = distanciaDaBase;

            SpriteRenderer desenho = GetComponent<SpriteRenderer>();
            desenho.sprite = sprite;
            desenho.drawMode = SpriteDrawMode.Tiled;
            desenho.tileMode = SpriteTileMode.Continuous;
            desenho.sortingOrder = ordemDeDesenho;
            desenho.color = tom;

            larguraImagem = sprite.bounds.size.x;
            desenho.size = new Vector2(larguraImagem * 3f, sprite.bounds.size.y);

            ancoraX = transformCamera.position.x;
            ancoraCameraX = transformCamera.position.x;
            LateUpdate();
        }

        private void LateUpdate()
        {
            if (transformCamera == null)
            {
                return;
            }

            float camX = transformCamera.position.x;
            float x = ancoraX + (camX - ancoraCameraX) * acompanhamento;
            x += Mathf.Round((camX - x) / larguraImagem) * larguraImagem;
            float baixo = transformCamera.position.y - cam.orthographicSize + deslocamentoBase;
            transform.position = new Vector3(x, baixo, 10f);
        }
    }
}
