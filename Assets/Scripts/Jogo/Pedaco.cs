using UnityEngine;

namespace Plataforma.Jogo
{
    public class Pedaco : MonoBehaviour
    {
        public const int OrdemDeDesenho = 9;

        private Vector2 velocidade;
        private float giro;
        private bool quicou;
        private bool pousou;
        private float tempoRastro;

        public static Pedaco Criar(Sprite sprite, Vector2 posicao, Vector2 velocidade)
        {
            GameObject objetoPedaco = new GameObject("Pedaco");
            objetoPedaco.transform.position = posicao;
            SpriteRenderer desenho = objetoPedaco.AddComponent<SpriteRenderer>();
            desenho.sprite = sprite;
            desenho.sortingOrder = OrdemDeDesenho;

            Pedaco pedaco = objetoPedaco.AddComponent<Pedaco>();
            pedaco.velocidade = velocidade;
            pedaco.giro = Random.Range(-720f, 720f);
            return pedaco;
        }

        private void Update()
        {
            if (pousou)
            {
                return;
            }

            float dt = Time.deltaTime;
            velocidade.y -= 24f * dt;
            Vector3 posicao = transform.position + (Vector3)(velocidade * dt);
            transform.Rotate(0f, 0f, giro * dt);

            tempoRastro -= dt;
            if (tempoRastro <= 0f)
            {
                tempoRastro = 0.05f;
                Sangue.Pingar(posicao);
            }

            float chao = Sangue.ChaoAbaixo(new Vector2(posicao.x, posicao.y + 0.3f)) + 0.12f;
            if (posicao.y <= chao && velocidade.y < 0f)
            {
                posicao.y = chao;
                if (!quicou)
                {
                    quicou = true;
                    velocidade = new Vector2(velocidade.x * 0.45f, -velocidade.y * 0.35f);
                    giro *= 0.4f;
                }
                else
                {
                    pousou = true;
                    GetComponent<SpriteRenderer>().sortingOrder = Sangue.OrdemDasManchas + 3;
                    gameObject.AddComponent<DesbotarESumir>().Configurar(8f, 2f);
                }
            }
            transform.position = posicao;
        }
    }
}
