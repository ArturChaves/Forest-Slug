using UnityEngine;

namespace Plataforma.Jogo
{
    public class GotaDeSangue : MonoBehaviour
    {
        private Vector2 velocidade;
        private bool pousou;
        private float alturaDoChao;

        public static GotaDeSangue Criar(Vector2 posicao, Vector2 velocidade, Sprite sprite)
        {
            GameObject objetoGota = new GameObject("Gota");
            objetoGota.transform.position = posicao;
            SpriteRenderer desenho = objetoGota.AddComponent<SpriteRenderer>();
            desenho.sprite = sprite;
            desenho.sortingOrder = Explosao.OrdemDeDesenho - 2;

            GotaDeSangue gota = objetoGota.AddComponent<GotaDeSangue>();
            gota.velocidade = velocidade;
            gota.alturaDoChao = Sangue.ChaoAbaixo(posicao);
            return gota;
        }

        private void Update()
        {
            if (pousou)
            {
                return;
            }

            float dt = Time.deltaTime;
            velocidade.y -= 22f * dt;
            Vector3 posicao = transform.position + (Vector3)(velocidade * dt);

            alturaDoChao = Sangue.ChaoAbaixo(new Vector2(posicao.x, Mathf.Max(posicao.y, alturaDoChao + 0.5f)));
            if (posicao.y <= alturaDoChao + 0.03f && velocidade.y < 0f)
            {
                posicao.y = alturaDoChao + 0.03f;
                pousou = true;
                GetComponent<SpriteRenderer>().sortingOrder = Sangue.OrdemDasManchas + 1;
                gameObject.AddComponent<DesbotarESumir>().Configurar(8f, 2f);
            }
            transform.position = posicao;
        }
    }
}
