using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class Granada : MonoBehaviour
    {
        private static readonly Collider2D[] Proximos = new Collider2D[8];

        private Equipe equipe;
        private float pavio = 1.6f;
        private int quiques;
        private bool explodiu;

        public static Granada Lancar(Vector2 posicao, Vector2 velocidade, Equipe equipe, Collider2D lancador)
        {
            GameObject objetoGranada = new GameObject("Granada");
            objetoGranada.transform.position = posicao;
            Granada granada = objetoGranada.AddComponent<Granada>();
            granada.equipe = equipe;

            SpriteRenderer desenho = objetoGranada.GetComponent<SpriteRenderer>();
            desenho.sprite = ArteDoJogo.PrimeiroQuadro(ArteDoJogo.SpriteGranada);
            desenho.sortingOrder = Projetil.OrdemDeDesenho;

            CircleCollider2D circulo = objetoGranada.GetComponent<CircleCollider2D>();
            circulo.radius = 0.16f;
            circulo.sharedMaterial = new PhysicsMaterial2D("Granada") { bounciness = 0.45f, friction = 0.6f };
            if (lancador != null)
            {
                Physics2D.IgnoreCollision(circulo, lancador);
            }

            Rigidbody2D corpo = objetoGranada.GetComponent<Rigidbody2D>();
            corpo.bodyType = RigidbodyType2D.Dynamic;
            corpo.gravityScale = 2.2f;
            corpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            corpo.linearVelocity = velocidade;
            corpo.angularVelocity = velocidade.x >= 0f ? -540f : 540f;
            return granada;
        }

        private void Update()
        {
            pavio -= Time.deltaTime;
            if (pavio <= 0f || EncostouEmInimigo())
            {
                Explodir();
            }
        }

        private void OnCollisionEnter2D(Collision2D colisao)
        {
            quiques++;
            if (quiques >= 2)
            {
                pavio = Mathf.Min(pavio, 0.2f);
            }
        }

        private bool EncostouEmInimigo()
        {
            ContactFilter2D filtro = new ContactFilter2D();
            filtro.useTriggers = true;
            int quantos = Physics2D.OverlapCircle(transform.position, 0.3f, filtro, Proximos);
            for (int i = 0; i < quantos; i++)
            {
                IAtingivel alvo = Proximos[i] != null ? Proximos[i].GetComponentInParent<IAtingivel>() : null;
                if (alvo != null && alvo.Equipe != equipe && alvo.EstaVivo)
                {
                    return true;
                }
            }
            return false;
        }

        private void Explodir()
        {
            if (explodiu)
            {
                return;
            }
            explodiu = true;
            Explosao.Criar(transform.position, 1.7f, equipe, 4, 1.4f);
            Destroy(gameObject);
        }
    }
}
